using Acr.UserDialogs;
using HomeBudgeStandard.Effects;
using HomeBudget.Code;
using HomeBudget.Code.Logic;
using HomeBudget.Converters;
using HomeBudget.Utils;
using Syncfusion.Data;
using Syncfusion.SfDataGrid.XForms;
using System;
using System.Collections.Generic;
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
        private SfDataGrid _dataGrid;

        private CultureInfo _cultureInfoPL = new CultureInfo("pl-PL");
        private DateTime _currentMonth;

        public string Date
        {
            get => _currentMonth.ToString("MMMM yyyy", _cultureInfoPL);
        }

        private bool _hasIncomes;
        private bool _hasExpenses;

        public BudgetRealView ()
		{
            _currentMonth = DateTime.Now;
            Budget = new ObservableCollection<BudgetViewModelData>();
            BindingContext = this;
            InitializeComponent ();

            CreateDataGrid();

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += SwitchChart;
            ExpensesChartSwitch.GestureRecognizers.Add(tapGesture);
            IncomeChartSwitch.GestureRecognizers.Add(tapGesture);
        }

        protected override void OnAppearing()
        {
            try
            {
                if (MainBudget.Instance.IsDataLoaded && !_setupDone)
                {
                    Task.Run(async () => await Setup());
                }
                else
                {
                    OnPropertyChanged(nameof(Budget));
                    UpdateCharts(_currentMonth);
                    ForceSwitchChart(ExpensesChartSwitch);

                    if (_dataGrid != null)
                    {
                        _dataGrid.View.TopLevelGroup.UpdateCaptionSummaries();
                        _dataGrid.View.Refresh();
                    }
                }

                _setupDone = true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private void CreateDataGrid()
        {
            _dataGrid = new SfDataGrid()
            {
                EnableDataVirtualization = true,
                AutoGenerateColumns = false,
                AutoExpandGroups = false,
                AllowGroupExpandCollapse = true,
                LiveDataUpdateMode = Syncfusion.Data.LiveDataUpdateMode.AllowSummaryUpdate,
                SelectionMode = SelectionMode.SingleDeselect,
                NavigationMode = NavigationMode.Cell,
                FrozenColumnsCount = 2,
                EditTapAction = TapAction.OnTap
            };

            _dataGrid.SortComparers.Add(new SortComparer
            {
                PropertyName = "Category.Name",
                Comparer = new BudgetCategorySortComparer()

            });
            _dataGrid.CurrentCellEndEdit += DataGrid_CurrentCellEndEdit;
            _dataGrid.GroupColumnDescriptions.Add(new GroupColumnDescription
            {
                ColumnName = "Category.Name"
            });

            var gridSummaryRow = new GridGroupSummaryRow
            {
                ShowSummaryInRow = true,
                Title = "{Key}: {Total}",
                SummaryColumns = new ObservableCollection<Syncfusion.Data.ISummaryColumn>
                {
                    new GridSummaryColumn
                    {
                        Name = "Total",
                        MappingName="Subcat.Value",
                        SummaryType= SummaryType.Custom,
                        CustomAggregate = new CurrencyDataGridHeader(),
                        Format = "{Currency}"
                    }
                }
            };
            _dataGrid.CaptionSummaryRow = gridSummaryRow;

            _dataGrid.Columns.Add(new GridTextColumn
            {
                MappingName = "Subcat.Name",
                HeaderText = "Kategoria",
                HeaderFontAttribute = FontAttributes.Bold,
                FontAttribute = FontAttributes.Bold,
                Width = 100
            });

            _dataGrid.Columns.Add(new GridNumericColumn
            {
                MappingName = "SubcatReal.Value",
                HeaderText = "Suma",
                HeaderFontAttribute = FontAttributes.Bold,
                //Width = 80,
                CellTextSize = 10,
                FontAttribute = FontAttributes.Bold,
                DisplayBinding = new Binding() { Path = "SubcatReal.Value", Converter = new CurrencyValueConverter() }
            });

            for(int i=0; i<31; i++ )
            {
                _dataGrid.Columns.Add(new GridNumericColumn
                {
                    MappingName = $"SubcatReal.Values[{i}].Value",
                    HeaderText = (i+1).ToString(),
                    AllowEditing = true,
                    //Width = 80,
                    CellTextSize = 10,
                    HeaderFontAttribute = FontAttributes.Bold,
                    DisplayBinding = new Binding() { Path = $"SubcatReal.Values[{i}].Value", Converter = new CurrencyValueConverter() }
                });
            }

            _dataGrid.SetBinding(SfDataGrid.ItemsSourceProperty, nameof(Budget));

            Grid.SetRow(_dataGrid, 2);
            mainGrid.Children.Add(_dataGrid);
        }

        private async Task Setup()
        {
            _currentMonth = DateTime.Now;
            OnPropertyChanged(nameof(Date));
            await SetupDataGrid(_currentMonth);
            UpdateCharts(_currentMonth);
            ForceSwitchChart(ExpensesChartSwitch);

            Device.BeginInvokeOnMainThread(() => UserDialogs.Instance.HideLoading());
        }

        private void UpdateCharts(DateTime date)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var budgetReal = MainBudget.Instance.GetMonth(date).BudgetReal;
                var chartDataExpenses = new List<ChartData>();
                foreach (BudgetRealCategory category in budgetReal.Categories)
                {
                    if (!category.IsIncome && category.TotalValues > 0)
                    {
                        chartDataExpenses.Add(new ChartData { Label = category.Name, Value = category.TotalValues });
                    }
                }
                chartExpense.SetData(chartDataExpenses);


                var chartDataIncome = new List<ChartData>();
                var incomesCategories = budgetReal.GetIncomesCategories();
                foreach (BudgetRealCategory category in incomesCategories)
                {
                    foreach (BaseBudgetSubcat subcat in category.subcats)
                    {
                        if (subcat.Value > 0)
                            chartDataIncome.Add(new ChartData { Label = subcat.Name, Value = subcat.Value });
                    }
                }

                chartIncome.SetData(chartDataIncome);
            });
        }

        private async Task SetupDataGrid(DateTime date)
        {
            await Task.Run(() =>
            {
                    
                var budget = new ObservableCollection<BudgetViewModelData>();
                try
                {
                    var budgetReal = MainBudget.Instance.GetMonth(date).BudgetReal;

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
                }
                catch (Exception e)
                {
                    var msg = e.Message;
                    return;
                }

                Device.BeginInvokeOnMainThread( () =>
                {
                    Budget = budget;
                    OnPropertyChanged(nameof(Budget));

                    PreviousMonthButton.IsEnabled = MainBudget.Instance.HasMonthData(_currentMonth.AddMonths(-1));
                    NextMonthButton.IsEnabled = MainBudget.Instance.HasMonthData(_currentMonth.AddMonths(1));
                });
            });
        }

        private void ForceSwitchChart(Label label)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                label.TextDecorations = TextDecorations.Underline;
                if (label == ExpensesChartSwitch)
                {
                    IncomeChartSwitch.TextDecorations = TextDecorations.None;
                    chartExpense.IsVisible = true;
                    chartIncome.IsVisible = false;
                }
                else
                {
                    ExpensesChartSwitch.TextDecorations = TextDecorations.None;
                    chartExpense.IsVisible = false;
                    chartIncome.IsVisible = true;
                }
            });
        }

        private void SwitchChart(object sender, EventArgs e)
        {
            if (sender is View view && view.Effects.Count == 0)
                ForceSwitchChart(sender as Label);
        }

        private void DataGrid_CurrentCellEndEdit(object sender, GridCurrentCellEndEditEventArgs e)
        {
            Task.Run(() => MainBudget.Instance.Save());

            Device.BeginInvokeOnMainThread(async () =>
            {
                await Task.Delay(100);

                try
                {
                    _dataGrid.View.TopLevelGroup.UpdateCaptionSummaries();
                    _dataGrid.View.Refresh();

                    UpdateCharts(_currentMonth);
                }
                catch(Exception exc)
                {
                    var msg = exc.Message;
                }
            });
        }

        private async void OnPrevMonth(object sender, EventArgs e)
        {
            _currentMonth = _currentMonth.AddMonths(-1);
            await RefreshAsync();
        }

        private async void OnNextMonth(object sender, EventArgs e)
        {
            _currentMonth = _currentMonth.AddMonths(1);
            await RefreshAsync();
        }

        private async Task RefreshAsync()
        {
            OnPropertyChanged(nameof(Date));
            await SetupDataGrid(_currentMonth);
            UpdateCharts(_currentMonth);
        }
    }
}