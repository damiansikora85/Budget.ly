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
	public partial class SummaryGroupHeaderViewCell : ViewCell
	{
		public SummaryGroupHeaderViewCell ()
		{
			InitializeComponent ();
            layout.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(ExpandCategory)
            });
        }

        protected override void OnBindingContextChanged()
        {
            if (BindingContext is BudgetSummaryDataViewModel element)
            {
                element.PropertyChanged += (sender, args) =>
                {
                    if(args.PropertyName == nameof(BudgetSummaryDataViewModel.IsExpanded))
                    {
                        if(element.IsExpanded)
                            expandIcon.RotateTo(90);
                        else
                            expandIcon.RotateTo(0);
                    }
                };
            }

            base.OnBindingContextChanged();
        }

        private void ExpandCategory()
        {
            if (BindingContext is BudgetSummaryDataViewModel element)
            {
                if (element.IsExpanding) return;

                MessagingCenter.Send(this, "CategoryClicked", element);
            }
        }
    }
}