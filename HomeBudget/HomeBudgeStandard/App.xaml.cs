
using HomeBudgeStandard.Pages;
using Xamarin.Forms;

namespace HomeBudget
{
    public partial class App : Application
	{
        public App()
		{
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("OTMwNUAzMTM2MmUzMjJlMzBLM3Q2MWJvZEE2UXR0bFNleUhwMUFESUh2VDB4Wjg0YXljRlM5QjBucDFRPQ==;OTMwNkAzMTM2MmUzMjJlMzBoVHFOWTFERUxDZDBqRkt4Q3NvcDkxdDVmaDg2TndHWU9TWkhVc0R0R1V3PQ==");
            InitializeComponent();

            //SwitchToHomePage();
            MainPage = new MainPage();

            /*if (Device.Idiom == TargetIdiom.Phone)
            {
                MainPage = new HomeBudget.MainPage();
            }
            if (Device.Idiom == TargetIdiom.Tablet)
            {
                MainPage = new HomeBudget.MainPage();
            }
            else
            {
                MainPage = new HomeBudget.MainPagePC();// new NavigationPage(new HomeBudget.MainPagePC());
                //MainPage = new HomeBudget.Pages.PC.MainPagePCNew();
            }*/


        }

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}
