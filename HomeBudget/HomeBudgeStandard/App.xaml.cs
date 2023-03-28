
using System;
using HomeBudgeStandard.Interfaces.Impl;
using HomeBudgeStandard.Pages;
using HomeBudget.Code.Interfaces;
using HomeBudget.Standard;
using Microsoft.AppCenter.Crashes;
using TinyIoC;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace HomeBudget
{
    public partial class App : Application
	{
        public App()
		{
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MjkzNzE4QDMxMzgyZTMyMmUzME0vV2dZcW96VnB4L2tQZmhoR29oSWpHL21yN2p1cEQ0b0Q5SzZIQ0srdlE9;MjkzNzE5QDMxMzgyZTMyMmUzMFE0TnpJQ3kxb1hDQnF4aUljVTlpcDcvOExUV05vVTlEVE1IU3A1cVdnVTA9");
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
                  typeof(Analytics), typeof(Crashes));
#endif

#if !CUSTOM
            DependencyService.Get<IRemoteConfig>().Init();
#endif

            if (Xamarin.Essentials.Preferences.Get("firstLaunch", true))
            {
                Xamarin.Essentials.Preferences.Set("ratePopupDisplayDate", DateTime.Now);
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
