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
                newCategory.PropertyChanged += OnBudgetPlannedChanged;
                Categories.Add(newCategory);
            }
        }

        public void Setup(List<BudgetCategoryTemplate> categoriesDesc)
        {
            foreach (BudgetCategoryTemplate categoryDesc in categoriesDesc)
            {
                BudgetPlannedCategory plannedCategory = BudgetPlannedCategory.Create(categoryDesc);
                plannedCategory.PropertyChanged += OnBudgetPlannedChanged;
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
                category.PropertyChanged += OnBudgetPlannedChanged;
                Categories.Add(category);
            }
        }

        private void OnBudgetPlannedChanged(object sender, PropertyChangedEventArgs e)
        {
            /*if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("budgetPlanned"));*/
        }
    }
}
