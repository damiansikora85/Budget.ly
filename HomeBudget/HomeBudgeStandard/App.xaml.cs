
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
using TinyIoC;
using HomeBudget.Code.Interfaces;
using HomeBudgeStandard.Interfaces.Impl;
using System.Threading.Tasks;
using HomeBudget.Code;
using HomeBudgetShared.Code.Synchronize;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace HomeBudget
{
    public partial class App : Application
	{
        public App()
		{
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTkyNTY1QDMxMzcyZTM0MmUzMGpiOEJDRWQ1TjRadGFRenpLL3BENTNrOTM2dEVDYytVOUZIT3BBS0hQOUU9;MTkyNTY2QDMxMzcyZTM0MmUzMFBNRmNyZ2ZXVjlrL1pZSEdWUGk3NEhpTVc3VkxrQnNXNG9lcFNJMzdMMW89");
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
                  typeof(Crashes), typeof(Push));
#else
            Microsoft.AppCenter.AppCenter.Start("android=d788ef5d-e265-4c16-abbf-2e9469285d52;" +
                  "uwp={Your UWP App secret here};" +
                  "ios={Your iOS App secret here}",
                  typeof(Analytics), typeof(Crashes), typeof(Push));
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
