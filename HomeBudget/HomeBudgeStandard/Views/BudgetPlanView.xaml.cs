//using Acr.UserDialogs;
using Acr.UserDialogs;
using HomeBudget.Code;
using HomeBudget.Code.Logic;
using HomeBudget.Converters;
using HomeBudget.Utils;
using Syncfusion.Data;
using Syncfusion.SfDataGrid.XForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeBudgeStandard.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class BudgetPlanView : ContentPage
	{
        public ObservableCollection<BudgetViewModelData> Budget { get; set; }

        private bool _setupDone;
        private bool _hasIncomes;
        private bool _hasExpenses;
        private DateTime _currentMonth;
        private SfDataGrid _dataGrid;

        private CultureInfo _cultureInfoPL = new CultureInfo("pl-PL");

        public string Date
        {
            get => _currentMonth.ToString("MMMM yyyy", _cultureInfoPL);
        }

        public BudgetPlanView ()
		{
            _currentMonth = DateTime.Now;
            Budget = new ObservableCollection<BudgetViewModelData>();
            BindingContext = this;
            InitializeComponent();

            CreateDataGrid();
            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += SwitchChart;
            ExpensesChartSwitch.GestureRecognizers.Add(tapGesture);
            IncomeChartSwitch.GestureRecognizers.Add(tapGesture);
        }

        protected override void OnAppearing()
        {
            if (MainBudget.Instance.IsDataLoaded && !_setupDone)
            {
                Task.Run(async () => await Setup());
            }
            else
            {
                UpdateCharts(_currentMonth);
                ForceSwitchChart(ExpensesChartSwitch);
            }

            _setupDone = true;
        }

        private async Task Setup()
        {
            _currentMonth = DateTime.Now;

            await SetupDataGrid(_currentMonth);
            UpdateCharts(_currentMonth);
            ForceSwitchChart(ExpensesChartSwitch);
        }

        private void CreateDataGrid()
        {
            _dataGrid = new SfDataGrid()
            {
                EnableDataVirtualization = true,
                AutoGenerateColumns = false,
                AutoExpandGroups = false,
                AllowGroupExpandCollapse = true,
                LiveDataUpdateMode = LiveDataUpdateMode.AllowSummaryUpdate,
                SelectionMode = SelectionMode.Single,
                NavigationMode = NavigationMode.Cell,
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
                SummaryColumns = new ObservableCollection<ISummaryColumn>
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
                Width = 100
            });

            _dataGrid.Columns.Add(new GridNumericColumn
            {
                MappingName = "SubcatPlanned.Value",
                HeaderText = "Suma",
                AllowEditing = true,
                HeaderFontAttribute = FontAttributes.Bold,
                ColumnSizer = ColumnSizer.Star,
                DisplayBinding = new Binding() { Path = "SubcatPlanned.Value", Converter = new CurrencyValueConverter() }
        });

            _dataGrid.SetBinding(SfDataGrid.ItemsSourceProperty, nameof(Budget));

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });

            grid.Children.Add(_dataGrid);
            var button = new Button { Text = "Użyj w kolejnych miesiącach", BackgroundColor = Color.DodgerBlue, TextColor = Color.White, VerticalOptions = LayoutOptions.End, Margin= new Thickness(12, 12) };
            button.Clicked += OnSave;
            grid.Children.Add(button);
            Grid.SetColumn(button, 1);

            Grid.SetRow(grid, 2);
            mainGrid.Children.Add(grid);
        }

        private void UpdateCharts(DateTime date)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var budgetPlanned = MainBudget.Instance.GetMonth(date).BudgetPlanned;
                var chartDataExpenses = new List<ChartData>();
                foreach (BudgetPlannedCategory category in budgetPlanned.Categories)
                {
                    if (!category.IsIncome && category.TotalValues > 0)
                    {
                        chartDataExpenses.Add(new ChartData { Label = category.Name, Value = category.TotalValues });
                    }
                }
                chartExpense.SetData(chartDataExpenses);


                var chartDataIncome = new List<ChartData>();
                var incomesCategories = budgetPlanned.GetIncomesCategories();
                foreach (BudgetPlannedCategory category in incomesCategories)
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

                Device.BeginInvokeOnMainThread(() =>
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

        private async void OnSave(object sender, EventArgs e)
        {
            await MainBudget.Instance.UpdateMainPlannedBudget();
            UserDialogs.Instance.Toast("Ten plan budżetu będzie używany w kolejnych miesiącach");
        }

        private void DataGrid_CurrentCellEndEdit(object sender, GridCurrentCellEndEditEventArgs e)
        {
            Task.Run(() => MainBudget.Instance.Save());

            Device.BeginInvokeOnMainThread(async () =>
            {
                await Task.Delay(100);

                _dataGrid.View.TopLevelGroup.UpdateCaptionSummaries();
                _dataGrid.View.Refresh();

                UpdateCharts(_currentMonth);   
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