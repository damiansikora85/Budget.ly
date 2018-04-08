using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using System.Threading.Tasks;
using HomeBudget.Pages.PC;
using HomeBudgeStandard.Pages;

namespace HomeBudget
{
	public interface IAuthenticate
	{
		Task<bool> Authenticate();
	}

	public partial class App : Application
	{
        private MainPagePC homePage;
        private AnalyticsPagePC analyticsPage;
        private PlanningPage planningPage;

        public MainPagePC HomePage
        {
            get
            {
                if (homePage == null)
                    homePage = new MainPagePC();
                return homePage;
            }
        }

        public AnalyticsPagePC AnalyticsPage
        {
            get
            {
                if (analyticsPage == null)
                    analyticsPage = new AnalyticsPagePC();
                return analyticsPage;
            }
        }

        public PlanningPage PlanningPage
        {
            get
            {
                if (planningPage == null)
                    planningPage = new PlanningPage();
                return planningPage;
            }
        }

        public App()
		{
			InitializeComponent();

            //SwitchToHomePage();
            MainPage = new MainPagePC();

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

        public void SwitchToHomePage() => MainPage = HomePage;   
        public void SwitchToAnalyticsPage() => MainPage = AnalyticsPage;
        public void SwitchToPlanningPage() => MainPage = PlanningPage;


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
