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
        public override Color GetHeaderBackgroundColor()
        {
            return Color.White;
        }

        public override Color GetCaptionSummaryRowBackgroundColor()
        {
            return Color.FromHex("f5f5f5"); 
        }

        public override Color GetCaptionSummaryRowForegroundColor()
        {
            return Color.FromHex("232825");
        }

        public override GridLinesVisibility GetGridLinesVisibility()
        {
            return GridLinesVisibility.Horizontal;
        }

        public override Color GetRecordBackgroundColor()
        {
            return Color.FromHex("f5f5f5");
        }

        public override Color GetAlternatingRowBackgroundColor()
        {
            
            return Color.FromHex("f9f9f9");
        }

        
    }
}
