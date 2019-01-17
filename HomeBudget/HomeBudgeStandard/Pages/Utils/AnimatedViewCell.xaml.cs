using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeBudget.Pages.Utils
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
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
                    layout.Margin = new Thickness(8, 1);
                    ForceUpdateSize();
                    layout.TranslateTo(0, 0, 500, Easing.CubicInOut);
                };
                data.Collapse += () =>
                {
                    layout.IsVisible = false;
                    layout.Margin = new Thickness(8, 0);
                    ForceUpdateSize();
                };
            }
            base.OnBindingContextChanged();
        }
    }
}