using System.Collections.ObjectModel;
using System.ComponentModel;
using Acr.UserDialogs;
using Firebase.Crashlytics;
using HomeBudget.Code;
using HomeBudget.Code.Logic;
using HomeBudget.Pages.Utils;
using HomeBudget.UseCases;
using HomeBudgetShared.Code.Interfaces;
using HomeBudgetStandard.Providers;
using HomeBudgetStandard.Views.ViewModels;

namespace HomeBudgetStandard.Views
{
    public class SummaryViewModel : INotifyPropertyChanged
    {
        public string Date => _currentDateTime.ToString("MMMM, yyyy");
        public double RemainedMoney { get; private set; }
        public double Expenses { get; private set; }
        public double ExpensesProgress { get; private set; }
        public double Incomes { get; private set; }
        public double IncomesProgress { get; private set; }

        public ObservableCollection<BudgetSummaryDataViewModel> SummaryListViewItems { get; private set; }
        public ObservableRangeCollection<TransactionsGroupViewModel> TransactionList { get; private set; }

        private DateTime _currentDateTime = DateTime.Now;
        private BudgetMonth _currentBudgetMonth;
        private bool _needRefreshData = true;
        private IBudgetTemplateProvider _budgetTemplateProvider;
        private readonly object _refreshListDataLock = new object();

        public bool IsBudgetPlanned { get; private set; }
        public bool NoBudgetPlanned => !IsBudgetPlanned;

        private double _headerScrollProgress = 1.0;

        public double HeaderScrollProgress
        {
            get => _headerScrollProgress;
            set
            {
                _headerScrollProgress = value;
                OnPropertyChanged(nameof(HeaderScrollProgress));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName="")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public SummaryViewModel()
        {
            _budgetTemplateProvider = new BudgetTemplateProvider();
            if (MainBudget.Instance.IsDataLoaded)
            {
                _currentBudgetMonth = MainBudget.Instance.GetMonth(_currentDateTime);
            }
        }

        public void ViewWillAppear()
        {
            MainBudget.Instance.BudgetDataChanged += BudgetDataChanged;
            MainBudget.Instance.BudgetDataChanged -= MarkBudgetChanged;
            if(_needRefreshData && MainBudget.Instance.IsDataLoaded)
            {
                _needRefreshData = false;
                RefreshAsync(true);
            }
            else if(_needRefreshData)
            {
                UserDialogs.Instance.ShowLoading("");
            }
        }

        public void ViewWillDisapear()
        {
            MainBudget.Instance.BudgetDataChanged -= BudgetDataChanged;
            MainBudget.Instance.BudgetDataChanged += MarkBudgetChanged;
            _needRefreshData = true;
        }

        public void DecreaseMonth()
        {
            _currentDateTime = _currentDateTime.AddMonths(-1);
            RefreshAsync(true);
        }

        public void IncreaseMonth()
        {
            _currentDateTime = _currentDateTime.AddMonths(1);
            RefreshAsync(true);
        }

        public void RefreshAsync(bool full = false)
        {
            _currentBudgetMonth = MainBudget.Instance.GetMonth(_currentDateTime);
            if (_currentBudgetMonth != null)
            {
                Expenses = _currentBudgetMonth.GetTotalExpenseReal();
                Incomes = _currentBudgetMonth.GetTotalIncomeReal();
                RemainedMoney = Incomes - Expenses;

                var expectedExpenses = _currentBudgetMonth.GetTotalExpensesPlanned();
                var expectedIncomes = _currentBudgetMonth.GetTotalIncomePlanned();
                ExpensesProgress = Expenses / expectedExpenses;
                IncomesProgress = Incomes / expectedIncomes;

                IsBudgetPlanned = expectedExpenses != 0 && expectedIncomes != 0;

                if (full)
                {
                    lock (_refreshListDataLock)
                    {
                        SummaryListViewItems = GetBudgetSummaryDataAsync(_currentBudgetMonth);
                    }
                }

                CreateTransactionsList();
            }
            OnPropertyChanged();
        }

        private void CreateTransactionsList()
        {
            var groupped = _currentBudgetMonth.BudgetReal.Transactions.GroupBy(t => t.Date);
            TransactionList = new ObservableRangeCollection<TransactionsGroupViewModel>
            {
                new TransactionsGroupViewModel { IsEmpty = true },
            };
            var budgetDesc = _budgetTemplateProvider.GetTemplate();
            var tempList = new List<TransactionsGroupViewModel>();
            foreach (var group in groupped)
            {
                try
                {
                    tempList.Add(new TransactionsGroupViewModel(group, budgetDesc));
                }
                catch(Exception exc)
                {
                    FirebaseCrashlytics.Instance.RecordException(Java.Lang.Throwable.FromException(exc));
                }
            }
            TransactionList.AddRange(tempList.OrderByDescending(el => el.Date));
        }

        private static ObservableCollection<BudgetSummaryDataViewModel> GetBudgetSummaryDataAsync(BudgetMonth budgetData)
        {
            var budgetSummaryCollection = new ObservableCollection<BudgetSummaryDataViewModel>();
            var categoriesDesc = MainBudget.Instance.BudgetDescription.Categories;

            var budgetReal = budgetData.BudgetReal;
            var budgetPlanned = budgetData.BudgetPlanned;
            AddEmptyElements(budgetSummaryCollection, 1);

            for (var i = 0; i < budgetReal.Categories.Count; i++)
            {
                var budgetSummaryData = new BudgetSummaryDataViewModel
                {
                    CategoryReal = budgetReal.Categories[i],
                    CategoryPlanned = budgetPlanned.Categories[i],
                    IconFile = categoriesDesc[i].IconFileName,
                    IsEmpty = false
                };

                budgetSummaryData.Init();
                budgetSummaryCollection.Add(budgetSummaryData);
            }

            return budgetSummaryCollection;
        }

        public void AddExpenseAsync(double value, DateTime date, BaseBudgetCategory category, int subcatId, string note)
        {
            BudgetUseCases.AddExpense(value, date, category, subcatId, note);

            RefreshAsync();
        }


        private void BudgetDataChanged(bool isLoadedFromCloud)
        {
            _currentBudgetMonth = MainBudget.Instance.GetMonth(_currentDateTime);
            RefreshAsync(true);
            MainThread.BeginInvokeOnMainThread(() => UserDialogs.Instance.HideLoading());
        }

        private void MarkBudgetChanged(bool arg)
        {
            _needRefreshData = true;
        }

        private static void AddEmptyElements(ObservableCollection<BudgetSummaryDataViewModel> budgetSummaryCollection, int itemsNum)
        {
            for (int i = 0; i < itemsNum; i++)
            {
                var emptyElem = new BudgetSummaryDataViewModel();
                emptyElem.Init();
                emptyElem.IsEmpty = true;
                budgetSummaryCollection.Add(emptyElem);
            }
        }

        internal void RemoveTransactionAsync(TransactionViewModel transactionViewModel)
        {
            var tvm = TransactionList.First(p => p.Contains(transactionViewModel));
            tvm.Remove(transactionViewModel);
            if(tvm.Count == 0)
            {
                TransactionList.Remove(tvm);
            }
            BudgetUseCases.RemoveTransaction(transactionViewModel.Transaction);
            RefreshAsync();
        }
    }
}
