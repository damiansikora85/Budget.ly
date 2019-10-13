using ProtoBuf;
using System;
using System.Collections.Generic;

namespace HomeBudget.Code.Logic
{
    [ProtoContract]
    public sealed class BudgetReal : BaseBudget.BaseBudget
    {
        public BudgetReal(BudgetReal budgetReal) : base()
        {
            foreach (BudgetRealCategory category in budgetReal.Categories)
            {
                var newCategory = new BudgetRealCategory(category);
                newCategory.PropertyChanged += OnCategoryModified;
                Categories.Add(newCategory);
            }
        }

        public BudgetReal() : base()
        {
        }

        public void Setup(List<BudgetCategoryTemplate> categoriesDesc)
        {
            foreach (BudgetCategoryTemplate categoryDesc in categoriesDesc)
            {
                var realCategory = BudgetRealCategory.Create(categoryDesc);
                realCategory.PropertyChanged += OnCategoryModified;
                Categories.Add(realCategory);
            }
        }

        public void AddExpense(double value, DateTime date, int categoryId, int subcatId)
        {
            var category = (BudgetRealCategory)GetBudgetCategory(categoryId);
            category.AddValue(value, date, subcatId);
        }

        public void AddIncome(double value, DateTime date, int incomeCategoryId)
        {
            var category = (BudgetRealCategory)GetIncomesCategories()[0];
            category.AddValue(value, date, incomeCategoryId);
        }
    }
}
