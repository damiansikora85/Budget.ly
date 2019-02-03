using HomeBudget.Code.Logic;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeBudget.Pages.Utils
{
    public class SummaryListSubcat
    {
        public double SpendPercentage => SubcatPlan.Value > 0 ? Math.Min((SubcatReal.Value / SubcatPlan.Value), 1) : 0;
        public int SpendPercentageInt
        {
            get
            {
                return (int)(SpendPercentage * 100);
            }
        }
        public string Name { get; set; }
        public RealSubcat SubcatReal { get; set; }
        public PlannedSubcat SubcatPlan { get; set; }
        public int Id { get; set; }
        public string Icon { get; set; }

        public Action Expand;
        public Action Collapse;
    }
}
