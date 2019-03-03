using Acr.UserDialogs;
using HomeBudgeStandard.Interfaces;
using HomeBudget.Code;
using HomeBudget.Code.Logic;
using HomeBudget.Converters;
using HomeBudget.Pages;
using HomeBudget.Utils;
using Syncfusion.Data;
using Syncfusion.SfDataGrid.XForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
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
            //CreateDataGrid();

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

        private async void OnDetailsClick(object sender, EventArgs args)
        {
            //MessagingCenter.Send(this, "Landscape");
            await Navigation.PushAsync(new BudgetDataGridPage());
        }

        private async void Setup()
        {
            _currentMonth = DateTime.Now;
            OnPropertyChanged(nameof(Date));
            await UpdateCharts(_currentMonth);
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
            UpdateCharts(_currentMonth);
        }
    }
}