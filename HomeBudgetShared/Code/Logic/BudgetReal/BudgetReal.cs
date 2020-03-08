using ProtoBuf;
using System;
using System.Collections.Generic;

namespace HomeBudget.Code.Logic
{
    [ProtoContract]
    public sealed class BudgetReal : BaseBudget.BaseBudget
    {
        [ProtoMember(1)]
        public List<BudgetTransaction> Transactions;

        public BudgetReal(BudgetReal budgetReal) : base()
        {
            foreach (BudgetRealCategory category in budgetReal.Categories)
            {
                var newCategory = new BudgetRealCategory(category);
                newCategory.PropertyChanged += OnCategoryModified;
                Categories.Add(newCategory);
            }
            Transactions = new List<BudgetTransaction>(budgetReal.Transactions);
        }

        public BudgetReal() : base()
        {
            Transactions = new List<BudgetTransaction>();
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

        public void AddExpense(double value, DateTime date, int categoryId, int subcatId, string note)
        {
            var category = (BudgetRealCategory)GetBudgetCategory(categoryId);
            category.AddValue(value, date, subcatId);
            AddTransaction(value, date, categoryId, subcatId, note);
        }

        public void AddIncome(double value, DateTime date, int incomeCategoryId, string note)
        {
            var category = (BudgetRealCategory)GetIncomesCategories()[0];
            category.AddValue(value, date, incomeCategoryId);
            AddTransaction(value, date, category.Id, incomeCategoryId, note);
        }

        private void AddTransaction(double value, DateTime date, int categoryId, int subcatId, string note)
        {
            Transactions.Add(new BudgetTransaction { Amount = value, CategoryId = categoryId, SubcatId = subcatId, Date = date, Note = note });
        }
    }
}
