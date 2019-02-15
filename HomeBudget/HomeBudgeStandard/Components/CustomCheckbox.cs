using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace HomeBudget.Components
{
    public class CustomCheckbox : View
    {
        public event EventHandler OnCheckChanged;

        public static BindableProperty IsCheckedProperty = BindableProperty.Create(nameof(IsChecked), typeof(bool), typeof(CustomCheckbox), false, BindingMode.TwoWay);
        public static BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(CustomCheckbox), "");
        public static BindableProperty CheckedCommandProperty = BindableProperty.Create(nameof(CheckedCommand), typeof(ICommand), typeof(CustomCheckbox), null);
        public static BindableProperty CheckedCommandParameterProperty = BindableProperty.Create(nameof(CheckedCommandParameter), typeof(object), typeof(CustomCheckbox), null);


        public bool IsChecked
        {
            get => (bool)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public object CheckedCommandParameter
        {
            get => GetValue(CheckedCommandParameterProperty);         
            set => SetValue(CheckedCommandParameterProperty, value);
        }

        public ICommand CheckedCommand
        {
            get => (ICommand)GetValue(CheckedCommandProperty);
            set => SetValue(CheckedCommandProperty, value);
        }
    }
}
