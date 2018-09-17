using Acr.UserDialogs;
using HomeBudgeStandard.Effects;
using HomeBudget.Code;
using HomeBudget.Code.Logic;
using HomeBudget.Utils;
using Syncfusion.Data;
using Syncfusion.SfDataGrid.XForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeBudgeStandard.Views
{
	[XamlCompilation(XamlCompilationOptions.Skip)]
	public partial class BudgetPlanView : ContentPage
	{
        public ObservableCollection<BudgetViewModelData> Budget { get; set; }
        public ObservableCollection<BudgetViewModelData> IncomesData { get; private set; }
        public ObservableCollection<BudgetViewModelData> ExpensesData { get; set; }

        private bool _setupDone;

        private CultureInfo _cultureInfoPL = new CultureInfo("pl-PL");

        public string Date
        {
            get => DateTime.Now.ToString("MMMM yyyy", _cultureInfoPL);
        }

        public BudgetPlanView ()
		{
            Budget = new ObservableCollection<BudgetViewModelData>();
            BindingContext = this;
            InitializeComponent();
            MainBudget.Instance.onBudgetLoaded += Setup;
        }

        protected override void OnAppearing()
        {
            if (MainBudget.Instance.IsInitialized && !_setupDone)
            {
                UserDialogs.Instance.ShowLoading();
                Setup();
            }
            _setupDone = true;

            ExpensesChartSwitch.Effects.Add(new UnderlineEffect());

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += SwitchChart;
            ExpensesChartSwitch.GestureRecognizers.Add(tapGesture);
            IncomeChartSwitch.GestureRecognizers.Add(tapGesture);
        }

        private void Setup()
        {
            Task.Run(() =>
            {
                CreateDataGrid();
                CreateCharts();
                Device.BeginInvokeOnMainThread(() => UserDialogs.Instance.HideLoading());
            });
        }

        private void CreateCharts()
        {
            ExpensesData = new ObservableCollection<BudgetViewModelData>();
            IncomesData = new ObservableCollection<BudgetViewModelData>();
            var budgetPlanned = MainBudget.Instance.GetCurrentMonthData().BudgetPlanned;
            foreach (BudgetPlannedCategory category in budgetPlanned.Categories)
            {
                var model = new BudgetViewModelData()
                {
                    Name = category.Name,
                    Category = category,
                };
                if (!category.IsIncome)
                    ExpensesData.Add(model);
            }

            var incomesCategories = budgetPlanned.GetIncomesCategories();
            foreach (BudgetPlannedCategory category in incomesCategories)
            {
                foreach (BaseBudgetSubcat subcat in category.subcats)
                {
                    var model = new BudgetViewModelData()
                    {
                        Name = subcat.Name,
                        Category = category,
                        Subcat = subcat as PlannedSubcat
                    };
                    IncomesData.Add(model);
                }
            }

            OnPropertyChanged(nameof(ExpensesData));
            OnPropertyChanged(nameof(IncomesData));

            SwitchChart(ExpensesChartSwitch, null);
        }

        private void CreateDataGrid()
        {
            var budget = new ObservableCollection<BudgetViewModelData>();
            try
            {
                var budgetPlanned = MainBudget.Instance.GetCurrentMonthData().BudgetPlanned;

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

                DataGrid.CaptionSummaryRow = new GridSummaryRow()
                {
                    ShowSummaryInRow = true,
                    Title = "{Key}: {Total}",

                    SummaryColumns = new ObservableCollection<ISummaryColumn>
                    {
                        new GridSummaryColumn()
                        {
                            Name = "Total",
                            MappingName="Subcat.Value",
                            SummaryType= SummaryType.Custom,
                            CustomAggregate = new CurrencyDataGridHeader(),
                            Format = "{Currency}"
                        }
                    }
                };
            }
            catch (Exception e)
            {
                return;
            }

            Device.BeginInvokeOnMainThread(() =>
            {
                Budget = budget;
                DataGrid.ItemsSource = Budget;
                DataGrid.GroupColumnDescriptions.Clear();
                DataGrid.GroupColumnDescriptions.Add(new GroupColumnDescription { ColumnName = "Category.Name" });
            });
        }

        private void SwitchChart(object sender, EventArgs e)
        {
            if (sender is View view && view.Effects.Count > 0)
                return;

            Device.BeginInvokeOnMainThread(() =>
            {
                var label = sender as Label;
                label.Effects.Add(new UnderlineEffect());
                if (label == ExpensesChartSwitch)
                {
                    IncomeChartSwitch.Effects.Clear();
                    chartExpense.IsVisible = true;
                    chartIncome.IsVisible = false;
                }
                else
                {
                    ExpensesChartSwitch.Effects.Clear();
                    chartExpense.IsVisible = false;
                    chartIncome.IsVisible = true;
                }
            });
        }

        private async void OnSave(object sender, EventArgs e)
        {
            await MainBudget.Instance.UpdateMainPlannedBudget();
        }

        private void DataGrid_CurrentCellEndEdit(object sender, GridCurrentCellEndEditEventArgs e)
        {
            Task.Run(() => MainBudget.Instance.Save());

            Device.BeginInvokeOnMainThread(async () =>
            {
                await Task.Delay(100);
                DataGrid.View.TopLevelGroup.UpdateCaptionSummaries();
                DataGrid.View.Refresh();
                OnPropertyChanged("Category.TotalValues");
            });
        }
    }
}