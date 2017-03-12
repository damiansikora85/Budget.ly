using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace HomeBudget.UWP
{
	public sealed partial class MainPage : IAuthenticate
	{
		// Define a authenticated user.
		private MobileServiceUser user;

		public async Task<bool> Authenticate()
		{
			string message = string.Empty;
			var success = false;

			try
			{
				// Sign in with Google login using a server-managed flow.
				if (user == null)
				{
					user = await HomeBudget.Code.AzureTest.MobileService
						.LoginAsync(MobileServiceAuthenticationProvider.Facebook);
					if (user != null)
					{
						success = true;
						message = string.Format("You are now signed-in as {0}.", user.UserId);
					}
				}

			}
			catch (Exception ex)
			{
				message = string.Format("Authentication Failed: {0}", ex.Message);
			}

			// Display the success or failure message.
			await new MessageDialog(message, "Sign-in result").ShowAsync();

			return success;
		}

		public MainPage()
		{
			new Syncfusion.SfChart.XForms.UWP.SfChartRenderer();

			this.InitializeComponent();

			// Initialize the authenticator before loading the app.
			HomeBudget.App.Init(this);

			LoadApplication(new HomeBudget.App());
		}
	}
}
