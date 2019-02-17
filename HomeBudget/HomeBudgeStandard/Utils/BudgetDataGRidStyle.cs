using Syncfusion.Data;
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
            return Color.White;
        }

        public override Color GetCaptionSummaryRowBackgroundColor(Group group)
        {
            return Color.White;
        }

        public override ImageSource GetGroupCollapseIcon()
        {
            return base.GetGroupCollapseIcon();//ImageSource.FromFile("forward.png");
        }

        public override ImageSource GetGroupExpanderIcon()
        {
            return ImageSource.FromFile("forward.png");
        }

        public override Color GetHeaderBackgroundColor()
        {
            return Color.White;
        }
    }
}
