using Syncfusion.Data;
using System;
using System.Globalization;
using Microsoft.Maui;
using Syncfusion.Maui.Data;
using Syncfusion.Maui.DataGrid;

namespace HomeBudget.Converters
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