using HomeBudget.Utils;
using Syncfusion.Data;
using System;
using System.Collections.Generic;

namespace HomeBudget.Converters
{
    public class BudgetCategorySortComparer : IComparer<Object>, ISortDirection
    {
        public ListSortDirection SortDirection { get; set; }

        public int Compare(object x, object y)
        {
            if(x is BudgetViewModelData first && y is BudgetViewModelData second)
            {
                return -1;
            }
            else if(x is Group group1 && y is Group group2)
            {
                return 1;
            }
            return 0;
        }
    }
}
