using HomeBudget.Pages.PC;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HomeBudget.Utils
{
    public class ChartCategoryNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string name = "";
            BudgetViewModelData data = (BudgetViewModelData)value;
            if(data.Category.IsIncome)
                name = data.Subcat.Value > 0 ? data.Subcat.Name : "";
            else
                name = data.Category.TotalValues > 0 ? data.Category.Name : "";

            return name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
