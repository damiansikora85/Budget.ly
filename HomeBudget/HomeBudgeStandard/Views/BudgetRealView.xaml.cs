using Acr.UserDialogs;
using HomeBudgeStandard.Converters;
using HomeBudgeStandard.Effects;
using HomeBudgeStandard.Interfaces;
using HomeBudgeStandard.Utils;
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
	public partial class BudgetRealView : ContentPage, IActiveAware
    {
        public ObservableCollection<BudgetViewModelData> Budget { get; set; }
        public ObservableCollection<BudgetViewModelData> IncomesData { get; private set; }
        public ObservableCollection<BudgetViewModelData> ExpensesData { get; set; }

        private bool _setupDone;
        private SfDataGrid _dataGrid;

        private CultureInfo _cultureInfoPL = new CultureInfo("pl-PL");
        private DateTime _currentMonth;

        private Label _expensesChartSwitch;
        private Label _incomeChartSwitch;
        private Grid _mainGrid;
        private BudgetChart _chartExpense;
        private BudgetChart _chartIncome;
        private Button _previousMonthButton;
        private Button _nextMonthButton;

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

        public BudgetRealView()
        {
            _currentMonth = DateTime.Now;
            Budget = new ObservableCollection<BudgetViewModelData>();
            BindingContext = this;
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            if (!IsActive) return;
            try
            {
                if (MainBudget.Instance.IsDataLoaded && !_setupDone)
                {
                    Setup();
                }
                else
                {
                    OnPropertyChanged(nameof(Budget));
                    UpdateCharts(_currentMonth);
                    ForceSwitchChart(_expensesChartSwitch);

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

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += SwitchChart;
            _expensesChartSwitch.GestureRecognizers.Add(tapGesture);
            _incomeChartSwitch.GestureRecognizers.Add(tapGesture);

            if (MainBudget.Instance.IsDataLoaded && !_setupDone)
            {
                Setup();
                //Task.Run(async () => await Setup());
            }
            else
            {
                OnPropertyChanged(nameof(Budget));
                UpdateCharts(_currentMonth);
                ForceSwitchChart(_expensesChartSwitch);

                if (_dataGrid != null)
                {
                    _dataGrid.View.TopLevelGroup.UpdateCaptionSummaries();
                    _dataGrid.View.Refresh();
                }
            }

            UserDialogs.Instance.HideLoading();
            _setupDone = true;
        }

        private void SetupVariables()
        {
            _expensesChartSwitch = this.Content.FindByName<Label>("ExpensesChartSwitch");
            _incomeChartSwitch = this.Content.FindByName<Label>("IncomeChartSwitch");
            _mainGrid = this.Content.FindByName<Grid>("mainGrid");
            _chartExpense = this.Content.FindByName<BudgetChart>("chartExpense");
            _chartIncome = this.Content.FindByName<BudgetChart>("chartIncome");
            _previousMonthButton = this.Content.FindByName<Button>("PreviousMonthButton");
            _nextMonthButton = this.Content.FindByName<Button>("NextMonthButton");
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
                EditTapAction = TapAction.OnTap,
                GridStyle = new BudgetDataGridStyle()
            };

            _dataGrid.SortComparers.Add(new SortComparer
            {
                PropertyName = "Category.Name",
                Comparer = new BudgetCategorySortComparer()

            });
            _dataGrid.CurrentCellEndEdit += DataGrid_CurrentCellEndEdit;
            _dataGrid.GroupColumnDescriptions.Add(new GroupColumnDescription
            {
                ColumnName = "Category.Name",
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
                        Format = "{Currency}",
                    }
                },
            };
            _dataGrid.CaptionSummaryRow = gridSummaryRow;

            _dataGrid.CaptionSummaryTemplate = new DataTemplate(() =>
            {
                var stackLayout = new StackLayout { Orientation = StackOrientation.Horizontal, Margin = new Thickness(5,0) };
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

            _dataGrid.Columns.Add(new GridTextColumn
            {
                MappingName = "Subcat.Name",
                HeaderText = "Kategoria",
                Width = 100,
                HeaderFont = "FiraSans-Bold.otf#Fira Sans Bold",
                RecordFont = "FiraSans-Regular.otf#Fira Sans Regular",
                HeaderCellTextSize = 16,
                LoadUIView = true
            });

            _dataGrid.Columns.Add(new GridNumericColumn
            {
                MappingName = "SubcatReal.Value",
                HeaderText = "Suma",
                HeaderFont = "FiraSans-Bold.otf#Fira Sans Bold",
                RecordFont = "FiraSans-Bold.otf#Fira Sans Bold",
                LoadUIView = true,
                CellTextSize = 10,
                HeaderCellTextSize = 16,
                DisplayBinding = new Binding() { Path = "SubcatReal.Value", Converter = new CurrencyValueConverter() }
            });

            for(int i=0; i<31; i++ )
            {
                _dataGrid.Columns.Add(new GridNumericColumn
                {
                    MappingName = $"SubcatReal.Values[{i}].Value",
                    HeaderText = (i+1).ToString(),
                    AllowEditing = true,
                    LoadUIView = true,
                    HeaderFont = "FiraSans-Bold.otf#Fira Sans Bold",
                    RecordFont = "FiraSans-Regular.otf#Fira Sans Regular",
                    CellTextSize = 10,
                    HeaderCellTextSize = 16,
                    HeaderFontAttribute = FontAttributes.Bold,
                    DisplayBinding = new Binding() { Path = $"SubcatReal.Values[{i}].Value", Converter = new CurrencyValueConverter() }
                });
            }

            _dataGrid.SetBinding(SfDataGrid.ItemsSourceProperty, nameof(Budget));

            Grid.SetRow(_dataGrid, 2);
            _mainGrid.Children.Add(_dataGrid);
        }

        private async void Setup()
        {
            _currentMonth = DateTime.Now;
            OnPropertyChanged(nameof(Date));
            await UpdateCharts(_currentMonth);
            await SetupDataGrid(_currentMonth);
            ForceSwitchChart(_expensesChartSwitch);
        }

        private async Task UpdateCharts(DateTime date)
        {
            var chartDataExpenses = new List<ChartData>();
            var chartDataIncome = new List<ChartData>();

            await Task.Factory.StartNew(() =>
            {
                var budgetReal = MainBudget.Instance.GetMonth(date).BudgetReal;
                
                foreach (BudgetRealCategory category in budgetReal.Categories)
                {
                    if (!category.IsIncome && category.TotalValues > 0)
                    {
                        chartDataExpenses.Add(new ChartData { Label = category.Name, Value = category.TotalValues });
                    }
                }

                var incomesCategories = budgetReal.GetIncomesCategories();
                foreach (BudgetRealCategory category in incomesCategories)
                {
                    foreach (BaseBudgetSubcat subcat in category.subcats)
                    {
                        if (subcat.Value > 0)
                            chartDataIncome.Add(new ChartData { Label = subcat.Name, Value = subcat.Value });
                    }
                }
            });
                
            _chartExpense.SetData(chartDataExpenses);
            _chartIncome.SetData(chartDataIncome);
          
        }

        private async Task SetupDataGrid(DateTime date)
        {
            var budget = new ObservableCollection<BudgetViewModelData>();
            await Task.Factory.StartNew(() =>
            {
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
            });

            Device.BeginInvokeOnMainThread(() =>
            {
               Budget = budget;
               OnPropertyChanged(nameof(Budget));

               _previousMonthButton.IsEnabled = MainBudget.Instance.HasMonthData(_currentMonth.AddMonths(-1));
               _nextMonthButton.IsEnabled = MainBudget.Instance.HasMonthData(_currentMonth.AddMonths(1));
            });
        }

        private void ForceSwitchChart(Label label)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                label.TextDecorations = TextDecorations.Underline;
                if (label == _expensesChartSwitch)
                {
                    _incomeChartSwitch.TextDecorations = TextDecorations.None;
                    _chartExpense.IsVisible = true;
                    _chartIncome.IsVisible = false;
                }
                else
                {
                    _expensesChartSwitch.TextDecorations = TextDecorations.None;
                    _chartExpense.IsVisible = false;
                    _chartIncome.IsVisible = true;
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

        private void OnPrevMonth(object sender, EventArgs e)
        {
            _currentMonth = _currentMonth.AddMonths(-1);
            RefreshAsync();
        }

        private void OnNextMonth(object sender, EventArgs e)
        {
            _currentMonth = _currentMonth.AddMonths(1);
            RefreshAsync();
        }

        private void RefreshAsync()
        {
            OnPropertyChanged(nameof(Date));
            SetupDataGrid(_currentMonth);
            UpdateCharts(_currentMonth);
        }
    }
}