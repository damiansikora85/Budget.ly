using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using System.Threading.Tasks;

namespace HomeBudget
{
	public interface IAuthenticate
	{
		Task<bool> Authenticate();
	}

	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

            MainPage = new HomeBudget.MainPagePC();

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
