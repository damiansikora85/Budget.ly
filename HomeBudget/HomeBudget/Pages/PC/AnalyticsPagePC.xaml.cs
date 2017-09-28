using HomeBudget.Code;
using Syncfusion.SfChart.XForms;
using System;
using System.Collections.Generic;
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

            chart.IsVisible = true;
            table.IsVisible = false;

            SetupChart();
        }

        private void SetupChart()
        {
            BarSeries barSeries = new BarSeries()
            {
                ItemsSource = Code.MainBudget.Instance.GetCurrentMonthChartData(),
                XBindingPath = "CategoryName",
                YBindingPath = "ExpensesSum"
            };

            chart.Series.Add(barSeries);
        }

        private void SetupTable()
        {
            BudgetDescription budget = MainBudget.Instance.BudgetDescription;
            BudgetMonth budgetMonth = MainBudget.Instance.GetCurrentMonthData();

            int rowIndex = 0;
           /* foreach (BudgetCategoryTemplate category in budget.Categories)
            { 

                ExpenseCategory budgetCategory = budgetMonth.GetCategory(category.Id);
                CreateCategoryTable(category, budgetCategory, grid22, rowIndex);

                rowIndex = rowIndex + 2 + category.subcategories.Count;
            }*/

            //table.Children.Add(grid);

        }

        /*private void CreateCategoryTable(BudgetCategoryTemplate categoryTemplate, ExpenseCategory category, Grid grid, int row)
        {

            Button button = new Button()
            {
                Text = category.Name
            };

            Grid.SetRow(button, row);
            Grid.SetColumn(button, 0);
            grid.Children.Add(button);

            int subcatID = 0;
            foreach (string subcat in categoryTemplate.subcategories)
            {
                Label subcatName = new Label()
                {
                    Text = subcat
                };
                Grid.SetColumn(subcatName, 0);
                Grid.SetRow(subcatName, row+1+subcatID);

                grid.Children.Add(subcatName);

                /*for (int i = 1; i < 32; i++)
                {
                    Label value = new Label()
                    {
                        Text = "0zł"
                    };
                    Grid.SetColumn(value, i);
                    Grid.SetRow(value, row + 1 + subcatID);
                    grid.Children.Add(value);
                }


                subcatID++;
            }

           
        }*/

        /*private Grid CreateSubcat(string name, ExpenseSubcat subcategory)
        {
            Grid grid = new Grid();

            ColumnDefinition col0 = new ColumnDefinition()
            {
                Width = new GridLength(3, GridUnitType.Star)
            };
            grid.ColumnDefinitions.Add(col0);

            RowDefinition row = new RowDefinition()
            {
                Height = new GridLength(1, GridUnitType.Star)
            };
            grid.RowDefinitions.Add(row);

            for (int i=0; i<31; i++)
            {
                ColumnDefinition col = new ColumnDefinition()
                {
                    Width = new GridLength(1, GridUnitType.Star)
                };
                grid.ColumnDefinitions.Add(col);
            }

            Label subcatName = new Label()
            {
                Text = name
            };
            Grid.SetColumn(subcatName, 0);
            Grid.SetRow(subcatName, 0);

            grid.Children.Add(subcatName);

            for(int i=1; i<32; i++)
            {
                Label value = new Label()
                {
                    Text = subcategory.GetExpenseDay(i-1) + "zł"
                };
                Grid.SetColumn(value, i);
                Grid.SetRow(value, 0);

                grid.Children.Add(value);
            }

            return grid;
        }*/

        void OnClick(object sender, EventArgs args)
        {

        }

        void OnHomeClick(object sender, EventArgs args)
        {
            NavigationPage mainPage = new NavigationPage(new MainPagePC());
            Navigation.PushModalAsync(mainPage);
        }

        private async void OnChart1(object sender, EventArgs e)
        {
            chart.IsVisible = true;
            table.IsVisible = false;

            chart.Series.Clear();
            BarSeries barSeries = new BarSeries()
            {
                ItemsSource = Code.MainBudget.Instance.GetCurrentMonthChartData(),
                XBindingPath = "CategoryName",
                YBindingPath = "ExpensesSum"
            };

            chart.Series.Add(barSeries);

        }

        private async void OnChart2(object sender, EventArgs e)
        {
            chart.IsVisible = true;
            table.IsVisible = false;

            chart.Series.Clear();
            PieSeries pieSeries = new PieSeries()
            {
                ItemsSource = Code.MainBudget.Instance.GetCurrentMonthChartData(),
                XBindingPath = "CategoryName",
                YBindingPath = "ExpensesSum",
                ExplodeAll = true
            };

            pieSeries.DataMarker = new ChartDataMarker();
            pieSeries.DataMarker.ShowLabel = true;
            pieSeries.DataMarker.LabelContent = LabelContent.Percentage;
            chart.Series.Add(pieSeries);

        }

        private async void OnChart3(object sender, EventArgs e)
        {
            chart.IsVisible = false;
            table.IsVisible = true;

            SetupTable();
        }
    }
}
