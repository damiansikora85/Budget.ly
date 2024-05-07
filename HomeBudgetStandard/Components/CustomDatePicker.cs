using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Maui;

namespace HomeBudgeStandard.Components
{
    public class CustomDatePicker : DatePicker
    {
        public Action SelectedDateConfirmed { get; set; }
    }
}
