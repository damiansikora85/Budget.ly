using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudgetMaui.Extensions
{
    public static class ProgressBarExtensions
    {
        public static readonly BindableProperty UseGradientProperty =
            BindableProperty.CreateAttached(
                "UseGradient",
                typeof(bool),
                typeof(ProgressBarExtensions),
                false);

        public static bool GetUseGradient(BindableObject view)
            => (bool)view.GetValue(UseGradientProperty);

        public static void SetUseGradient(BindableObject view, bool value)
            => view.SetValue(UseGradientProperty, value);
    }
}
