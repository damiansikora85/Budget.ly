using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Code.Logic
{
    public sealed class BudgetPlanned : BaseBudget.BaseBudget
    {
        public BudgetPlanned() { }

        public BudgetPlanned(BudgetPlanned budgetPlanned)
        {
            foreach (BudgetPlannedCategory category in budgetPlanned.Categories)
            {
                BudgetPlannedCategory newCategory = new BudgetPlannedCategory(category);
                newCategory.PropertyChanged += OnCategoryModified;
                Categories.Add(newCategory);
            }
        }

        public void Setup(List<BudgetCategoryTemplate> categoriesDesc)
        {
            foreach (BudgetCategoryTemplate categoryDesc in categoriesDesc)
            {
                BudgetPlannedCategory plannedCategory = BudgetPlannedCategory.Create(categoryDesc);
                plannedCategory.PropertyChanged += OnCategoryModified;
                Categories.Add(plannedCategory);
            }
        }

        public void Deserialize(BinaryData binaryData)
        {
            Categories.Clear();
            int categoriesNum = binaryData.GetInt();
            for(int i=0; i<categoriesNum; i++)
            {
                BudgetPlannedCategory category = new BudgetPlannedCategory();
                category.Deserialize(binaryData);
                category.PropertyChanged += OnCategoryModified;
                Categories.Add(category);
            }
        }
    }
}
