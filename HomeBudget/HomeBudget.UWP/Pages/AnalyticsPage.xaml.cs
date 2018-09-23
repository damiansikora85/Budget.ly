using HomeBudget.Code;
using HomeBudget.Code.Logic;
using HomeBudget.Utils;
using Syncfusion.Data;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SkiaSharp;
using SkiaSharp.Views;
using SkiaSharp.Views.UWP;
using System.Linq;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace HomeBudget.UWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AnalyticsPage : Page, INotifyPropertyChanged
    {
        private ObservableCollection<BudgetViewModelData> Budget = new ObservableCollection<BudgetViewModelData>();
        public ObservableCollection<BudgetViewModelData> IncomesData { get; set; }
        public ObservableCollection<BudgetViewModelData> ExpensesData { get; set; }

        private CultureInfo _cultureInfoPL = new CultureInfo("pl-PL");
        private DateTime _currentDate;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Date
        {
            get => _currentDate.ToString("MMMM yyyy", _cultureInfoPL);
        }

        public AnalyticsPage()
        {
            _currentDate = DateTime.Now;
            InitializeComponent();
            
            CreateCharts(_currentDate);
            CreateDataGrid(_currentDate);
        }

        private void OnPaint(object sender, SKPaintSurfaceEventArgs args)
        {
            var info = args.Info;
            var surface = args.Surface;
            var canvas = surface.Canvas;

            canvas.Clear();

            using (var paint = new SKPaint { Style = SKPaintStyle.Fill, Color = SKColors.LightGray })
            {
                canvas.DrawCircle(info.Width / 2, info.Height / 2, info.Height * 0.38f, paint);
            }

            using (var textPaint = new SKPaint { Color = SKColors.Black, TextSize = info.Width / 2 })
            {
                var message = "Brak danych";
                var textWidth = textPaint.MeasureText(message);
                textPaint.TextSize = 0.7f * info.Height * textPaint.TextSize / textWidth;

                var textBounds = new SKRect();
                textPaint.MeasureText(message, ref textBounds);

                var xText = info.Width / 2 - textBounds.MidX;
                var yText = info.Height / 2 - textBounds.MidY;

                canvas.DrawText(message, xText, yText, textPaint);
            }
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
                    var model = new BudgetViewModelData()
                    {
                        Name = category.Name,
                        Category = category,
                    };
                    if (category.IsIncome == false)
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

                Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    OnPropertyChanged("IncomesData");
                    OnPropertyChanged("ExpensesData");
                    emptyChartViewExpenses.Visibility = (ExpensesData.Sum(el => el.Category.TotalValues) > 0) ? Visibility.Collapsed : Visibility.Visible;
                    emptyChartViewIncomes.Visibility = (IncomesData.Sum(el => el.Category.TotalValues) > 0) ? Visibility.Collapsed : Visibility.Visible;
                });
            });
        }

        private async Task CreateDataGrid(DateTime date)
        {
            await Task.Run(() =>
            {
                var budgetReal = MainBudget.Instance.GetMonth(date).BudgetReal;
                var budgetTemp = new ObservableCollection<BudgetViewModelData>();

                foreach (var category in budgetReal.Categories)
                {
                    foreach (var subcat in category.subcats)
                    {
                        var model = new BudgetViewModelData
                        {
                            Category = category,
                            Subcat = subcat
                        };
                        budgetTemp.Add(model);
                    }
                }

                Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    Budget = budgetTemp;
                    DataGrid.CaptionSummaryRow = new GridSummaryRow()
                    {
                        ShowSummaryInRow = true,
                        Title = "{Key}: {Total}",

                        SummaryColumns = new ObservableCollection<ISummaryColumn>()
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
                
                    PreviousMonthButton.IsEnabled = MainBudget.Instance.HasMonthData(_currentDate.AddMonths(-1));
                    NextMonthButton.IsEnabled = MainBudget.Instance.HasMonthData(_currentDate.AddMonths(1));

                    DataGrid.ItemsSource = Budget;
                });
            });
        }

        private void RecordChanged(object sender, PropertyChangedEventArgs e)
        {
            DataGrid.View.TopLevelGroup.UpdateCaptionSummaries();
        }

        private void DataGrid_CurrentCellEndEdit(object sender, CurrentCellEndEditEventArgs e)
        {
            Task.Run(() => MainBudget.Instance.Save());

            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () => 
            {
                await Task.Delay(100);

                emptyChartViewExpenses.Visibility = (ExpensesData.Sum(el => el.Category.TotalValues) > 0) ? Visibility.Collapsed : Visibility.Visible;
                emptyChartViewIncomes.Visibility = (IncomesData.Sum(el => el.Category.TotalValues) > 0) ? Visibility.Collapsed : Visibility.Visible;

                DataGrid.View.TopLevelGroup.UpdateCaptionSummaries();
                DataGrid.View.Refresh();
            });
        }

        private async Task RefreshAsync()
        {
            OnPropertyChanged(nameof(Date));
            await CreateDataGrid(_currentDate);
            await CreateCharts(_currentDate);
        }

        private async void PreviousMonth_Click(object sender, RoutedEventArgs e)
        {
            _currentDate = _currentDate.AddMonths(-1);
            await RefreshAsync();
        }

        private async void NextMonth_Click(object sender, RoutedEventArgs e)
        {
            _currentDate = _currentDate.AddMonths(1);
            await RefreshAsync();
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
