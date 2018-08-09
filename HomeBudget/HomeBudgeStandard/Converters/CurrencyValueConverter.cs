using System;
using System.Globalization;
using Xamarin.Forms;

namespace HomeBudget.Converters
{
    public class CurrencyValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var valueDouble = (double)value;

            var cultureInfoPL = new CultureInfo("pl-PL");
            return string.Format(cultureInfoPL, "{0:c}", valueDouble);
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
