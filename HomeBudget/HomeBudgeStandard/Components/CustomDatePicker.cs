using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace HomeBudgeStandard.Components
{
    public class CustomDatePicker : DatePicker
    {
        public Action SelectedDateConfirmed { get; set; }
    }
}
