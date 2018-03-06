using Syncfusion.Data;
using Syncfusion.SfDataGrid.XForms;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace HomeBudget.Utils
{
    public class GroupCaptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var data = value != null ? value as Group : null;
            if (data != null)
            {
                SfDataGrid dataGrid = (SfDataGrid)parameter;
                var summaryText = SummaryCreator.GetSummaryDisplayTextForRow((value as Group).SummaryDetails, dataGrid.View);

                return summaryText;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
