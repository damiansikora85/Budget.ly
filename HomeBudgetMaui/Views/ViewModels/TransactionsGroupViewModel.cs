﻿using System.Collections.ObjectModel;
using Firebase.Crashlytics;
using HomeBudget.Code;
using HomeBudget.Code.Logic;

namespace HomeBudgetStandard.Views.ViewModels
{
    public class TransactionsGroupViewModel : ObservableCollection<TransactionViewModel>
    {
        public TransactionsGroupViewModel()
        {
        }

        public TransactionsGroupViewModel(IGrouping<DateTime, BudgetTransaction> group, BudgetDescription budgetDescription)
        {
            Date = group.Key;
            foreach(var transaction in group)
            {
                try
                {
                    var category = budgetDescription.Categories.FirstOrDefault(c => c.Id == transaction.CategoryId);
                    if (category != null)
                    {
                        Add(TransactionViewModel.Create(transaction, category));
                    }
                }
                catch(Exception exc)
                {
                    FirebaseCrashlytics.Instance.RecordException(Java.Lang.Throwable.FromException(exc));
                }
            }
        }

        public DateTime Date { get; private set; }
        public bool IsEmpty { get; internal set; }
    }
}
