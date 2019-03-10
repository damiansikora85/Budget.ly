using HomeBudget;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeBudgeStandard.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RatePage : PopupPage
    {
		public RatePage ()
		{
			InitializeComponent ();
		}

        private async void OnLaterClick(object sender, EventArgs args)
        {
            Xamarin.Essentials.Preferences.Set("shouldShowRatePopup", true);
            await Navigation.PopPopupAsync();
        }

        private async void OnNotShowAgainClick(object sender, EventArgs args)
        {
            Xamarin.Essentials.Preferences.Set("shouldShowRatePopup", false);
            await Navigation.PopPopupAsync();
        }

        private async void OnRateNowClick(object sender, EventArgs args)
        {
            Xamarin.Essentials.Preferences.Set("shouldShowRatePopup", false);
            await Navigation.PopPopupAsync();
            Device.OpenUri(new Uri("market://details?id=com.darktower.homebudget"));
        }
    }
}