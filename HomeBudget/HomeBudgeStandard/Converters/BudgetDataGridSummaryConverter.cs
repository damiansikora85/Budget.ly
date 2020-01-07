using Syncfusion.Data;
using Syncfusion.SfDataGrid.XForms;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace HomeBudgeStandard.Views
{
    public class BudgetDataGridSummaryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var data = value != null ? value as Group : null;
            if (data != null && parameter is SfDataGrid dataGrid && value is Group group && dataGrid.View != null)
            {
                var summaryText = "";
                try
                {
                    summaryText = SummaryCreator.GetSummaryDisplayTextForRow(group.SummaryDetails, dataGrid.View);
                }
                catch (NullReferenceException)
                {

                }
                return summaryText;
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}