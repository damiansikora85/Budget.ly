using HomeBudget.Pages.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace HomeBudgeStandard.Utils
{
    public class SummaryListViewTemplateSelector : DataTemplateSelector
    {
        public DataTemplate EmptyCellDataTemplate { get; set; }
        public DataTemplate BudgetCellDataTemplate { get;set; }
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if(item is BudgetSummaryDataViewModel data)
            {
                return data.IsEmpty ? EmptyCellDataTemplate : BudgetCellDataTemplate;
            }
            else
            {
                return EmptyCellDataTemplate;
            }
        }
    }
}
