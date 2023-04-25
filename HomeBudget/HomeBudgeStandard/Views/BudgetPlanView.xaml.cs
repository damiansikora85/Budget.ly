//using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using HomeBudgeStandard.Interfaces;
using HomeBudgeStandard.Views.ViewModels;
using HomeBudget.Code;
using HomeBudget.Code.Logic;
using HomeBudget.Converters;
using HomeBudget.Utils;
using Syncfusion.Data;
using Syncfusion.SfDataGrid.XForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeBudgeStandard.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class BudgetPlanView : ContentPage, IActiveAware
    {
        //public ObservableCollection<BudgetViewModelData> Budget { get; set; }
        private BudgetPlanViewModel _viewModel;

        private bool _setupDone;
        private bool _hasIncomes;
        private bool _hasExpenses;
        private DateTime _currentMonth;
        private SfDataGrid _dataGrid;

        //private Label _expensesChartSwitch;
        private Label _incomeChartSwitch;
        private Grid _mainGrid;
        //private BudgetChart _chartExpense;
        //private BudgetChart _chartIncome;
        private Button _previousMonthButton;
        private Button _nextMonthButton;

        private CultureInfo _cultureInfoPL = new CultureInfo("pl-PL");

        public string Date
        {
            get => _currentMonth.ToString("MMMM yyyy", _cultureInfoPL);
        }

        public event EventHandler IsActiveChanged;

        bool _isActive;
        public virtual bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                }
            }
        }

        public BudgetPlanView ()
		{
            _currentMonth = DateTime.Now;
            _viewModel = new BudgetPlanViewModel();
            BindingContext = _viewModel;
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            if (!IsActive) return;

            MainBudget.Instance.BudgetDataChanged -= MarkDataChanged;

            if (MainBudget.Instance.IsDataLoaded && !_setupDone)
            {
                _viewModel.Setup();
                Setup();

            }
            else
            {
                UpdateCharts(_currentMonth);
                ForceSwitchChart();
            }

            _setupDone = true;
        }

        protected override void OnDisappearing()
        {
            MainBudget.Instance.BudgetDataChanged += MarkDataChanged;
        }

        private void MarkDataChanged(bool obj)
        {
            _setupDone = false;
        }

        public async Task Activate()
        {
            UserDialogs.Instance.ShowLoading("");

            var dataTemplate = (DataTemplate)Resources["ContentTemplate"];
            View view = null;
            await Task.Factory.StartNew(() =>
            {
                view = (View)dataTemplate.CreateContent();
            });
            this.Content = view;

            SetupVariables();
            CreateDataGrid();

            //var tapGesture = new TapGestureRecognizer();
            //tapGesture.Tapped += SwitchChart;
            //_expensesChartSwitch.GestureRecognizers.Add(tapGesture);
            //_incomeChartSwitch.GestureRecognizers.Add(tapGesture);

            if (MainBudget.Instance.IsDataLoaded && !_setupDone)
            {
                Setup();
            }
            else
            {
                UpdateCharts(_currentMonth);
                ForceSwitchChart();
            }

            _setupDone = true;

            UserDialogs.Instance.HideLoading();
        }

        private void SetupVariables()
        {
            //_expensesChartSwitch = this.Content.FindByName<Label>("ExpensesChartSwitch");
            //_incomeChartSwitch = this.Content.FindByName<Label>("IncomeChartSwitch");
            _mainGrid = this.Content.FindByName<Grid>("mainGrid");
            //_chartExpense = this.Content.FindByName<BudgetChart>("chartExpense");
            //_chartIncome = this.Content.FindByName<BudgetChart>("chartIncome");
            _previousMonthButton = this.Content.FindByName<Button>("PreviousMonthButton");
            _nextMonthButton = this.Content.FindByName<Button>("NextMonthButton");
        }

        private async Task Setup()
        {
            _currentMonth = DateTime.Now;

            //SetupDataGrid(_currentMonth);
            UpdateCharts(_currentMonth);
            ForceSwitchChart();
        }

        private void CreateDataGrid()
        {
            _dataGrid = new SfDataGrid
            {
                EnableDataVirtualization = true,
                AutoGenerateColumns = false,
                AutoExpandGroups = false,
                AllowGroupExpandCollapse = true,
                LiveDataUpdateMode = LiveDataUpdateMode.AllowSummaryUpdate,
                SelectionMode = Syncfusion.SfDataGrid.XForms.SelectionMode.Single,
                NavigationMode = NavigationMode.Cell,
                EditTapAction = TapAction.OnTap,
                GridStyle = new BudgetDataGridStyle(),
                Margin = new Thickness(12, 0),
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

            _dataGrid.CaptionSummaryTemplate = new DataTemplate(() =>
            {
                var stackLayout = new StackLayout { Orientation = StackOrientation.Horizontal, Margin = new Thickness(5, 0) };
                var label = new Label { FontFamily = "FiraSans-Regular.otf#Fira Sans Regular", VerticalTextAlignment = TextAlignment.Center, FontSize = 16, TextColor = Color.Black };

                var icon = new Image { HeightRequest = 25 };
                icon.SetBinding(Image.SourceProperty, new Binding(".", BindingMode.Default, new BudgetGridIconConverter(), _dataGrid));

                var converter = new BudgetDataGridSummaryConverter();
                var binding = new Binding(".", BindingMode.Default, converter, _dataGrid);
                label.SetBinding(Label.TextProperty, binding);

                stackLayout.Children.Add(icon);
                stackLayout.Children.Add(label);

                return new ViewCell { View = stackLayout };
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
                HeaderFont = "FiraSans-Bold.otf#Fira Sans Bold",
                RecordFont = "FiraSans-Regular.otf#Fira Sans Regular",
                HeaderCellTextSize = 16,
                LoadUIView = true,
                ColumnSizer = ColumnSizer.Star,
                //Width = 100
            });

            _dataGrid.Columns.Add(new GridNumericColumn
            {
                MappingName = "SubcatPlanned.Value",
                HeaderText = "Suma",
                AllowEditing = true,
                HeaderFont = "FiraSans-Bold.otf#Fira Sans Bold",
                RecordFont = "FiraSans-Regular.otf#Fira Sans Regular",
                //LoadUIView = true,
                CellTextSize = 14,
                HeaderCellTextSize = 16,
                ColumnSizer = ColumnSizer.Star,

                DisplayBinding = new Binding() { Path = "SubcatPlanned.Value", Converter = new CurrencyValueConverter() }
        });

            _dataGrid.SetBinding(SfDataGrid.ItemsSourceProperty, new Binding(path: nameof(_viewModel.Budget), source: _viewModel));

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(3, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = 50 });

            grid.Children.Add(_dataGrid);
            var button = new Button { Text = "Użyj w kolejnych miesiącach", Style = (Style)Application.Current.Resources["ButtonStyle"], VerticalOptions = LayoutOptions.End, Margin= new Thickness(12, 3) };
            button.Clicked += OnSave;
            grid.Children.Add(button);
            Grid.SetRow(button, 1);

            Grid.SetRow(grid, 2);
            _mainGrid.Children.Add(grid);
        }

        private void UpdateCharts(DateTime date)
        {
            _viewModel.UpdateCharts(date);

            Device.BeginInvokeOnMainThread(() =>
            {
                var budgetPlanned = MainBudget.Instance.GetMonth(date).BudgetPlanned;

                var chartDataIncome = new List<ChartData>();
                var incomesCategories = budgetPlanned.GetIncomesCategories();
                var totalIncome = incomesCategories.Sum(el => el.TotalValues);
                foreach (BudgetPlannedCategory category in incomesCategories)
                {
                    foreach (BaseBudgetSubcat subcat in category.subcats)
                    {
                        if (subcat.Value > 0)
                            chartDataIncome.Add(new ChartData { Label = subcat.Name, Value = subcat.Value, Percentage = subcat.Value / totalIncome });
                    }

                }
                var chartDataExpenses = new List<ChartData>();
                var totalExpense = budgetPlanned.Categories.Sum(el => el.TotalValues) - totalIncome;
                foreach (BudgetPlannedCategory category in budgetPlanned.Categories)
                {
                    if (!category.IsIncome && category.TotalValues > 0)
                    {
                        chartDataExpenses.Add(new ChartData { Label = category.Name, Value = category.TotalValues, Percentage = category.TotalValues / totalExpense });
                    }
                }
                //_chartExpense.SetData(chartDataExpenses);

                //_chartIncome.SetData(chartDataIncome);
            });
        }

        private void ForceSwitchChart()//Label label)
        {
            _viewModel.ForceSwitchChart();

            //Device.BeginInvokeOnMainThread(() =>
            //{
            //    label.TextDecorations = TextDecorations.Underline;
            //    if (label == _expensesChartSwitch)
            //    {
            //        _incomeChartSwitch.TextDecorations = TextDecorations.None;
            //        _chartExpense.IsVisible = true;
            //        _chartIncome.IsVisible = false;
            //    }
            //    else
            //    {
            //        _expensesChartSwitch.TextDecorations = TextDecorations.None;
            //        _chartExpense.IsVisible = false;
            //        _chartIncome.IsVisible = true;
            //    }
            //});
        }

        private void SwitchChart(object sender, EventArgs e)
        {
            if (sender is View view && view.Effects.Count == 0)
            {
                ForceSwitchChart();
            }
        }

        private async void OnSave(object sender, EventArgs e)
        {
            await MainBudget.Instance.UpdateMainPlannedBudget(_currentMonth);
            UserDialogs.Instance.Toast("Ten plan budżetu będzie używany w kolejnych miesiącach");
        }

        private void DataGrid_CurrentCellEndEdit(object sender, GridCurrentCellEndEditEventArgs e)
        {
            if (e.OldValue is double oldValue && e.NewValue is double newValue && oldValue == newValue) return;

            Task.Factory.StartNew(() => MainBudget.Instance.Save());

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
            //await SetupDataGrid(_currentMonth);
            UpdateCharts(_currentMonth);
        }
    }
}