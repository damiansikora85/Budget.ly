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
	public partial class AboutPage : ContentPage
	{
        public Command LinkCommand { get; private set; }
		public AboutPage ()
		{
            LinkCommand = new Command(() => OpenLink());
			InitializeComponent ();
            BindingContext = this;
            version.Text = $"Wersja: {Xamarin.Essentials.VersionTracking.CurrentVersion}({Xamarin.Essentials.VersionTracking.CurrentBuild})";
		}

        private void OpenLink()
        {
            Device.OpenUri(new Uri("https://jakoszczedzacpieniadze.pl/darmowy-szablon-budzetu-domowego"));
        }
    }
}