using System;
using System.Collections.Generic;
using System.Text;

namespace HomeBudget.Pages.Utils
{
    public class SummaryListSubcat
    {
        public double SpendPercentage { get; set; }
        public int SpendPercentageInt
        {
            get
            {
                return (int)(SpendPercentage * 100);
            }
        }
        public string Name { get; set; }
        public double AmountReal { get; set; }
        public double AmountPlanned { get; set; }
        public int Id { get; set; }
        public string Icon { get; set; }

        public Action Expand;
        public Action Collapse;
    }
}
