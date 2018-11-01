using Plugin.LocalNotifications;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeBudgeStandard.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage ()
		{
			InitializeComponent ();
            BindingContext = this;
		}

        private void Button_Clicked(object sender, EventArgs e)
        {
            CrossLocalNotifications.Current.Show("Budget.ly", "Budget reminder", 0, DateTime.Now.AddSeconds(30));
        }
    }
}