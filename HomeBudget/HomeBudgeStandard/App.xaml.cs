
using HomeBudgeStandard.Pages;
using Xamarin.Forms;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using HomeBudgeStandard.Utils;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace HomeBudget
{
    public partial class App : Application
	{
        public App()
		{
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzAyMjhAMzEzNjJlMzMyZTMwWnNGUjdzSXhBQ1lNWGdhd3hjdXpMM3craCs1bVZISGswWUdNSnpJWHR2UT0=;MzAyMjlAMzEzNjJlMzMyZTMwWjNZRERVT0xRbVVnN1BZR0xVUVNva3JKRHhLcmtONkd5UittaU94bU8wTT0=");
            InitializeComponent();

            MainPage = new MainPage();
        }

		protected override void OnStart()
		{
            // Handle when your app starts
            Microsoft.AppCenter.AppCenter.Start("android=d788ef5d-e265-4c16-abbf-2e9469285d52;" +
                  "uwp={Your UWP App secret here};" +
                  "ios={Your iOS App secret here}",
                  typeof(Analytics), typeof(Crashes));

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
