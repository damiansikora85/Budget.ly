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
        public Command LinkJOP { get; private set; }
        public Command LinkI8 { get; private set; }
        public AboutPage ()
		{
            LinkJOP = new Command(() => OpenJOP());
            LinkJOP = new Command(() => OpenI8());
            InitializeComponent ();
            BindingContext = this;
            version.Text = $"Wersja: {Xamarin.Essentials.VersionTracking.CurrentVersion}({Xamarin.Essentials.VersionTracking.CurrentBuild})";
		}

        private void OpenJOP()
        {
            Device.OpenUri(new Uri("https://jakoszczedzacpieniadze.pl/darmowy-szablon-budzetu-domowego"));
        }

        private void OpenI8()
        {
            Device.OpenUri(new Uri("https://icons8.com"));
        }
    }
}