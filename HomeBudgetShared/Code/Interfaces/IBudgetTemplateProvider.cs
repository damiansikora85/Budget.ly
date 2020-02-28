using HomeBudget.Code;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeBudgetShared.Code.Interfaces
{
    public interface IBudgetTemplateProvider
    {
        BudgetDescription GetTemplate();
    }
}
