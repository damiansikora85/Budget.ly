//using Acr.UserDialogs;
using HomeBudgeStandard.Effects;
using HomeBudget.Code;
using HomeBudget.Code.Logic;
using HomeBudget.Utils;
using OxyPlot.Series;
using SkiaSharp;
using SkiaSharp.Views.Forms;
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


    [XamlCompilation(XamlCompilationOptions.Skip)]
	public partial class BudgetPlanView : ContentPage
	{
        public ObservableCollection<BudgetViewModelData> Budget { get; set; }
        //public ObservableCollection<BudgetViewModelData> IncomesData { get; private set; }
        //public ObservableCollection<BudgetViewModelData> ExpensesData { get; set; }

        private bool _setupDone;
        private SKCanvasView _skCanvas;

        private bool _hasIncomes;
        private bool _hasExpenses;
        private DateTime _currentMonth;

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
            MainBudget.Instance.BudgetDataChanged += () => Task.Run(async () =>
            {
                //Device.BeginInvokeOnMainThread(() => UserDialogs.Instance.ShowLoading());
                await Setup();
            });

            var canvasView = new SKCanvasView();
            canvasView.PaintSurface += OnCanvasViewPaintSurface;


            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += SwitchChart;
            ExpensesChartSwitch.GestureRecognizers.Add(tapGesture);
            IncomeChartSwitch.GestureRecognizers.Add(tapGesture);
        }

        protected override void OnAppearing()
        {
            if (MainBudget.Instance.IsDataLoaded && !_setupDone)
            {
                //UserDialogs.Instance.ShowLoading();
                Task.Run(async () => await Setup());
            }
            else ForceSwitchChart(ExpensesChartSwitch);

            _setupDone = true;  
        }

        private void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            var info = args.Info;
            var surface = args.Surface;
            var canvas = surface.Canvas;

            canvas.Clear();

            using (var paint = new SKPaint { Style = SKPaintStyle.Fill, Color = Color.FromRgb(200, 200, 200).ToSKColor() })
            {
                canvas.DrawCircle(info.Width / 4, info.Height / 2, info.Height * 0.38f, paint);
            }

            using (var textPaint = new SKPaint { Color = Color.Black.ToSKColor(), TextSize = info.Width / 2 })
            {
                var message = "Brak danych";
                var textWidth = textPaint.MeasureText(message);
                textPaint.TextSize = 0.7f * info.Height * textPaint.TextSize / textWidth;

                var textBounds = new SKRect();
                textPaint.MeasureText(message, ref textBounds);

                var xText = info.Width / 4 - textBounds.MidX;
                var yText = info.Height / 2 - textBounds.MidY;

                canvas.DrawText(message, xText, yText, textPaint);
            }
        }

        private async Task Setup()
        {
            _currentMonth = DateTime.Now;

            await CreateDataGrid(_currentMonth);
            //await CreateCharts(_currentMonth);
            UpdateCharts(_currentMonth);
            ForceSwitchChart(ExpensesChartSwitch);
            //Device.BeginInvokeOnMainThread(() => UserDialogs.Instance.HideLoading());
        }

        //private async Task CreateCharts(DateTime date)
        //{
        //    await Task.Run(() =>
        //    {
        //        var seriesExpense = new PieSeries();
        //        //ExpensesData = new ObservableCollection<BudgetViewModelData>();
        //        //IncomesData = new ObservableCollection<BudgetViewModelData>();
        //        var budgetPlanned = MainBudget.Instance.GetMonth(date).BudgetPlanned;
        //        foreach (BudgetPlannedCategory category in budgetPlanned.Categories)
        //        {
        //            var model = new BudgetViewModelData()
        //            {
        //                Name = category.Name,
        //                Category = category,
        //            };
        //            if (!category.IsIncome)
        //                ExpensesData.Add(model);
        //            seriesExpense.Slices.Add(new PieSlice(model.Name, model.Category.TotalValues));
        //        }
        //        var plotModelExpense = new OxyPlot.PlotModel();
        //        plotModelExpense.Series.Add(seriesExpense);
        //        chartExpense.Model = plotModelExpense;

        //        var seriesIncome = new PieSeries();

        //        var incomesCategories = budgetPlanned.GetIncomesCategories();
        //        foreach (BudgetPlannedCategory category in incomesCategories)
        //        {
        //            foreach (BaseBudgetSubcat subcat in category.subcats)
        //            {
        //                var model = new BudgetViewModelData
        //                {
        //                    Name = subcat.Name,
        //                    Category = category,
        //                    Subcat = subcat as PlannedSubcat
        //                };
        //                IncomesData.Add(model);
        //                seriesIncome.Slices.Add(new PieSlice(model.Name, model.Subcat.Value));
        //            }
        //        }

        //        var plotModelIncome = new OxyPlot.PlotModel();
        //        plotModelIncome.Series.Add(seriesIncome);
        //        chartIncome.Model = plotModelIncome;

        //        _hasExpenses = ExpensesData.Sum(el => el.Category.TotalValues) > 0;
        //        _hasIncomes = IncomesData.Sum(el => el.Category.TotalValues) > 0;

        //        OnPropertyChanged(nameof(ExpensesData));
        //        OnPropertyChanged(nameof(IncomesData));

        //        ForceSwitchChart(ExpensesChartSwitch);
        //    });
        //}

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

        private async Task CreateDataGrid(DateTime date)
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

        private void SwitchChart(object sender, EventArgs e)
        {
            if (sender is View view && view.Effects.Count == 0)
                ForceSwitchChart(sender as Label);
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

                //_hasExpenses = ExpensesData.Sum(el => el.Category.TotalValues) > 0;
                //_hasIncomes = IncomesData.Sum(el => el.Category.TotalValues) > 0;
                //emptyChartView.IsVisible = (ExpensesChartSwitch.Effects.Count > 0) ? !_hasExpenses : !_hasIncomes;
                //chartExpense.BindingContext = this;
                //chartIncome.BindingContext = this;

                DataGrid.View.TopLevelGroup.UpdateCaptionSummaries();
                DataGrid.View.Refresh();
                //OnPropertyChanged("Category.TotalValues");
                //OnPropertyChanged("ExpensesData");

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
            await CreateDataGrid(_currentMonth);
            //await CreateCharts(_currentMonth);
            UpdateCharts(_currentMonth);
        }
    }
}