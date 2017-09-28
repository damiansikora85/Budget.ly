using HomeBudget.Code;
using HomeBudget.Code.Logic;
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
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.Specialized;
using HomeBudget.Code.Logic.Temp;

namespace HomeBudget.Pages.PC
{
    public class BudgetPlannedModel : INotifyPropertyChanged
    {
        public string CategoryName { get; set; }
        private PlannedSubcat subcat;
        public PlannedSubcat Subcat
        {
            get { return subcat; }
            set
            {
                subcat = value;
                subcat.PropertyChanged += (object sender, PropertyChangedEventArgs args) =>
                {
                    RaisePropertyChanged("Subcat.Value");
                };
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }

    public class BudgetCategoryOverallData : INotifyPropertyChanged
    {
        public string Name { get; set; }
        //public double Value { get; set; }
        private BaseBudgetCategory category;
        public BaseBudgetCategory Category
        {
            get { return category; }
            set
            {
                category = value;
                category.onSubcatChanged += OnChanged;
            }
        }
        public BudgetCategoryOverallData Thiz { get { return this; } }

        private void OnChanged()
        {
            RaisePropertyChanged("Category");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }

    public class BudgetCategoryOverallIncomesData : INotifyPropertyChanged
    {
        public string Name { get; set; }
        private PlannedSubcat subcat;
        public PlannedSubcat Subcat
        {
            get { return subcat; }
            set
            {
                subcat = value;
                subcat.PropertyChanged += OnChanged;
            }
        }
        public BudgetCategoryOverallIncomesData Thiz { get { return this; } }

        private void OnChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged("Subcat");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlanningPage : ContentPage
    {
        private int selectedCategory;
        private int selectedSubcat;

        public PlanningPage()
        {
            InitializeComponent();
            UpdateSummary();
            //MainBudget.Instance.onPlannedBudgetChanged += OnPlannedBudgetChanged;

            SetupDataGrid();
            SetupChart(chartIncome, GetIncomesData());
            SetupChart(chartExpense, GetExpensesData());
        }

        private void SetupDataGrid()
        {
            ObservableCollection<BudgetPlannedModel> plannedModel = new ObservableCollection<BudgetPlannedModel>();
            BudgetPlanned budgetPlanned = MainBudget.Instance.GetCurrentMonthData().BudgetPlanned;
            foreach (BudgetPlannedCategory category in budgetPlanned.Categories)
            {
                foreach (PlannedSubcat subcat in category.subcats)
                {
                    BudgetPlannedModel model = new BudgetPlannedModel()
                    {
                        CategoryName = category.Name,
                        Subcat = subcat
                    };
                    plannedModel.Add(model);
                }
            }

            listView.GridStyle = new BudgetDataGridStyle();
            listView.ItemsSource = plannedModel;
            listView.HeaderRowHeight = 0;

            listView.Columns.Add(new GridTextColumn()
            {
                MappingName = "Subcat.Name",
                HeaderText = "Kategoria",
                ColumnSizer = ColumnSizer.Auto
            });

            listView.Columns.Add(new GridTextColumn()
            {
                MappingName = "Subcat.Value",
                HeaderText = "Suma",
                AllowEditing = true,
                ColumnSizer = ColumnSizer.LastColumnFill,
                Format = "C",
                CultureInfo = new CultureInfo("pl-PL")
            });

            listView.GroupColumnDescriptions.Add(new GroupColumnDescription()
            {
                ColumnName = "CategoryName",
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
                    UpdateSummary();
                    var recordentry = listView.View.Records.GetRecord(recordSender);
                    listView.View.TopLevelGroup.UpdateSummaries(recordentry.Parent as Group);
                };
            };

           
        }

        private void RecordChanged(object sender, PropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private ObservableCollection<BudgetCategoryOverallData> GetExpensesData()
        {
            BudgetPlanned budgetPlanned = MainBudget.Instance.GetCurrentMonthData().BudgetPlanned;
            ObservableCollection<BudgetCategoryOverallData> chartData = new ObservableCollection<BudgetCategoryOverallData>();
            foreach (BudgetPlannedCategory category in budgetPlanned.Categories)
            {
                if (category.IsIncome == false)
                {
                    BudgetCategoryOverallData data = new BudgetCategoryOverallData()
                    {
                        Name = category.Name,
                        //Value = decimal.ToDouble(category.GetTotalValues())
                        Category = category
                    };
                    chartData.Add(data);
                }
            }
            SetupChart(chartExpense, chartData);
            return chartData;
        }

        private ObservableCollection<BudgetCategoryOverallIncomesData> GetIncomesData()
        {
            BudgetPlanned budgetPlanned = MainBudget.Instance.GetCurrentMonthData().BudgetPlanned;
            ObservableCollection<BudgetCategoryOverallIncomesData> chartData = new ObservableCollection<BudgetCategoryOverallIncomesData>();
            BudgetPlannedCategory incomeCategory = budgetPlanned.GetIncomesCategories()[0];

            foreach (PlannedSubcat category in incomeCategory.subcats)
            {
                BudgetCategoryOverallIncomesData data = new BudgetCategoryOverallIncomesData()
                {
                    Name = category.Name,
                    //Value = category.Value
                    Subcat = category
                };
                chartData.Add(data);
            }
            return chartData;
        }

        private void SetupChart(SfChart chart, ObservableCollection<BudgetCategoryOverallIncomesData> data)
        {
            PieSeries pieSeries = new PieSeries()
            {
                ItemsSource = data,
                XBindingPath = "Name",
                YBindingPath = "Subcat.Value",
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

            //chart.Series.CollectionChanged += OnDataGridChanged;
            chart.Series.Clear();
            chart.Series.Add(pieSeries);
        }

        private void SetupChart(SfChart chart, ObservableCollection<BudgetCategoryOverallData> data)
        {
            PieSeries pieSeries = new PieSeries()
            {
                ItemsSource = data,
                XBindingPath = "Name",
                YBindingPath = "Category.TotalValues",
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

            chart.Series.Clear();
            chart.Series.Add(pieSeries);
        }

        private void UpdateSummary()
        {
            BudgetMonth budgetMonth = MainBudget.Instance.GetCurrentMonthData();
            double monthExpensesPlanned = budgetMonth.GetTotalExpensesPlanned();
            double monthIncomePlanned = budgetMonth.GetTotalIncomePlanned();
            double diffPlanned = monthIncomePlanned - monthExpensesPlanned;

            CultureInfo cultureInfoPL = new CultureInfo("pl-PL");
            plannedExpenses.Text = string.Format(cultureInfoPL, "{0:c}", monthExpensesPlanned);
            plannedIncomes.Text = string.Format(cultureInfoPL, "{0:c}", monthIncomePlanned);
            plannedDiff.Text = string.Format(cultureInfoPL, "{0:c}", diffPlanned);
        }
        
        private void OnPlannedBudgetChanged()
        {
            UpdateSummary();
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