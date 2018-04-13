using System;
using System.Globalization;
using Xamarin.Forms;

namespace HomeBudget.Utils
{
    public class CurrencyValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double valueDouble = (double)value;

            CultureInfo cultureInfoPL = new CultureInfo("pl-PL");
            return valueDouble > 0 ? string.Format(cultureInfoPL, "{0:c}", valueDouble) : "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return null;
        }
    }
}
