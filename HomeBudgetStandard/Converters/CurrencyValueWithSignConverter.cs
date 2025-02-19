using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Maui;

namespace HomeBudget.Converters
{
    public class CurrencyValueWithSignConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var cultureInfoPL = new CultureInfo("pl-PL");
            if (value != null)
            {
                var valueDouble = (double)value;
                return string.Format(cultureInfoPL, valueDouble > 0 ? "+{0:c}" : "{0:c}", valueDouble);
            }
            return string.Format(cultureInfoPL, "{0:c}", 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string valueStr)
            {
                var style = NumberStyles.Float | NumberStyles.AllowCurrencySymbol;
                if (double.TryParse(valueStr, style, CultureInfo.InvariantCulture.NumberFormat, out double result))
                    return result;
            }
            return 0.0;
        }
    }
}
