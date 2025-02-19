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
                /*if (first.Category.Id != second.Category.Id)
                    return second.Category.Id - first.Category.Id;
                else
                    return second.Subcat.Id - first.Subcat.Id;*/
                return second.Category.Id - first.Category.Id;
            }
            else if(x is Group group1 && y is Group group2)
            {
                return 1;
            }
            return 0;
        }
    }
}
