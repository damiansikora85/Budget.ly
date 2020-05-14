using HomeBudgeStandard.Interfaces.Impl;
using HomeBudgeStandard.Utils;
using HomeBudget.Code;
using HomeBudget.Helpers;
using HomeBudgetShared.Code.Synchronize;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections;
using System.Globalization;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeBudgeStandard.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : MasterDetailPage
    {
        public MainPage()
        {
            CultureInfo.CurrentCulture = new CultureInfo("pl-PL");
            InitializeComponent();
            InitBudget();
            MasterPage.ListView.ItemSelected += ListView_ItemSelected;

            Crashes.NotifyUserConfirmation(UserConfirmation.AlwaysSend);
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

        private void InitBudget()
        {
            var crashReporter = new XamarinCrashReporter();
            MainBudget.Instance.Init(new FileManagerXamarin(), new BudgetSynchronizer(new DropboxCloudStorage(crashReporter)), crashReporter);
            if(Settings.FirstLaunch)
            {
                NotificationManager.ScheduleDefaultNotifications();
                Settings.FirstLaunch = false;
            }
        }

        public void AfterCloudLogin()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Detail = new NavigationPage(new MainTabbedPage());
                IsPresented = false;
            });
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (!(e.SelectedItem is MainPageMenuItem item))
                return;

            try
            {
                if(item.OnClick != null)
                {
                    item.OnClick.Invoke();
                }
                else if (item.TargetType != typeof(MainTabbedPage))
                {
                    var page = (Page)Activator.CreateInstance(item.TargetType);
                    Detail.Navigation.PushAsync(page);
                }
                IsPresented = false;
            }
            catch(Exception exc)
            {
                var msg = exc.Message;
            }
            MasterPage.ListView.SelectedItem = null;
        }

        protected async override void OnAppearing()
        {
            await Crashes.SetEnabledAsync(true);
            var didAppCrash = await Crashes.HasCrashedInLastSessionAsync();
        }
    }
}