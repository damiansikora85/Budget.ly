using Acr.UserDialogs;
using HomeBudgeStandard.Effects;
using HomeBudget.Code;
using HomeBudget.Code.Logic;
using HomeBudget.Utils;
using Syncfusion.Data;
using Syncfusion.SfChart.XForms;
using Syncfusion.SfDataGrid.XForms;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeBudgeStandard.Views
{
    [XamlCompilation(XamlCompilationOptions.Skip)]
	public partial class BudgetRealView : ContentPage
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

        public BudgetRealView ()
		{
            Budget = new ObservableCollection<BudgetViewModelData>();
            BindingContext = this;
            InitializeComponent ();

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += SwitchChart;
            ExpensesChartSwitch.GestureRecognizers.Add(tapGesture);
            IncomeChartSwitch.GestureRecognizers.Add(tapGesture);

            MainBudget.Instance.onBudgetLoaded += () => Task.Run(async () =>
            {
                Device.BeginInvokeOnMainThread(() => UserDialogs.Instance.ShowLoading());
                await Setup();
            });
        }

        protected override void OnAppearing()
        {
            try
            {
                if (MainBudget.Instance.IsInitialized && !_setupDone)
                {
                    UserDialogs.Instance.ShowLoading();
                    Task.Run(async () => await Setup());
                }
                _setupDone = true;

                ExpensesChartSwitch.Effects.Add(new UnderlineEffect());
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private async Task Setup()
        {
            await CreateDataGrid();
            //await CreateCharts();
             
            Device.BeginInvokeOnMainThread(() => UserDialogs.Instance.HideLoading());
        }

        private async Task CreateCharts()
        {
            await Task.Run(() =>
            {
                ExpensesData = new ObservableCollection<BudgetViewModelData>();
                IncomesData = new ObservableCollection<BudgetViewModelData>();
                var budgetReal = MainBudget.Instance.GetCurrentMonthData().BudgetReal;
                foreach (BudgetRealCategory category in budgetReal.Categories)
                {
                    var model = new BudgetViewModelData()
                    {
                        Name = category.Name,
                        Category = category,
                        CategoryReal = category as BudgetRealCategory
                    };
                    if (!category.IsIncome)
                        ExpensesData.Add(model);
                }

                var incomesCategories = budgetReal.GetIncomesCategories();
                foreach (BaseBudgetCategory category in incomesCategories)
                {
                    foreach (BaseBudgetSubcat subcat in category.subcats)
                    {
                        var model = new BudgetViewModelData()
                        {
                            Name = subcat.Name,
                            Category = category,
                            Subcat = subcat as RealSubcat
                        };
                        IncomesData.Add(model);
                    }
                }

                OnPropertyChanged(nameof(ExpensesData));
                OnPropertyChanged(nameof(IncomesData));

                SwitchChart(ExpensesChartSwitch, null);
            });
        }

        private async Task CreateDataGrid()
        {
            await Task.Run(() =>
                {
                    
                var budget = new ObservableCollection<BudgetViewModelData>();
                try
                {
                    var budgetReal = MainBudget.Instance.GetCurrentMonthData().BudgetReal;

                    foreach (var category in budgetReal.Categories)
                    {
                        foreach (var subcat in category.subcats)
                        {
                            var model = new BudgetViewModelData
                            {
                                Category = category,
                                Subcat = subcat,
                                SubcatReal = subcat as RealSubcat,
                                Name = category.Name
                            };
                            budget.Add(model);
                        }
                    }

                        //DataGrid.GroupCaptionTextFormat = "{ColumnName} : {Key}"; -> OK

                        /*var summaryRow = new GridGroupSummaryRow
                        {
                            Title = "Total Salary:{TotalSalary}",
                            ShowSummaryInRow = true
                        };
                        summaryRow.SummaryColumns.Add(new GridSummaryColumn
                        {
                            Name = "TotalSalary",
                            MappingName = "Subcat.Value",
                            Format = "{Sum:c}",
                            SummaryType = SummaryType.DoubleAggregate
                        });

                        DataGrid.CaptionSummaryRow = summaryRow;*/

                        /*DataGrid.CaptionSummaryRow = new GridSummaryRow
                        {
                            ShowSummaryInRow = true,
                            Title = "{Key}: {Total}",

                            SummaryColumns = new ObservableCollection<ISummaryColumn>
                            {
                                new GridSummaryColumn
                                {
                                    Name = "Total",
                                    MappingName="Subcat.Value",
                                    SummaryType= SummaryType.CountAggregate,
                                    CustomAggregate = new CurrencyDataGridHeader(),
                                    Format = "{Currency}"
                                }
                            }
                        };*/
                    }
                catch (Exception e)
                {
                    return;
                }

                Device.BeginInvokeOnMainThread( () =>
                {
                    //await Task.Yield();
                    Budget = budget;
                    OnPropertyChanged("Budget");
                    //DataGrid.ItemsSource = Budget;
                    //DataGrid.GroupColumnDescriptions.Clear();
                    //DataGrid.GroupColumnDescriptions.Add(new GroupColumnDescription { ColumnName = "Category.Name" });
                });
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