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
        public double Amount { get; set; }
        public int Id { get; set; }
    }
}
