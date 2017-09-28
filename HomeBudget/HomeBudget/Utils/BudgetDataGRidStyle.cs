using Syncfusion.SfDataGrid.XForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HomeBudget.Utils
{
    public class BudgetDataGridStyle : DataGridStyle
    {
        public override Color GetCaptionSummaryRowBackgroundColor()
        {
            return Color.FromHex("D2F3DF"); 
        }

        public override Color GetCaptionSummaryRowForegroundColor()
        {
            return Color.FromHex("232825");
        }

        public override GridLinesVisibility GetGridLinesVisibility()
        {
            return GridLinesVisibility.None;
        }
    }
}
