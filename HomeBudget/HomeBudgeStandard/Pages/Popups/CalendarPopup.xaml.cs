using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

//using Rg.Plugins.Popup.Extensions;
//using Rg.Plugins.Popup.Pages;
using Syncfusion.SfCalendar.XForms;

namespace HomeBudget
{
    public partial class CalendarPopup : ContentPage
    {
        Action<DateTime> OnDateChanged;

        public CalendarPopup()
        {
            InitializeComponent();
            calendar.OnCalendarTapped += OnDaySelected;
        }

        public CalendarPopup(Action<DateTime> callback)
        {
            InitializeComponent();
            calendar.OnCalendarTapped += OnDaySelected;
            OnDateChanged = callback;
        }

        private async void OnDaySelected(object sender, CalendarTappedEventArgs args)
        {
            OnDateChanged(args.datetime);
            await Navigation.PopModalAsync();
        }

        private async void OnBack(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}
