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

        internal static TransactionViewModel Create(BudgetTransaction transaction, BudgetCategoryTemplate category)
        {
            var transactionViewModel = new TransactionViewModel();
            transactionViewModel.Setup(transaction, category);
            return transactionViewModel;
        }

        private void Setup(BudgetTransaction transaction, BudgetCategoryTemplate category)
        {
            _transaction = transaction;
            if (category != null)
            {
                CategoryName = category.Name;
                Icon = category.IconFileName;
                SubcatName = category.subcategories[transaction.SubcatId];
                IsIncome = category.IsIncome;
            }
        }

        public DateTime Date => _transaction.Date;
        public double Amount => IsIncome ? _transaction.Amount : _transaction.Amount *-1;

        public string Icon { get; set; }
        public string CategoryName { get; set; }
        public string SubcatName { get; set; }
        public string Note => _transaction.Note;
        public bool IsIncome { get; private set; }

        public bool IsEmpty { get; internal set; }
    }
}
