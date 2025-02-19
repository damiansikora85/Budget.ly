using HomeBudget.Utils;
using Syncfusion.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Maui;
using Syncfusion.Maui.Data;

namespace HomeBudget.Converters
{
    class BudgetGridIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is Group group)
            {
                if(group.Source[0] is BudgetViewModelData budgetData)
                {
                    return budgetData.Category.IconName;
                }
            }
            
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
