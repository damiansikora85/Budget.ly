using HomeBudget.Code.Interfaces;
using HomeBudgetStandard.Interfaces.Impl;
using HomeBudgetStandard.Pages;
using Microsoft.AppCenter.Crashes;
using TinyIoC;

namespace HomeBudgetMaui
{
    public partial class App : Application
    {
        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzU2Mzk5OUAzMjM3MmUzMDJlMzBmL1V1RGhLWXl4MVFJZnNvSjRRc0lpNnM5VjJGR1IxeCtyS3Y4S1k2aGFRPQ==");
            InitializeComponent();
            RegisterServices(TinyIoCContainer.Current);
            UserAppTheme = AppTheme.Light;

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
#if DEBUG
            Microsoft.AppCenter.AppCenter.Start("android=d788ef5d-e265-4c16-abbf-2e9469285d52;" +
                  "uwp={Your UWP App secret here};" +
                  "ios={Your iOS App secret here}",
                  typeof(Crashes));
#else
            Microsoft.AppCenter.AppCenter.Start("android=d788ef5d-e265-4c16-abbf-2e9469285d52;" +
                  "uwp={Your UWP App secret here};" +
                  "ios={Your iOS App secret here}",
                 typeof(Crashes));
#endif

#if !CUSTOM
            HomeBudget.Standard.RemoteConfig.Instance.Init();
#endif

            if (Preferences.Get("firstLaunch", true))
            {
                Preferences.Set("ratePopupDisplayDate", DateTime.Now);
            }

            //NotificationManager.ClearAllNotifications();
            //NotificationManager.ReScheduleNotificationsBySettings();
        }

        private void RegisterServices(TinyIoCContainer container)
        {
            container.Register<IFeatureSwitch, FeatureSwitch>().AsSingleton();
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        public bool OnBackPressed()
        {
            var masterDetail = MainPage as MainPage;
            return masterDetail.OnBackPressed();
        }
    }
}
