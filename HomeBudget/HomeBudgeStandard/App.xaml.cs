
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
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("ODMwODFAMzEzNzJlMzEyZTMwWnBBcStFNGRXZnRMNXFsSW52My9uU0REcXFKdjhvOXlETlFOZ1l4eDB3az0=;ODMwODJAMzEzNzJlMzEyZTMwS1JPMTVidWFQb25FcHo0eEJ2ZFZRQ09lRmFvY1kwOEZrbzRVMGhXM1Vpcz0=");
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

#if (!CUSTOM)
            DependencyService.Get<IRemoteConfig>().Init();
#endif

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
