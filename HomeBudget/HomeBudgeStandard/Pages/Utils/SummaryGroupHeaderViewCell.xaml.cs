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

        private void ExpandCategory()
        {
            if (BindingContext is BudgetSummaryDataViewModel element)
            {
                if (element.IsExpanded)
                    expandIcon.RotateTo(0);
                else
                    expandIcon.RotateTo(90);

                MessagingCenter.Send(this, "CategoryClicked", element);
            }
        }
    }
}