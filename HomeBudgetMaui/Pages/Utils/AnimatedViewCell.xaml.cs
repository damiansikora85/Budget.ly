using CommunityToolkit.Mvvm.Messaging;
using HomeBudgetMaui.Messages;

namespace HomeBudget.Pages.Utils
{
	public partial class AnimatedViewCell : Grid
	{
        private SummaryListSubcat _previousContext;

        public AnimatedViewCell ()
		{
			InitializeComponent ();
            this.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(OnClick)
            });
            //this.IsVisible = false;
            this.BindingContextChanged += (s, e) => OnContextChanged();  
        }

        private void OnClick()
        {
            if(BindingContext is SummaryListSubcat subcat)
            {
                WeakReferenceMessenger.Default.Send(new SubcatClickedMessage(subcat));
            }
        }

        private void OnContextChanged()
        {
            // Odłącz eventy z poprzedniego kontekstu
            if (_previousContext != null)
            {
                _previousContext.Expand -= OnExpand;
                _previousContext.Collapse -= OnCollapse;
            }

            // Przypisz nowy kontekst
            if (BindingContext is SummaryListSubcat data)
            {
                _previousContext = data;

                data.Expand += OnExpand;
                data.Collapse += OnCollapse;
                data.MarkAddedToList();
            }
        }

        private void OnExpand()
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                IsVisible = true;
                Opacity = 1;
                HeightRequest = 50;
                Margin = new Thickness(5, 0);
                TranslationX = -500;
                //this.ForceUpdateSize();
                await this.TranslateTo(0, 0, 500, Easing.CubicInOut);
            });
        }

        private void OnCollapse()
        {
            IsVisible = false;
            HeightRequest = 0;
            Opacity = 0;
            Margin = new Thickness(5, 0);
        }
    }
}