using CommunityToolkit.Maui.Views;
using Microsoft.AppCenter.Analytics;

namespace HomeBudgetStandard.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RatePage : Popup
    {
		public RatePage ()
		{
			InitializeComponent ();
		}

        private async void OnLaterClick(object sender, EventArgs args)
        {
            Analytics.TrackEvent("RateLater", new Dictionary<string, string> { { "donotShowAgain", donotShowAgain.IsChecked ? "true" : "false" } });
            await CloseAsync();
        }

        private async void OnNotShowAgainClick(object sender, EventArgs args)
        {
            Analytics.TrackEvent("DisableRate");
            Preferences.Set("shouldShowRatePopup", false);
            await CloseAsync();
        }

        private async void OnRateNowClick(object sender, EventArgs args)
        {
            Analytics.TrackEvent("RateNow", new Dictionary<string, string> { { "donotShowAgain", donotShowAgain.IsChecked ? "true" : "false" } });
            await CloseAsync();
            await Launcher.OpenAsync(new Uri("market://details?id=com.darktower.homebudget"));
        }

        private async void DontLikeClicked(object sender, EventArgs e)
        {
            Analytics.TrackEvent("DontLikeClicked", new Dictionary<string, string> { { "donotShowAgain", donotShowAgain.IsChecked ? "true" : "false" } });
            Preferences.Set("shouldShowRatePopup", !donotShowAgain.IsChecked);
            await CloseAsync();
        }

        private void LikeClicked(object sender, EventArgs e)
        {
            Analytics.TrackEvent("LikeClicked", new Dictionary<string, string> { { "donotShowAgain", donotShowAgain.IsChecked ? "true" : "false" } });
            Preferences.Set("shouldShowRatePopup", !donotShowAgain.IsChecked);
            first.IsVisible = false;
            second.IsVisible = true;
        }
    }
}