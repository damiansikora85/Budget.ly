using Acr.UserDialogs;
using HomeBudgeStandard.Effects;
using HomeBudget.Code;
using HomeBudget.Code.Logic;
using HomeBudget.Utils;
using SkiaSharp;
using SkiaSharp.Views.Forms;
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
        private SKCanvasView _skCanvas;

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

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += SwitchChart;
            ExpensesChartSwitch.GestureRecognizers.Add(tapGesture);
            IncomeChartSwitch.GestureRecognizers.Add(tapGesture);

            var canvasView = new SKCanvasView();
            canvasView.PaintSurface += OnCanvasViewPaintSurface;
            emptyChartView.Content = canvasView;

            MainBudget.Instance.BudgetDataChanged += () => Task.Run(async () =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    UserDialogs.Instance.ShowLoading();
                    //UserDialogs.Instance.Toast("Zaktualizowano dane z Dropbox");
                });
                await Setup();
            });
        }

        private void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            var info = args.Info;
            var surface = args.Surface;
            var canvas = surface.Canvas;

            canvas.Clear();

            using (var paint = new SKPaint{ Style = SKPaintStyle.Fill, Color = Color.FromRgb(200, 200, 200).ToSKColor() })
            {
                canvas.DrawCircle(info.Width / 4, info.Height / 2, info.Height*0.38f, paint);
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

        protected override void OnAppearing()
        {
            try
            {
                if (MainBudget.Instance.IsDataLoaded && !_setupDone)
                {
                    UserDialogs.Instance.ShowLoading();
                    Task.Run(async () => await Setup());
                }
                else SwitchChartForce(ExpensesChartSwitch);

                _setupDone = true;
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private async Task Setup()
        {
            _currentMonth = DateTime.Now;
            OnPropertyChanged(nameof(Date));
            await CreateDataGrid(_currentMonth);
            await CreateCharts(_currentMonth);
             
            Device.BeginInvokeOnMainThread(() => UserDialogs.Instance.HideLoading());
        }

        private async Task CreateCharts(DateTime date)
        {
            await Task.Run(() =>
            {
                ExpensesData = new ObservableCollection<BudgetViewModelData>();
                IncomesData = new ObservableCollection<BudgetViewModelData>();
                var budgetReal = MainBudget.Instance.GetMonth(date).BudgetReal;
                foreach (BudgetRealCategory category in budgetReal.Categories)
                {
                    var model = new BudgetViewModelData
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
                        var model = new BudgetViewModelData
                        {
                            Name = subcat.Name,
                            Category = category,
                            Subcat = subcat as RealSubcat
                        };
                        IncomesData.Add(model);
                    }
                }

                _hasExpenses = ExpensesData.Sum(el => el.CategoryReal.TotalValues) > 0;
                _hasIncomes = IncomesData.Sum(el => el.Category.TotalValues) > 0;

                OnPropertyChanged(nameof(ExpensesData));
                OnPropertyChanged(nameof(IncomesData));

                SwitchChartForce(ExpensesChartSwitch);
            });
        }

        private async Task CreateDataGrid(DateTime date)
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

        private void SwitchChartForce(Label label)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                label.Effects.Add(new UnderlineEffect());
                if (label == ExpensesChartSwitch)
                {
                    IncomeChartSwitch.Effects.Clear();
                    chartExpense.IsVisible = true;
                    chartIncome.IsVisible = false;

                    emptyChartView.IsVisible = !_hasExpenses;
                }
                else
                {
                    ExpensesChartSwitch.Effects.Clear();
                    chartExpense.IsVisible = false;
                    chartIncome.IsVisible = true;

                    emptyChartView.IsVisible = !_hasIncomes;
                }
            });
        }

        private void SwitchChart(object sender, EventArgs e)
        {
            if (sender is View view && view.Effects.Count == 0)
                SwitchChartForce(sender as Label);
        }

        private void DataGrid_CurrentCellEndEdit(object sender, GridCurrentCellEndEditEventArgs e)
        {
            Task.Run(() => MainBudget.Instance.Save());

            Device.BeginInvokeOnMainThread(async () =>
            {
                await Task.Delay(100);

                _hasExpenses = ExpensesData.Sum(el => el.CategoryReal.TotalValues) > 0;
                _hasIncomes = IncomesData.Sum(el => el.Category.TotalValues) > 0;

                emptyChartView.IsVisible = (ExpensesChartSwitch.Effects.Count > 0) ? !_hasExpenses : !_hasIncomes;

                DataGrid.View.TopLevelGroup.UpdateCaptionSummaries();
                DataGrid.View.Refresh();
                OnPropertyChanged("Category.TotalValues");
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
            await CreateCharts(_currentMonth);
        }
    }
}