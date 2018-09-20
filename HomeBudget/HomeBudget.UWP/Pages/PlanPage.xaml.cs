using HomeBudget.Code;
using HomeBudget.Code.Logic;
using HomeBudget.Utils;
using SkiaSharp;
using SkiaSharp.Views.UWP;
using Syncfusion.Data;
using Syncfusion.UI.Xaml.Charts;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace HomeBudget.UWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlanPage : Page
    {
        private ObservableCollection<BudgetViewModelData> Budget = new ObservableCollection<BudgetViewModelData>();
        public ObservableCollection<BudgetViewModelData> IncomesData { get; set; }
        public ObservableCollection<BudgetViewModelData> ExpensesData { get; set; }

        public PlanPage()
        {
            InitializeComponent();
            CreateDataGrid();
            CreateCharts();
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

        private void CreateCharts()
        {
            ExpensesData = new ObservableCollection<BudgetViewModelData>();
            IncomesData = new ObservableCollection<BudgetViewModelData>();
            var budgetPlan = MainBudget.Instance.GetCurrentMonthData().BudgetPlanned;
            foreach (var category in budgetPlan.Categories)
            {
                var model = new BudgetViewModelData()
                {
                    Name = category.Name,
                    Category = category,
                };
                if (category.IsIncome == false)
                    ExpensesData.Add(model);
            }

            var incomesCategories = budgetPlan.GetIncomesCategories();
            foreach (var category in incomesCategories)
            {
                foreach (var subcat in category.subcats)
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

            emptyChartViewExpenses.Visibility = (ExpensesData.Sum(el => el.Category.TotalValues) > 0) ? Visibility.Collapsed : Visibility.Visible;
            emptyChartViewIncomes.Visibility = (IncomesData.Sum(el => el.Category.TotalValues) > 0) ? Visibility.Collapsed : Visibility.Visible;
        }

        private void SetupChart(SfChart chart, ObservableCollection<BudgetViewModelData> data, string xBindingPath, string yBindingPath)
        {
            var pieSeries = new PieSeries()
            {
                ItemsSource = data,
                XBindingPath = xBindingPath,
                YBindingPath = yBindingPath,
                EnableSmartLabels = true,
                ListenPropertyChange = true,
                ExplodeOnMouseClick = true,

            };

            chart.Series.Add(pieSeries);
        }

        private void CreateDataGrid()
        {
            var budgetPlan = MainBudget.Instance.GetCurrentMonthData().BudgetPlanned;

            foreach (var category in budgetPlan.Categories)
            {
                foreach (var subcat in category.subcats)
                {
                    var model = new BudgetViewModelData
                    {
                        Category = category,
                        Subcat = subcat
                    };
                    Budget.Add(model);
                }
            }

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
                        Format = "{Currency}",

                    }
                }
            };

            DataGrid.ItemsSource = Budget;
            DataGrid.GroupColumnDescriptions.Add(new GroupColumnDescription() { ColumnName = "Category.Name" });
        }

        private void OnSave(object sender, RoutedEventArgs e)
        {
            Task.Run(async () => await MainBudget.Instance.UpdateMainPlannedBudget());
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
    }
}
