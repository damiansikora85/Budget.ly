using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Maui;

namespace HomeBudgetStandard.Components
{
    public class CustomDatePicker : DatePicker
    {
        public Action SelectedDateConfirmed { get; set; }
    }
}
