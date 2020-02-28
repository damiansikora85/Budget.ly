using HomeBudget.Code;
using HomeBudgetShared.Code.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeBudgeStandard.Providers
{
    public class BudgetTemplateProvider : IBudgetTemplateProvider
    {
        public BudgetDescription GetTemplate()
        {
            return MainBudget.Instance.BudgetDescription;
        }
    }
}
