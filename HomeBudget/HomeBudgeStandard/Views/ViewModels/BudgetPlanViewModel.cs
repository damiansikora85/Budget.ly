using HomeBudget.Code;
using HomeBudget.Code.Logic;
using HomeBudget.Utils;
using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace HomeBudgeStandard.Views.ViewModels
{
    public class BudgetPlanViewModel : BaseViewModel
    {
        public bool ExpensesVisible { get; private set; } = true;
        public bool IncomesVisible { get; private set; } = false;
        public TextDecorations ExpensesChartTextDecorations { get; private set; } = TextDecorations.Underline;
        public TextDecorations IncomesChartTextDecorations { get; private set; } = TextDecorations.None;
        public ICommand ExpensesChartCommand { get; private set; }
        public ICommand IncomesChartCommand { get; private set; }

        private List<ChartData> _expensesChartData;
        private List<ChartData> _incomesChartData;
        public List<ChartData> ExpensesChartData => _expensesChartData;
        public List<ChartData> IncomesChartData => _incomesChartData;

        public BudgetPlanViewModel()
        {
            ExpensesChartCommand = new MvvmHelpers.Commands.Command(ForceSwitchChart);
            IncomesChartCommand = new MvvmHelpers.Commands.Command(ForceSwitchChart);
        }

        internal void ForceSwitchChart()
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

            _incomesChartData = new List<ChartData>();
            var incomesCategories = budgetPlanned.GetIncomesCategories();
            var totalIncome = incomesCategories.Sum(el => el.TotalValues);
            foreach (BudgetPlannedCategory category in incomesCategories)
            {
                foreach (BaseBudgetSubcat subcat in category.subcats)
                {
                    if (subcat.Value > 0)
                    {
                        _incomesChartData.Add(new ChartData { Label = subcat.Name, Value = subcat.Value, Percentage = subcat.Value / totalIncome });
                    }
                }

            }
            _expensesChartData = new List<ChartData>();
            var totalExpense = budgetPlanned.Categories.Sum(el => el.TotalValues) - totalIncome;
            foreach (BudgetPlannedCategory category in budgetPlanned.Categories)
            {
                if (!category.IsIncome && category.TotalValues > 0)
                {
                    _expensesChartData.Add(new ChartData { Label = category.Name, Value = category.TotalValues, Percentage = category.TotalValues / totalExpense });
                }
            }
            //_chartExpense.SetData(chartDataExpenses);
            //_chartIncome.SetData(chartDataIncome);
        }
    }
}
