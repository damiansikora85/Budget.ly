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
        private Stack _pagesStack;
        public MainPage()
        {
            _pagesStack = new Stack();
            CultureInfo.CurrentCulture = new CultureInfo("pl-PL");
            InitializeComponent();
            InitBudget();
            MasterPage.ListView.ItemSelected += ListView_ItemSelected;

            _pagesStack.Push(Detail);
            Crashes.NotifyUserConfirmation(UserConfirmation.AlwaysSend);
        }

        public bool OnBackPressed()
        {
            if (Detail is NavigationPage navigationPage && navigationPage.RootPage is MainTabbedPage mainTabbedPage)
            {
                return mainTabbedPage.OnBackPressed();
            }
            else if (_pagesStack.Count > 1)
            {
                _pagesStack.Pop();
                if (_pagesStack.Peek() is NavigationPage page)
                {
                    Detail = page;
                }
                
                return true;
            }
            else
            {
                return false;
            }
        }

        private void InitBudget()
        {
            MainBudget.Instance.Init(new FileManagerXamarin(), new BudgetSynchronizer(new DropboxCloudStorage()));
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
                var page = (Page)Activator.CreateInstance(item.TargetType);

                Detail = new NavigationPage(page);
                _pagesStack.Push(Detail);
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

        protected override bool OnBackButtonPressed()
        {
            return _pagesStack.Count > 1;
        }
    }
}