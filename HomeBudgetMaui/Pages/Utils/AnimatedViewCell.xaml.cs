using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui;

namespace HomeBudget.Pages.Utils
{
	public partial class AnimatedViewCell : ViewCell
	{
		public AnimatedViewCell ()
		{
			InitializeComponent ();
            layout.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(OnClick)
            });
            layout.IsVisible = false;
		}

        private void OnClick()
        {
            if(BindingContext is SummaryListSubcat subcat)
            {
                MessagingCenter.Send(this, "SubcatClicked", subcat);
            }
        }

        protected override void OnBindingContextChanged()
        {
            if(BindingContext is SummaryListSubcat data)
            {
                data.Expand += () =>
                {
                    layout.IsVisible = true;
                    layout.TranslationX = -300;
                    layout.HeightRequest = 50;
                    layout.Margin = new Thickness(5, 0);
                    ForceUpdateSize();
                    layout.TranslateTo(0, 0, 500, Easing.CubicInOut);
                };
                data.Collapse += () =>
                {
                    layout.IsVisible = false;
                    layout.Margin = new Thickness(5, 0);
                    ForceUpdateSize();
                };
            }
            base.OnBindingContextChanged();
        }
    }
}