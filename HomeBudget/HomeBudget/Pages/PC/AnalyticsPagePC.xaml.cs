using HomeBudget.Code;
using HomeBudget.Code.Logic;
using HomeBudget.Pages.PC;
using HomeBudget.Utils;
using Syncfusion.Data;
using Syncfusion.SfChart.XForms;
using Syncfusion.SfDataGrid.XForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeBudget
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AnalyticsPagePC : ContentPage
    {
        public AnalyticsPagePC()
        {
            InitializeComponent();
            SetupDataGrid();
            SetupCharts();
        }

        private void SetupDataGrid()
        {
            ObservableCollection<BudgetViewModelData> data = new ObservableCollection<BudgetViewModelData>();
            BudgetReal budgetReal = MainBudget.Instance.GetCurrentMonthData().BudgetReal;
            foreach (BudgetRealCategory category in budgetReal.Categories)
            {
                foreach (RealSubcat subcat in category.subcats)
                {
                    BudgetViewModelData model = new BudgetViewModelData()
                    {
                        Category = category,
                        SubcatReal = subcat,
                        Subcat = subcat
                    };
                    data.Add(model);
                }
            }

            listView.GridStyle = new BudgetDataGridStyle();
            listView.ItemsSource = data;

            listView.Columns.Add(new GridTextColumn()
            {
                MappingName = "Subcat.Name",
                HeaderText = "Kategoria",
                HeaderFont = "Cambria",
                RecordFont = "Cambria",
                ColumnSizer = ColumnSizer.Auto
            });

            Style cellStyle = new Style(typeof(GridCell))
            {
                Setters = 
                {
                    new Setter { Property = Label.BackgroundColorProperty, Value = Color.FromHex("D2F3DF")  }
                }
            };
            listView.Columns.Add(new GridTextColumn()
            {
                MappingName = "Subcat.Value",
                HeaderText = "Suma",
                HeaderFont = "Cambria",
                ColumnSizer = ColumnSizer.Auto,
                Format = "C",
                RecordFont = "Cambria",
                CultureInfo = new CultureInfo("pl-PL"),
                CellStyle = cellStyle
            });

            for(int i=0; i<31; i++)
            {
                listView.Columns.Add(new GridTextColumn()
                {
                    MappingName = "SubcatReal.Values["+i.ToString()+"].Value",
                    HeaderText = (i+1).ToString(),
                    HeaderFont = "Cambria",
                    ColumnSizer = ColumnSizer.LastColumnFill,
                    AllowEditing = true,
                    Format = "C",
                    RecordFont = "Cambria",
                    CultureInfo = new CultureInfo("pl-PL")
                });
            }

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
                CustomAggregate = new CurrencyDataGridHeader(),
                MappingName = "Subcat.Value",
                Format = "{Currency}",
                SummaryType = Syncfusion.Data.SummaryType.Custom,
                
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
                //MainBudget.Instance.Save();
            };
        }

        private void SetupCharts()
        {
            ObservableCollection<BudgetViewModelData> expensesData = new ObservableCollection<BudgetViewModelData>();
            ObservableCollection<BudgetViewModelData> incomesData = new ObservableCollection<BudgetViewModelData>();
            BudgetReal budgetReal = MainBudget.Instance.GetCurrentMonthData().BudgetReal;
            foreach (BudgetRealCategory category in budgetReal.Categories)
            {
                BudgetViewModelData model = new BudgetViewModelData()
                {
                    Name = category.Name,
                    Category = category,
                };
                if (category.IsIncome == false)
                    expensesData.Add(model);
            }

            List<BaseBudgetCategory> incomesCategories = budgetReal.GetIncomesCategories();
            foreach (BaseBudgetCategory category in incomesCategories)
            {
                foreach (BaseBudgetSubcat subcat in category.subcats)
                {
                    BudgetViewModelData model = new BudgetViewModelData()
                    {
                        Name = subcat.Name,
                        Category = category,
                        Subcat = subcat as RealSubcat
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
            PieSeries pieSeries = new PieSeries()
            {
                ItemsSource = data,
                XBindingPath = xBindingPath,
                YBindingPath = yBindingPath,
                EnableSmartLabels = true,
                DataMarkerPosition = CircularSeriesDataMarkerPosition.OutsideExtended,
                ListenPropertyChange = true
            };

            DataTemplate dataMarkerTemplate = new DataTemplate(() =>
            {
                Label label = new Label();
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

        private async void OnHomeClick(object sender, EventArgs args)
        {
            await Navigation.PushModalAsync(new MainPagePC());
        }

        private async void OnPlanClick(object sender, EventArgs args)
        {
            await Navigation.PushModalAsync(new PlanningPage());
        }
    }
}
