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
            InitBudget();
            DropboxManager.Instance.DownloadData();
        }

		protected override async void OnAppearing()
		{
			base.OnAppearing();


			if (authenticated == true)
			{

			}		
		}

		private async void OnWrite(object sender, EventArgs e)
		{
			// do something
			//NavigationPage expensesPage = new NavigationPage(new ExpensesPage());
			//await Navigation.PushModalAsync(expensesPage);

			NavigationPage categories = new NavigationPage(new ExpensesCategories());
			await Navigation.PushModalAsync(categories);
		}

        private async void OnPlan(object sender, EventArgs e)
        {
            NavigationPage planPage = new NavigationPage(new Page1());
            await Navigation.PushModalAsync(planPage);

        }


        private async void OnAnalize(object sender, EventArgs e)
		{
			// do something
			//NavigationPage expensesPage = new NavigationPage(new ExpensesPage());
			//await Navigation.PushModalAsync(expensesPage);

			NavigationPage analizePage = new NavigationPage(new AnalizePage());
			await Navigation.PushModalAsync(analizePage);
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
