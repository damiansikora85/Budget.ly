using Acr.UserDialogs;
using HomeBudget.Code;
using HomeBudget.Code.Logic;
using HomeBudget.Pages.Utils;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HomeBudgeStandard.Views
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

        private DateTime _currentDateTime = DateTime.Now;
        private BudgetMonth _currentBudgetMonth;
        private bool _needRefreshData = true;

        public bool IsBudgetPlanned { get; private set; }
        public bool NoBudgetPlanned => !IsBudgetPlanned;

        private double _scrollProgress = 1.0;

        public double ScrollProgress
        {
            get => _scrollProgress;
            set
            {
                _scrollProgress = value;
                OnPropertyChanged(nameof(ScrollProgress));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName="")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public SummaryViewModel()
        {
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
            }
            OnPropertyChanged();
        }

        private async static Task<ObservableCollection<BudgetSummaryDataViewModel>> GetBudgetSummaryDataAsync(BudgetMonth budgetData) =>
#pragma warning disable CA2008 // Do not create tasks without passing a TaskScheduler
            await Task.Factory.StartNew(() =>
            {
                var budgetSummaryCollection = new ObservableCollection<BudgetSummaryDataViewModel>();
                var categoriesDesc = MainBudget.Instance.BudgetDescription.Categories;

                var budgetReal = budgetData.BudgetReal;
                var budgetPlanned = budgetData.BudgetPlanned;
                AddEmptyElements(budgetSummaryCollection, 2);

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

        public async void AddExpenseAsync(double value, DateTime date, BaseBudgetCategory category, int subcatId)
        {
            var budgetMonth = MainBudget.Instance.GetMonth(date);
            if (category.IsIncome)
            {
                budgetMonth.AddIncome(value, date, subcatId);
            }
            else
            {
                budgetMonth.AddExpense(value, date, category.Id, subcatId);
            }

            await RefreshAsync().ConfigureAwait(false);
        }
#pragma warning restore CA2008 // Do not create tasks without passing a TaskScheduler

        private async void BudgetDataChanged(bool isLoadedFromCloud)
        {
            _currentBudgetMonth = MainBudget.Instance.GetMonth(_currentDateTime);
            await RefreshAsync(true).ConfigureAwait(false);
            Device.BeginInvokeOnMainThread(() => UserDialogs.Instance.HideLoading());
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
    }
}
