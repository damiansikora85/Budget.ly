using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace HomeBudget.Components
{
    public class GradientView : View
    {
        public static readonly BindableProperty StartColorProperty = BindableProperty.Create(nameof(StartColor), typeof(Color), typeof(GradientView), Color.Red);
        public static readonly BindableProperty EndColorProperty = BindableProperty.Create(nameof(EndColor), typeof(Color), typeof(GradientView), Color.White);

        public Color StartColor
        {
            get => (Color)GetValue(StartColorProperty);
            set => SetValue(StartColorProperty, value);
        }

        public Color EndColor
        {
            get => (Color)GetValue(EndColorProperty);
            set => SetValue(EndColorProperty, value);
        }
    }
}
