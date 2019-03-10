
using HomeBudgeStandard.Pages;
using Xamarin.Forms;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using HomeBudgeStandard.Utils;
using Xamarin.Forms.Xaml;
using Microsoft.AppCenter.Push;
using HomeBudget.Standard;
using System;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace HomeBudget
{
    public partial class App : Application
	{
        public App()
		{
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NTMyMThAMzEzNjJlMzQyZTMwWVlrQ3ZMeHpoL3dobHRYU0haaVhUVUNoTnY3UElTWXFJcjJaSGFiZGVzbz0=;NTMyMTlAMzEzNjJlMzQyZTMwbWdDVTd0cS9CYW82Tm8vY0V1NU1ENXg0anFiOXV3QU9PTmZ1d3o0UEhJTT0=");
            InitializeComponent();

            MainPage = new MainPage();
        }

		protected override void OnStart()
		{
            // Handle when your app starts
            Microsoft.AppCenter.AppCenter.Start("android=d788ef5d-e265-4c16-abbf-2e9469285d52;" +
                  "uwp={Your UWP App secret here};" +
                  "ios={Your iOS App secret here}",
                  typeof(Analytics), typeof(Crashes), typeof(Push));

            DependencyService.Get<IRemoteConfig>().Init();

            if (Xamarin.Essentials.Preferences.Get("firstLaunch", true))
            {
                Xamarin.Essentials.Preferences.Set("ratePopupDisplayDate", DateTime.Now); 
            }

            //NotificationManager.ClearAllNotifications();
            //NotificationManager.ReScheduleNotificationsBySettings();
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
