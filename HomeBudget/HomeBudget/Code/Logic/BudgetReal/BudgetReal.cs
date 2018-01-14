using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Code.Logic
{
    public sealed class BudgetReal : BaseBudget.BaseBudget
    {
        public void Setup(List<BudgetCategoryTemplate> categoriesDesc)
        {
            foreach (BudgetCategoryTemplate categoryDesc in categoriesDesc)
            {
                BudgetRealCategory realCategory = BudgetRealCategory.Create(categoryDesc);
                realCategory.PropertyChanged += OnCategoryModified;
                Categories.Add(realCategory);
            }
        }

        public void Deserialize(BinaryData binaryData)
        {
            Categories.Clear();
            int categoriesNum = binaryData.GetInt();
            for (int i = 0; i < categoriesNum; i++)
            {
                BudgetRealCategory category = new BudgetRealCategory();
                category.Deserialize(binaryData);
                category.PropertyChanged += OnCategoryModified;
                Categories.Add(category);
            }
        }

        public void AddExpense(double value, DateTime date, int categoryId, int subcatId)
        {
            BudgetRealCategory category = (BudgetRealCategory)GetBudgetCategory(categoryId);
            category.AddValue(value, date, subcatId);
        }

        public void AddIncome(double value, DateTime date, int incomeCategoryId)
        {
            BudgetRealCategory category = (BudgetRealCategory)GetIncomesCategories()[0];
            category.AddValue(value, date, incomeCategoryId);
        }
    }
}
