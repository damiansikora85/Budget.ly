using System;
using System.Collections.Generic;
using System.Text;

namespace HomeBudget.Code.Logic
{
    public class BudgetTransaction
    {
        public DateTime Date { get; set; }
        public string Note { get; set; }
        public int CategoryId { get; set; }
        public int SubcatId { get; set; }
        public double Amount { get; set; }
    }
}
