using HomeBudget.Code;
using HomeBudget.Code.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomeBudgeStandard.Views.ViewModels
{
    public class TransactionViewModel
    {
        private BudgetTransaction _transaction;

        public TransactionViewModel()
        {
        }

        public TransactionViewModel(BudgetTransaction transaction, BudgetDescription budgetDesc)
        {
            _transaction = transaction;
            var category = budgetDesc.Categories.Where(c => c.Id == transaction.CategoryId).FirstOrDefault();
            if (category != null)
            {
                CategoryName = category.Name;
                Icon = category.IconFileName;
                SubcatName = category.subcategories[transaction.SubcatId];
            }
        }

        public DateTime Date => _transaction.Date;
        public double Amount => _transaction.Amount;

        public string Icon { get; set; }
        public string CategoryName { get; set; }
        public string SubcatName { get; set; }

        public bool IsEmpty { get; internal set; }
    }
}
