using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeBudget.Pages.Utils;

namespace HomeBudgetMaui.Messages
{
    public class CategoryClickedMessage
    {
        public CategoryClickedMessage(BudgetSummaryDataViewModel element)
        {
            Element = element;
        }

        public BudgetSummaryDataViewModel Element
        {
            get;
        }
    }
}
