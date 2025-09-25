using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeBudget.Pages.Utils;

namespace HomeBudgetMaui.Messages
{
    public class SubcatClickedMessage
    {
        public SubcatClickedMessage(SummaryListSubcat subcat)
        {
            Subcat = subcat;
        }

        public SummaryListSubcat Subcat
        {
            get;
        }
           
     }
}
