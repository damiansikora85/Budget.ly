
using HomeBudgeStandard.Pages;
using Xamarin.Forms;

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
