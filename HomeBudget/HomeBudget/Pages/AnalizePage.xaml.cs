using Syncfusion.SfChart.XForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeBudget
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AnalizePage : ContentPage
    {
        public AnalizePage()
        {
            InitializeComponent();
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

            //chart.Series.Add(barSeries);
        }

        private async void OnBack(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void OnChart1(object sender, EventArgs e)
        {
            /*chart.Series.Clear();
            BarSeries barSeries = new BarSeries()
            {
                ItemsSource = Code.MainBudget.Instance.GetCurrentMonthChartData(),
                XBindingPath = "CategoryName",
                YBindingPath = "ExpensesSum"
            };

            chart.Series.Add(barSeries);*/

        }

        private async void OnChart2(object sender, EventArgs e)
        {
            /*chart.Series.Clear();
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
            chart.Series.Add(pieSeries);*/

        }

        private async void OnChart3(object sender, EventArgs e)
        {


        }
    }

}
