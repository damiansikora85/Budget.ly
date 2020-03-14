using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HomeBudget.Code.Logic
{
    [ProtoContract]
    public sealed class BudgetReal : BaseBudget.BaseBudget
    {
        //[ProtoMember(1)]
        public List<BudgetTransaction> Transactions;

        public BudgetReal(BudgetReal budgetReal) : base()
        {
            foreach (BudgetRealCategory category in budgetReal.Categories)
            {
                var newCategory = new BudgetRealCategory(category);
                newCategory.PropertyChanged += OnCategoryModified;
                Categories.Add(newCategory);
            }
            Transactions = new List<BudgetTransaction>(budgetReal.Transactions.Select(t => t.Clone()));
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

        public BudgetReal Clone()
        {
            var clone = new BudgetReal();
            foreach (BudgetRealCategory category in Categories)
            {
                var newCategory = new BudgetRealCategory(category);
                clone.Categories.Add(newCategory);
            }
            clone.Transactions = new List<BudgetTransaction>(Transactions.Select(t => t.Clone()));
            return clone;
        }

        public void AddExpense(double value, DateTime date, int categoryId, int subcatId)
        {
            var category = (BudgetRealCategory)GetBudgetCategory(categoryId);
            category.AddValue(value, date, subcatId);
            AddTransaction(value, date, categoryId, subcatId, "");
        }

        public void AddIncome(double value, DateTime date, int incomeCategoryId)
        {
            var category = (BudgetRealCategory)GetIncomesCategories()[0];
            category.AddValue(value, date, incomeCategoryId);
            AddTransaction(value, date, category.Id, incomeCategoryId, "");
        }

        private void AddTransaction(double value, DateTime date, int categoryId, int subcatId, string note)
        {
            Transactions.Add(new BudgetTransaction { Amount = value, CategoryId = categoryId, SubcatId = subcatId, Date = date, Note = note });
        }
    }
}
