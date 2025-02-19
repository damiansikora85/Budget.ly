
using System;
using HomeBudgetStandard.Interfaces.Impl;
using HomeBudgetStandard.Pages;
using HomeBudget.Code.Interfaces;
using HomeBudget.Standard;
using Microsoft.AppCenter.Crashes;
using TinyIoC;
using Microsoft.Maui;

namespace HomeBudget
{
    public partial class App : Application
	{
        public App()
		{
            //Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTcxNDE5NEAzMjMxMmUzMTJlMzMzOUc3M3NFTlJlblMzWGJXYTNjMHYxVnltZUVSYTlSNWJEaEV5dDZ1bnVLeXM9");
            InitializeComponent();
            RegisterServices(TinyIoCContainer.Current);

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
            //DependencyService.Get<IRemoteConfig>().Init();
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
