﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Maui;

namespace HomeBudgetStandard.Converters
{
    public class ToCapitalLetterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is string text)
            {
                return text.Substring(0, 1).ToUpper() + text.Substring(1);
            }
            else
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
