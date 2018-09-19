using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace HomeBudget.UWP.Converters
{
    public class ValueLabelColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is double valueDouble)
                return valueDouble > 0 ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.Gray);

            return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
