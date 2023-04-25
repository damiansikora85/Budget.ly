using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using HomeBudget.Code;
using HomeBudget.Code.Logic;
using HomeBudget.Utils;
using MvvmHelpers;
using Xamarin.Forms;

namespace HomeBudgeStandard.Views.ViewModels
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

        //private List<ChartData> _expensesChartData;
        //private List<ChartData> _incomesChartData;
        public List<ChartData> ExpensesChartData
        {
            get; private set;
        }
        public List<ChartData> IncomesChartData
        {
            get; private set;
        }

        public BudgetPlanViewModel()
        {
            ExpensesChartCommand = new MvvmHelpers.Commands.Command(ForceSwitchChart);
            IncomesChartCommand = new MvvmHelpers.Commands.Command(ForceSwitchChart);
            Budget = new ObservableCollection<BudgetViewModelData>();
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

        internal void UpdateCharts(DateTime date)
        {
            var budgetPlanned = MainBudget.Instance.GetMonth(date).BudgetPlanned;

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
            //_chartExpense.SetData(chartDataExpenses);
            //_chartIncome.SetData(chartDataIncome);
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

        public void Setup()
        {
            SetupDataGrid(DateTime.Now);
        }
    }
}
