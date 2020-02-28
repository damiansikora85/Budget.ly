using HomeBudget.Code;
using HomeBudget.Code.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomeBudgeStandard.Views.ViewModels
{
    public class TransactionsGroupViewModel : List<TransactionViewModel>
    {
        public TransactionsGroupViewModel()
        {
        }

        public TransactionsGroupViewModel(IGrouping<DateTime, BudgetTransaction> group, BudgetDescription budgetDescription)
        {
            Date = group.Key;
            foreach(var transaction in group)
            {
                Add(new TransactionViewModel(transaction, budgetDescription));
            }
        }

        public DateTime Date { get; private set; }
        public bool IsEmpty { get; internal set; }
    }
}
