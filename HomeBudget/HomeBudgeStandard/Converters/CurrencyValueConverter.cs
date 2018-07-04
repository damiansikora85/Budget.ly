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
            return null;
        }
    }
}
