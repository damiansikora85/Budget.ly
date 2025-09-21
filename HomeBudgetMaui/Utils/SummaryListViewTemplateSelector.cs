using HomeBudget.Pages.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Maui;

namespace HomeBudgetStandard.Utils
{
    public class SummaryListViewTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if(item is BudgetSummaryDataViewModel data)
            {
                return data.IsEmpty ? new DataTemplate(()=> new BoxView
                {
                        HeightRequest = 270
                }) 
                : new DataTemplate(() => new SummaryGroupHeaderViewCell());
            }
            else
            {
                return new DataTemplate(() => new BoxView
                {
                        HeightRequest = 270
                });
            }
        }
    }
}
