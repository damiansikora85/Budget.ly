using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using HomeBudgetMaui.Messages;
using Microsoft.Maui;

namespace HomeBudget.Pages.Utils
{
	public partial class SummaryGroupHeaderViewCell : Grid
	{
		public SummaryGroupHeaderViewCell ()
		{
			InitializeComponent ();
            this.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(ExpandCategory)
            });
            this.BindingContextChanged += (s, e) => OnContextChanged();
        }

        private void OnContextChanged()
        {
            if (BindingContext is BudgetSummaryDataViewModel element)
            {
                element.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == nameof(BudgetSummaryDataViewModel.IsExpanded))
                    {
                        if (element.IsExpanded)
                        {
                            expandIcon.RotateTo(90);
                        }
                        else
                        {
                            expandIcon.RotateTo(0);
                        }
                    }
                };
            }
        }

        private void ExpandCategory()
        {
            if (BindingContext is BudgetSummaryDataViewModel element)
            {
                if (element.IsExpanding) return;

                WeakReferenceMessenger.Default.Send(new CategoryClickedMessage(element));
            }
        }
    }
}