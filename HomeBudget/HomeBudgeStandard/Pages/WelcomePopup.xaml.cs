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
	public partial class WelcomePopup : PopupPage
    {
        public Command LinkCommand { get; private set; }
        public WelcomePopup ()
		{
            LinkCommand = new Command(() => OpenLink());
            InitializeComponent ();
            BindingContext = this;
		}

        private void OpenLink()
        {
            Device.OpenUri(new Uri("https://jakoszczedzacpieniadze.pl/darmowy-szablon-budzetu-domowego"));
        }

        protected override void OnAppearing()
        {
            SwitchToWelcome(null, EventArgs.Empty);
        }

        private void SwitchToHowTo(object sender, EventArgs args)
        {
            welcome.IsVisible = false;
            howTo.IsVisible = true;
            planning.IsVisible = false;
            dropbox.IsVisible = false;
        }

        private void SwitchToWelcome(object sender, EventArgs args)
        {
            welcome.IsVisible = true;
            howTo.IsVisible = false;
            planning.IsVisible = false;
            dropbox.IsVisible = false;
        }

        private void SwitchToPlanning(object sender, EventArgs args)
        {
            welcome.IsVisible = false;
            howTo.IsVisible = false;
            planning.IsVisible = true;
            dropbox.IsVisible = false;
        }

        private void SwitchToSynchro(object sender, EventArgs args)
        {
            welcome.IsVisible = false;
            howTo.IsVisible = false;
            planning.IsVisible = false;
            dropbox.IsVisible = true;
        }

        private async void LetsStart(object sender, EventArgs args)
        {
            await Navigation.PopPopupAsync();
        }
    }
}