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
	public partial class App : Application
	{

        public App()
		{
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
