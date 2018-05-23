using HomeBudget.Code;
using HomeBudget.Code.Logic;
using HomeBudget.Utils;
using Syncfusion.Data;
using Syncfusion.UI.Xaml.Charts;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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
    public sealed partial class AnalyticsPage : Page
    {
        private ObservableCollection<BudgetViewModelData> Budget = new ObservableCollection<BudgetViewModelData>();
        public ObservableCollection<BudgetViewModelData> IncomesData { get; set; }
        public ObservableCollection<BudgetViewModelData> ExpensesData { get; set; }

        private CultureInfo _cultureInfoPL = new CultureInfo("pl-PL");

        public string Date
        {
            get => DateTime.Now.ToString("MMMM yyyy", _cultureInfoPL);
        }

        public AnalyticsPage()
        {
            InitializeComponent();

            CreateCharts();
            CreateDataGrid();
        }

        private void CreateCharts()
        {
            ExpensesData = new ObservableCollection<BudgetViewModelData>();
            IncomesData = new ObservableCollection<BudgetViewModelData>();
            var budgetReal = MainBudget.Instance.GetCurrentMonthData().BudgetReal;
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
            var budgetReal = MainBudget.Instance.GetCurrentMonthData().BudgetReal;

            foreach (var category in budgetReal.Categories)
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
                DataGrid.View.TopLevelGroup.UpdateCaptionSummaries();
                DataGrid.View.Refresh();
            });
        }

        private void PreviousMonth_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NextMonth_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
