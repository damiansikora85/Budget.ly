using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace HomeBudget.Code.Logic
{
    [ProtoContract]
    public sealed class BudgetPlanned : BaseBudget.BaseBudget
    {
        public BudgetPlanned() { }

        public BudgetPlanned(BudgetPlanned budgetPlanned)
        {
            foreach (BudgetPlannedCategory category in budgetPlanned.Categories)
            {
                var newCategory = new BudgetPlannedCategory(category);
                newCategory.PropertyChanged += OnCategoryModified;
                Categories.Add(newCategory);
            }
        }

        public void Setup(List<BudgetCategoryTemplate> categoriesDesc)
        {
            Categories.Clear();
            foreach (BudgetCategoryTemplate categoryDesc in categoriesDesc)
            {
                var plannedCategory = BudgetPlannedCategory.Create(categoryDesc);
                plannedCategory.PropertyChanged += OnCategoryModified;
                Categories.Add(plannedCategory);
            }
        }
    }
}
