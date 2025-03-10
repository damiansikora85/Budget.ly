using System.Globalization;
using Firebase.Crashlytics;
using HomeBudget.Code;
using HomeBudget.Code.Interfaces;
using HomeBudget.Helpers;
using HomeBudgetShared.Code.Synchronize;
using HomeBudgetStandard.Interfaces.Impl;
using HomeBudgetStandard.Utils;

namespace HomeBudgetStandard.Pages
{
    public partial class MainPage : FlyoutPage
    {
        private readonly ISettings _settings;

        public MainPage()
        {
            _settings = new Settings();
            CultureInfo.CurrentCulture = new CultureInfo("pl-PL");
            InitializeComponent();
            Task.Factory.StartNew(async () => await InitBudget());
            MasterPage.ListView.ItemSelected += ListView_ItemSelected;
        }

        public bool OnBackPressed()
        {
            if (Detail is NavigationPage navigationPage && navigationPage.RootPage is MainTabbedPage mainTabbedPage)
            {
                return mainTabbedPage.OnBackPressed();
            }
            else
            {
                return false;
            }
        }

        private async Task InitBudget()
        {
            try
            {
                var crashReporter = new XamarinCrashReporter();
                var featureSwitch = TinyIoC.TinyIoCContainer.Current.Resolve<IFeatureSwitch>();

                await MainBudget.Instance.Init(new FileManagerXamarin(), new BudgetSynchronizer(new DropboxCloudStorage(crashReporter, _settings)), crashReporter, _settings, featureSwitch);
                if (_settings.FirstLaunch || _settings.NeedSetupDefaultNotifications)
                {
                    NotificationManager.ScheduleDefaultNotifications();
                    _settings.FirstLaunch = false;
                    _settings.NeedSetupDefaultNotifications = false;
                }
            }
            catch (Exception exc)
            {
                var msg = exc.Message;
            }
        }

        public void AfterCloudLogin()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var mainTabbedPage = new MainTabbedPage();
                mainTabbedPage.SetSettings(_settings);
                Detail = new NavigationPage(mainTabbedPage);
                IsPresented = false;
            });
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (!(e.SelectedItem is MainPageMenuItem item))
                return;

            try
            {
                if (item.OnClick != null)
                {
                    item.OnClick.Invoke();
                }
                else if (item.TargetType != typeof(MainTabbedPage))
                {
                    Page page;
                    if (item.TargetType == typeof(DropboxPage))
                    {
                        page = (Page)Activator.CreateInstance(item.TargetType, new object[] { _settings });
                    }
                    else
                    {
                        page = (Page)Activator.CreateInstance(item.TargetType);
                    }
                    Detail.Navigation.PushAsync(page);
                }
                IsPresented = false;
            }
            catch (Exception exc)
            {
                var msg = exc.Message;
            }
            MasterPage.ListView.SelectedItem = null;
        }

        //protected override void OnAppearing()
        //{
        //    base.OnAppearing();
        //}

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if (Detail is NavigationPage navigationPage && navigationPage.CurrentPage is MainTabbedPage mainTabbedPage)
            {
                mainTabbedPage.SetSettings(_settings);
            }

            FirebaseCrashlytics.Instance.CheckForUnsentReports();
        }
    }
}