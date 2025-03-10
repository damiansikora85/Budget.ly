using System.Collections.ObjectModel;
using System.ComponentModel;
using Controls.UserDialogs.Maui;
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

        public async Task ViewWillAppear()
        {
            MainBudget.Instance.BudgetDataChanged += BudgetDataChanged;
            MainBudget.Instance.BudgetDataChanged -= MarkBudgetChanged;
            if(_needRefreshData && MainBudget.Instance.IsDataLoaded)
            {
                _needRefreshData = false;
                await RefreshAsync(true).ConfigureAwait(false);
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

        public async void DecreaseMonth()
        {
            _currentDateTime = _currentDateTime.AddMonths(-1);
            await RefreshAsync(true).ConfigureAwait(false);
        }

        public async void IncreaseMonth()
        {
            _currentDateTime = _currentDateTime.AddMonths(1);
            await RefreshAsync(true).ConfigureAwait(false);
        }

        public async Task RefreshAsync(bool full = false)
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
                    SummaryListViewItems = await GetBudgetSummaryDataAsync(_currentBudgetMonth).ConfigureAwait(false);
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

        private async static Task<ObservableCollection<BudgetSummaryDataViewModel>> GetBudgetSummaryDataAsync(BudgetMonth budgetData) =>
#pragma warning disable CA2008 // Do not create tasks without passing a TaskScheduler
            await Task.Factory.StartNew(() =>
            {
                var budgetSummaryCollection = new ObservableCollection<BudgetSummaryDataViewModel>();
                var categoriesDesc = MainBudget.Instance.BudgetDescription.Categories;

                var budgetReal = budgetData.BudgetReal;
                var budgetPlanned = budgetData.BudgetPlanned;
                AddEmptyElements(budgetSummaryCollection, 1);

                for (int i = 0; i < budgetReal.Categories.Count; i++)
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
            }).ConfigureAwait(false);

#pragma warning restore CA2008 // Do not create tasks without passing a TaskScheduler

        public async void AddExpenseAsync(double value, DateTime date, BaseBudgetCategory category, int subcatId, string note)
        {
            BudgetUseCases.AddExpense(value, date, category, subcatId, note);

            await RefreshAsync().ConfigureAwait(false);
        }


        private async void BudgetDataChanged(bool isLoadedFromCloud)
        {
            _currentBudgetMonth = MainBudget.Instance.GetMonth(_currentDateTime);
            await RefreshAsync(true).ConfigureAwait(false);
            Device.BeginInvokeOnMainThread(() => UserDialogs.Instance.HideHud());
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

        internal async Task RemoveTransactionAsync(TransactionViewModel transactionViewModel)
        {
            var tvm = TransactionList.First(p => p.Contains(transactionViewModel));
            tvm.Remove(transactionViewModel);
            if(tvm.Count == 0)
            {
                TransactionList.Remove(tvm);
            }
            BudgetUseCases.RemoveTransaction(transactionViewModel.Transaction);
            await RefreshAsync().ConfigureAwait(false);
        }
    }
}
