using HomeBudgeStandard.Pages.Common;
using HomeBudget.Code;
using HomeBudget.Code.Logic;
using HomeBudget.Utils;
using Syncfusion.SfChart.XForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using Xamarin.Forms;

namespace HomeBudget.Pages.PC
{
    //[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlanningPage : ContentPage
    {
        ObservableCollection<BudgetViewModelData> plannedModel;

        public PlanningPage()
        {
            InitializeComponent();
            sideBar.SetMode(Views.SideBarPC.EMode.Planning);
            SetupTable();
            //SetupDataGrid();
            SetupCharts();
            UpdateSummary(null, null);
        }

        private void SetupDataGrid()
        {
            plannedModel = new ObservableCollection<BudgetViewModelData>();
            var budgetPlanned = MainBudget.Instance.GetCurrentMonthData().BudgetPlanned;
            budgetPlanned.PropertyChanged += UpdateSummary;
            foreach (BudgetPlannedCategory category in budgetPlanned.Categories)
            {
                foreach (PlannedSubcat subcat in category.subcats)
                {
                    var model = new BudgetViewModelData()
                    {
                        Category = category,
                        Subcat = subcat
                    };
                    plannedModel.Add(model);
                }
            }

            /*listView.GridStyle = new BudgetDataGridStyle();
            listView.ItemsSource = plannedModel;
            listView.HeaderRowHeight = 0;

            listView.Columns.Add(new GridTextColumn()
            {
                MappingName = "Subcat.Name",
                HeaderText = "Kategoria",
                HeaderTemplate = new DataTemplate(() =>
                {
                    Label label = new Label()
                    {
                        Text = "Kategoria",
                        //FontSize = 12,
                        TextColor = Color.Gray,
                        HorizontalOptions = LayoutOptions.Start,
                        VerticalOptions = LayoutOptions.Center
                    };
                    return label;
                }),
                //HeaderFont = "Cambria",
                //RecordFont = "Cambria",
                //CellTextSize = 12,
                
                ColumnSizer = ColumnSizer.Auto
            });

            listView.Columns.Add(new GridTextColumn()
            {
                MappingName = "Subcat.Value",
                HeaderText = "Suma",
                HeaderTemplate = new DataTemplate(() =>
                {
                    Label label = new Label()
                    {
                        Text = "Suma",
                        //FontSize = 12,
                        TextColor = Color.Gray,
                        HorizontalOptions = LayoutOptions.Start,
                        VerticalOptions = LayoutOptions.Center
                    };
                    return label;
                }),
                AllowEditing = true,
                ColumnSizer = ColumnSizer.LastColumnFill,
                //HeaderFont = "Cambria",
                //RecordFont = "Cambria",
                Format = "C",
                CultureInfo = new CultureInfo("pl-PL")
            });

            listView.GroupColumnDescriptions.Add(new GroupColumnDescription()
            {
                ColumnName = "Category.Name",
            });

            GridSummaryRow summaryRow = new GridSummaryRow
            {
                ShowSummaryInRow = true,
                Title = "{Key}: {Total}"
            };

            summaryRow.SummaryColumns.Add(new GridSummaryColumn
            {
                Name = "Total",
                //CustomAggregate = new CurrencyDataGridHeader(),
                MappingName = "Subcat.Value",
                //Format = "{Currency}",
                Format = "{Sum:c}",
                SummaryType = SummaryType.DoubleAggregate
                //SummaryType = SummaryType.Custom,

            });

            listView.CaptionSummaryRow = summaryRow;
            listView.GridViewCreated += (object sender, GridViewCreatedEventArgs e) =>
            {
                listView.View.LiveDataUpdateMode = LiveDataUpdateMode.AllowSummaryUpdate;
                listView.View.RecordPropertyChanged += (object recordSender, PropertyChangedEventArgs args) =>
                {
                    //UpdateSummary();
                    var recordentry = listView.View.Records.GetRecord(recordSender);
                    listView.View.TopLevelGroup.UpdateSummaries(recordentry.Parent as Group);
                };
            };

            listView.CurrentCellEndEdit += (object sender, GridCurrentCellEndEditEventArgs args) =>
            {
                MainBudget.Instance.Save();
            };*/
        }

        private void SetupTable()
        {
            plannedModel = new ObservableCollection<BudgetViewModelData>();
            var budgetPlanned = MainBudget.Instance.GetCurrentMonthData().BudgetPlanned;
            budgetPlanned.PropertyChanged += UpdateSummary;
            foreach (BudgetPlannedCategory category in budgetPlanned.Categories)
            {
                var categoryTable = new CategoryPlanDataGrid();
                categoryTable.Setup(category);
                tableLayout.Children.Add(categoryTable);
            }
        }

        private void SetupCharts()
        {
            var expensesData = new ObservableCollection<BudgetViewModelData>();
            var incomesData = new ObservableCollection<BudgetViewModelData>();
            var budgetPlanned = MainBudget.Instance.GetCurrentMonthData().BudgetPlanned;
            foreach (BudgetPlannedCategory category in budgetPlanned.Categories)
            {
                var model = new BudgetViewModelData
                {
                    Name = category.Name,
                    Category = category,
                };
                if (!category.IsIncome)
                    expensesData.Add(model);
            }

            var incomesCategories = budgetPlanned.GetIncomesCategories();
            foreach (BaseBudgetCategory category in incomesCategories)
            {
                foreach(BaseBudgetSubcat subcat in category.subcats)
                {
                    var model = new BudgetViewModelData
                    {
                        Name = subcat.Name,
                        Category = category,
                        Subcat = subcat as PlannedSubcat
                    };
                    incomesData.Add(model);
                }
            }

            SetupIncomesChart(incomesData);
            SetupExpensesChart(expensesData);
        }

        private void SetupIncomesChart(ObservableCollection<BudgetViewModelData> data)
        {
            SetupChart(chartIncome, data, "Name", "Subcat.Value");
        }

        private void SetupExpensesChart(ObservableCollection<BudgetViewModelData> data)
        {
            SetupChart(chartExpense, data, "Name", "Category.TotalValues");
        }

        private void SetupChart(SfChart chart, ObservableCollection<BudgetViewModelData> data, string xBindingPath, string yBindingPath)
        {
            var pieSeries = new PieSeries
            {
                ItemsSource = data,
                XBindingPath = xBindingPath,
                YBindingPath = yBindingPath,
                EnableSmartLabels = true,
                DataMarkerPosition = CircularSeriesDataMarkerPosition.OutsideExtended,
                ListenPropertyChange = true
            };

            var dataMarkerTemplate = new DataTemplate(() =>
            {
                var label = new Label();
                label.SetBinding(Label.TextProperty, "Thiz", BindingMode.Default, new ChartCategoryNameConverter());
                label.FontSize = 10;
                label.VerticalOptions = LayoutOptions.Center;
               
                return label;
            });

            pieSeries.DataMarker = new ChartDataMarker
            {
                LabelContent = LabelContent.YValue,
                LabelTemplate = dataMarkerTemplate
            };
            chart.Series.Add(pieSeries);
        }

        private void UpdateSummary(object sender, PropertyChangedEventArgs e)
        {
            var budgetMonth = MainBudget.Instance.GetCurrentMonthData();
            var monthExpensesPlanned = budgetMonth.GetTotalExpensesPlanned();
            var monthIncomePlanned = budgetMonth.GetTotalIncomePlanned();
            var diffPlanned = monthIncomePlanned - monthExpensesPlanned;

            var cultureInfoPL = new CultureInfo("pl-PL");
            plannedExpenses.Text = string.Format(cultureInfoPL, "{0:c}", monthExpensesPlanned);
            plannedIncomes.Text = string.Format(cultureInfoPL, "{0:c}", monthIncomePlanned);
            plannedDiff.Text = string.Format(cultureInfoPL, "{0:c}", diffPlanned);
        }

        private async void OnHomeClick(object sender, EventArgs args)
        { 
            await Navigation.PushModalAsync(new MainPagePC());
        }

        private async void OnAnalizeClick(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new AnalyticsPagePC());
        }

        private async void OnOnlyThisMonth(object sender, EventArgs args)
        {
            await MainBudget.Instance.Save();
        }

        private async void OnSaveForAll(object sender, EventArgs args)
        {
            await MainBudget.Instance.UpdateMainPlannedBudget();
        }
    }
}