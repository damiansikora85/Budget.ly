namespace HomeBudget.Pages.Utils
{
	public partial class AnimatedViewCell : ViewCell
	{
        private SummaryListSubcat _previousContext;

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
            // Odłącz eventy z poprzedniego kontekstu
            if (_previousContext != null)
            {
                _previousContext.Expand -= OnExpand;
                _previousContext.Collapse -= OnCollapse;
            }

            base.OnBindingContextChanged();

            // Przypisz nowy kontekst
            if (BindingContext is SummaryListSubcat data)
            {
                _previousContext = data;

                data.Expand += OnExpand;
                data.Collapse += OnCollapse;
            }
        }

        private void OnExpand()
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                layout.IsVisible = true;
                layout.TranslationX = -300;
                layout.HeightRequest = 50;
                layout.Margin = new Thickness(5, 0);
                ForceUpdateSize();
                await layout.TranslateTo(0, 0, 500, Easing.CubicInOut);
            });
        }

        private void OnCollapse()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                layout.IsVisible = false;
                layout.Margin = new Thickness(5, 0);
                ForceUpdateSize();
            });
        }

    }
}