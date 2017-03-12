using Syncfusion.SfChart.XForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace HomeBudget
{
	public partial class NewPage : ContentPage
	{
		public NewPage()
		{
			InitializeComponent();

            BarSeries barSeries = new BarSeries()
            {
                ItemsSource = Code.MainBudget.Instance.GeCurrentMonthData(),
                XBindingPath = "CategoryName",
                YBindingPath = "ExpensesSum"
            };
            chart.Series.Add(barSeries);
            /*chart.Series.Add(new ColumnSeries()
			{

				ItemsSource = Code.MainBudget.Instance.GeCurrentMonthData()

			});*/
		}

		private async void OnClick(object sender, EventArgs e)
		{

		}

		
	}
}
