using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Maui;
using Microsoft.Maui.Handlers;

namespace HomeBudgetStandard.Components
{
    public class CustomDatePicker : DatePicker, IDatePicker
    {
        //DateTime IDatePicker.Date
        //{
        //    get => Date;
        //    set
        //    {
        //        if (value.Equals(DateTime.Today.Date))
        //            Date = value.AddDays(-1);
        //        Date = value;
        //        OnPropertyChanged(nameof(Date));
        //    }
        //}

        public Action SelectedDateConfirmed { get; set; }
        public void OpenDialog()
        {
#if ANDROID
            var handler = Handler as IDatePickerHandler;
            handler?.PlatformView.PerformClick();
#endif
        }
    }
}
