using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace HomeBudget.UWP
{
    public class CurrencyValueConverter : IValueConverter
    {
        private CultureInfo _cultureInfoPL = new CultureInfo("pl-PL");

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value is double valueDouble)
            {
                return string.Format(_cultureInfoPL, "{0:c}", valueDouble);
            }

            return string.Format(_cultureInfoPL, "{0:c}", 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if(value is string valueStr)
            {
                var style = NumberStyles.Float | NumberStyles.AllowCurrencySymbol;
                if (double.TryParse(valueStr, style, CultureInfo.InvariantCulture.NumberFormat, out double result))
                    return result;
            }
            return 0.0;
        }
    }
}
