using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using HomeBudget.Code;
using HomeBudget.Code.Logic;
using HomeBudget.Utils;
using HomeBudgetStandard.Views.ViewModels;

namespace HomeBudgetStandard.Views.ViewModels
{
    public class BudgetPlanViewModel : BaseViewModel
    {
        public ObservableCollection<BudgetViewModelData> Budget
        {
            get; set;
        }
        public bool ExpensesVisible { get; private set; } = true;
        public bool IncomesVisible { get; private set; } = false;
        public TextDecorations ExpensesChartTextDecorations { get; private set; } = TextDecorations.Underline;
        public TextDecorations IncomesChartTextDecorations { get; private set; } = TextDecorations.None;

        public ICommand ExpensesChartCommand { get; private set; }
        public ICommand IncomesChartCommand { get; private set; }
        public ICommand PrevMonthCommand { get; private set; }
        public ICommand NextMonthCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }

       
        public List<ChartData> ExpensesChartData
        {
            get; private set;
        }
        public List<ChartData> IncomesChartData
        {
            get; private set;
        }

        private DateTime _currentMonth;

        private readonly CultureInfo _cultureInfoPL = new CultureInfo("pl-PL");
        public string Date
        {
            get => _currentMonth.ToString("MMMM yyyy", _cultureInfoPL);
        }

        public BudgetPlanViewModel()
        {
            _currentMonth = DateTime.Now;
            ExpensesChartCommand = new Command(ForceSwitchChart);
            IncomesChartCommand = new Command(ForceSwitchChart);
            PrevMonthCommand = new AsyncRelayCommand(OnPrevMonth);
            NextMonthCommand = new AsyncRelayCommand(OnNextMonth);
            SaveCommand = new AsyncRelayCommand(OnSave);
            Budget = new ObservableCollection<BudgetViewModelData>();
        }

        public void Setup()
        {
            _currentMonth = DateTime.Now;
            SetupDataGrid(DateTime.Now);
            UpdateCharts();
        }

        public void ForceSwitchChart()
        {
            ExpensesVisible = !ExpensesVisible;
            IncomesVisible = !IncomesVisible;
            OnPropertyChanged(nameof(ExpensesVisible));
            OnPropertyChanged(nameof(IncomesVisible));

            ExpensesChartTextDecorations = ExpensesChartTextDecorations == TextDecorations.None ? TextDecorations.Underline : TextDecorations.None;
            IncomesChartTextDecorations = IncomesChartTextDecorations == TextDecorations.None ? TextDecorations.Underline : TextDecorations.None;
            OnPropertyChanged(nameof(ExpensesChartTextDecorations));
            OnPropertyChanged(nameof(IncomesChartTextDecorations));
        }

        public void UpdateCharts()
        {
            var budgetPlanned = MainBudget.Instance.GetMonth(_currentMonth).BudgetPlanned;

            IncomesChartData = new List<ChartData>();
            var incomesCategories = budgetPlanned.GetIncomesCategories();
            var totalIncome = incomesCategories.Sum(el => el.TotalValues);
            foreach (var category in incomesCategories.Cast<BudgetPlannedCategory>())
            {
                foreach (var subcat in category.subcats)
                {
                    if (subcat.Value > 0)
                    {
                        IncomesChartData.Add(new ChartData { Label = subcat.Name, Value = subcat.Value, Percentage = subcat.Value / totalIncome });
                    }
                }

            }
            ExpensesChartData = new List<ChartData>();
            var totalExpense = budgetPlanned.Categories.Sum(el => el.TotalValues) - totalIncome;
            foreach (var category in budgetPlanned.Categories.Cast<BudgetPlannedCategory>())
            {
                if (!category.IsIncome && category.TotalValues > 0)
                {
                    ExpensesChartData.Add(new ChartData { Label = category.Name, Value = category.TotalValues, Percentage = category.TotalValues / totalExpense });
                }
            }

            OnPropertyChanged(nameof(IncomesChartData));
            OnPropertyChanged(nameof(ExpensesChartData));
        }

        private async Task OnSave()
        {
            await MainBudget.Instance.UpdateMainPlannedBudget(_currentMonth);
            //UserDialogs.Instance.Toast("Ten plan budżetu będzie używany w kolejnych miesiącach");
        }

        private async Task OnNextMonth()
        {
            _currentMonth = _currentMonth.AddMonths(1);
            await RefreshAsync();
        }

        private async Task OnPrevMonth()
        {
            _currentMonth = _currentMonth.AddMonths(-1);
            await RefreshAsync();
        }

        private async Task RefreshAsync()
        {
            OnPropertyChanged(nameof(Date));
            //await SetupDataGrid(_currentMonth);
            UpdateCharts();
        }

        private void SetupDataGrid(DateTime date)
        {
            var budget = new ObservableCollection<BudgetViewModelData>();
            try
            {
                var budgetPlanned = MainBudget.Instance.GetMonth(date).BudgetPlanned;

                foreach (var category in budgetPlanned.Categories)
                {
                    foreach (var subcat in category.subcats)
                    {
                        var model = new BudgetViewModelData
                        {
                            Category = category,
                            Subcat = subcat,
                            SubcatPlanned = subcat as PlannedSubcat,
                        };
                        budget.Add(model);
                    }
                }
            }
            catch (Exception e)
            {
                var msg = e.Message;
                return;
            }

            Budget = budget;
            OnPropertyChanged(nameof(Budget));

            Device.BeginInvokeOnMainThread(() =>
            {
                //_previousMonthButton.IsEnabled = MainBudget.Instance.HasMonthData(_currentMonth.AddMonths(-1));
                //_nextMonthButton.IsEnabled = MainBudget.Instance.HasMonthData(_currentMonth.AddMonths(1));
            });
        }
    }
}
