using Acr.UserDialogs;
using HomeBudget.Code;
using HomeBudget.Code.Logic;
using HomeBudget.Utils;
using Syncfusion.Data;
using Syncfusion.SfChart.XForms;
using Syncfusion.SfDataGrid.XForms;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeBudgeStandard.Views
{
    [XamlCompilation(XamlCompilationOptions.Skip)]
	public partial class BudgetRealView : ContentPage
	{
        public ObservableCollection<BudgetViewModelData> Budget { get; set; }
        public ObservableCollection<BudgetViewModelData> IncomesData { get; set; }
        public ObservableCollection<BudgetViewModelData> ExpensesData { get; set; }

        private bool _setupDone;

        private CultureInfo _cultureInfoPL = new CultureInfo("pl-PL");

        public string Date
        {
            get => DateTime.Now.ToString("MMMM yyyy", _cultureInfoPL);
        }

        public BudgetRealView ()
		{
            Budget = new ObservableCollection<BudgetViewModelData>();
            BindingContext = this;
            InitializeComponent ();
            MainBudget.Instance.onBudgetLoaded += Setup;
        }

        protected override void OnAppearing()
        {
            if (MainBudget.Instance.IsInitialized && !_setupDone)
            {
                UserDialogs.Instance.ShowLoading();
                Setup();
            }
            _setupDone = true;
        }

        private void Setup()
        {
            Task.Run(() =>
            {
                CreateCharts();
                CreateDataGrid();
                Device.BeginInvokeOnMainThread(() => UserDialogs.Instance.HideLoading());
            });
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
                if (!category.IsIncome)
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

            //OnPropertyChanged(nameof(ExpensesData));
            //OnPropertyChanged(nameof(IncomesData));
        }

        private void SetupChart(SfChart chart, ObservableCollection<BudgetViewModelData> data, string xBindingPath, string yBindingPath)
        {
            var pieSeries = new PieSeries()
            {
                ItemsSource = data,
                XBindingPath = xBindingPath,
                YBindingPath = yBindingPath,
                EnableSmartLabels = true,
                DataMarkerPosition = CircularSeriesDataMarkerPosition.OutsideExtended,
                ListenPropertyChange = true
            };

            /*DataTemplate dataMarkerTemplate = new DataTemplate(() =>
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
            };*/
            chart.Series.Add(pieSeries);
        }

        private void CreateDataGrid()
        {
            var budget = new ObservableCollection<BudgetViewModelData>();
            try
            {
                var budgetReal = MainBudget.Instance.GetCurrentMonthData().BudgetReal;
                

                foreach (var category in budgetReal.Categories)
                {
                    foreach (var subcat in category.subcats)
                    {
                        var model = new BudgetViewModelData
                        {
                            Category = category,
                            Subcat = subcat,
                            SubcatReal = subcat as RealSubcat
                        };
                        budget.Add(model);
                    }
                }

                DataGrid.CaptionSummaryRow = new GridSummaryRow()
                {
                    ShowSummaryInRow = true,
                    Title = "{Key}: {Total}",

                    SummaryColumns = new ObservableCollection<ISummaryColumn>
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
            }
            catch(Exception e)
            {
                 return;
            }

            Device.BeginInvokeOnMainThread(() =>
            {
                Budget = budget;
                DataGrid.ItemsSource = Budget;
                DataGrid.GroupColumnDescriptions.Add(new GroupColumnDescription { ColumnName = "Category.Name" });
            });
        }

        private void DataGrid_CurrentCellEndEdit(object sender, GridCurrentCellEndEditEventArgs e)
        {
            Task.Run(() => MainBudget.Instance.Save());

            Device.BeginInvokeOnMainThread(async () =>
            {
                await Task.Delay(100);
                DataGrid.View.TopLevelGroup.UpdateCaptionSummaries();
                DataGrid.View.Refresh();
            });
        }
    }
}