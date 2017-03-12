using HomeBudget.Code;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HomeBudget
{
	public partial class MainPage : ContentPage
	{
		// Track whether the user has authenticated.
		bool authenticated = false;
		private const string TEMPLATE_FILENAME = "HomeBudget.template.json";

		public MainPage()
		{
			InitializeComponent();
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			// Refresh items only when authenticated.
			if (authenticated == true)
			{
				// Set syncItems to true in order to synchronize the data
				// on startup when running in offline mode.
				//await RefreshItems(true, syncItems: false);

				// Hide the Sign-in button.
				this.LoginBtn.IsVisible = false;
			}

			InitBudget();
		}

		async void loginButton_Clicked(object sender, EventArgs e)
		{
			if (App.Authenticator != null)
				authenticated = await App.Authenticator.Authenticate();

			// Set syncItems to true to synchronize the data on startup when offline is enabled.
			/*if (authenticated == true)
				await RefreshItems(true, syncItems: false);*/
		}

		private async void OnWrite(object sender, EventArgs e)
		{
			// do something
			//NavigationPage expensesPage = new NavigationPage(new ExpensesPage());
			//await Navigation.PushModalAsync(expensesPage);

			NavigationPage jsonPage = new NavigationPage(new JsonCategories());
			await Navigation.PushModalAsync(jsonPage);
		}

		private async void OnTest(object sender, EventArgs e)
		{
			// do something
			//NavigationPage expensesPage = new NavigationPage(new ExpensesPage());
			//await Navigation.PushModalAsync(expensesPage);

			NavigationPage testPage = new NavigationPage(new NewPage());
			await Navigation.PushModalAsync(testPage);
		}

		private void InitBudget()
		{
			var assembly = typeof(MainPage).GetTypeInfo().Assembly;

			Stream stream = assembly.GetManifestResourceStream(TEMPLATE_FILENAME);
			string jsonString = "";
			using (var reader = new System.IO.StreamReader(stream))
			{
				jsonString = reader.ReadToEnd();

				MainBudget.Instance.InitWithJson(jsonString);
			}
		}
	}
}
