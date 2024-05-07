using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui;

namespace HomeBudgeStandard.Pages
{
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
            //version.Text = $"Wersja: {Essentials.VersionTracking.CurrentVersion}({Essentials.VersionTracking.CurrentBuild})";
		}

        private void OpenJOP()
        {
            Launcher.OpenAsync(new Uri("https://jakoszczedzacpieniadze.pl/darmowy-szablon-budzetu-domowego"));
        }

        private void OpenI8()
        {
            Launcher.OpenAsync(new Uri("https://icons8.com"));
        }
    }
}