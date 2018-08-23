
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

            MainPage = new MainPage();
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

        public bool OnBackPressed()
        {
            var masterDetail = MainPage as MainPage;
            return masterDetail.OnBackPressed();
        }
	}
}
