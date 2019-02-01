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
	public partial class SummaryViewCell : ViewCell
	{
        private double collapsedSize;
        private double expandedSize;

        public SummaryViewCell()
		{
            InitializeComponent ();
        }

        private void ExpandCategory(object sender, EventArgs args)
        {
            collapsedSize = Height;
            grid.Animate("sizeAnim", (val) => grid.HeightRequest = val, collapsedSize, collapsedSize*4);
        }
    }
}