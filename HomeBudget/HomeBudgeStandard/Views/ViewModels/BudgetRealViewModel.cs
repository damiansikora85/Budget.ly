using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using HomeBudget.Code;
using HomeBudget.Code.Logic;
using HomeBudget.Utils;
using MvvmHelpers;
using Xamarin.Forms;

namespace HomeBudgeStandard.Views.ViewModels
{
    public class BudgetRealViewModel : BaseViewModel
    {
        private DateTime _currentMonth;

        private readonly CultureInfo _cultureInfoPL = new CultureInfo("pl-PL");

        public ICommand PrevMonthCommand
        {
            get; private set;
        }
        public ICommand NextMonthCommand
        {
            get; private set;
        }

        public ICommand ExpensesChartCommand
        {
            get; private set;
        }
        public ICommand IncomesChartCommand
        {
            get; private set;
        }

        public string Date
        {
            get => _currentMonth.ToString("MMMM yyyy", _cultureInfoPL);
        }
        public DateTime CurrentMonth => _currentMonth;

        public bool ExpensesVisible { get; private set; } = true;
        public bool IncomesVisible { get; private set; } = false;
        public TextDecorations ExpensesChartTextDecorations { get; private set; } = TextDecorations.Underline;
        public TextDecorations IncomesChartTextDecorations { get; private set; } = TextDecorations.None;

        public List<ChartData> ExpensesChartData
        {
            get; private set;
        }
        public List<ChartData> IncomesChartData
        {
            get; private set;
        }

        public BudgetRealViewModel()
        {
            _currentMonth = DateTime.Now;
            PrevMonthCommand = new Command(OnPrevMonth);
            NextMonthCommand = new Command(OnNextMonth);
            ExpensesChartCommand = new MvvmHelpers.Commands.Command(ForceSwitchChart);
            IncomesChartCommand = new MvvmHelpers.Commands.Command(ForceSwitchChart);
        }

        public void Setup()
        {
            _currentMonth = DateTime.Now;
            OnPropertyChanged(nameof(Date));
        }

        private void OnNextMonth()
        {
            _currentMonth = _currentMonth.AddMonths(1);
            RefreshUI();
        }

        private void OnPrevMonth()
        {
            _currentMonth = _currentMonth.AddMonths(-1);
            RefreshUI();
        }

        private void RefreshUI()
        {
            OnPropertyChanged(nameof(Date));
            UpdateCharts();
        }

        public void UpdateCharts()
        {
            ExpensesChartData = new List<ChartData>();
            IncomesChartData = new List<ChartData>();

            var budgetReal = MainBudget.Instance.GetMonth(_currentMonth).BudgetReal;

            var incomesCategories = budgetReal.GetIncomesCategories();
            var totalIncome = incomesCategories.Sum(el => el.TotalValues);
            foreach (BudgetRealCategory category in incomesCategories)
            {
                foreach (BaseBudgetSubcat subcat in category.subcats)
                {
                    if (subcat.Value > 0)
                    {
                        IncomesChartData.Add(new ChartData { Label = subcat.Name, Value = subcat.Value, Percentage = subcat.Value / totalIncome });
                    }
                }
            }

            var totalExpenses = budgetReal.Categories.Sum(el => el.TotalValues) - totalIncome;

            foreach (BudgetRealCategory category in budgetReal.Categories)
            {
                if (!category.IsIncome && category.TotalValues > 0)
                {
                    ExpensesChartData.Add(new ChartData { Label = category.Name, Value = category.TotalValues, Percentage = category.TotalValues / totalExpenses });
                }
            }

            OnPropertyChanged(nameof(IncomesChartData));
            OnPropertyChanged(nameof(ExpensesChartData));
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
    }
}
