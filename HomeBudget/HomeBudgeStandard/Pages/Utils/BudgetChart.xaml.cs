using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeBudget.Utils
{
    public class ChartData
    {
        public string Label { get; set; }
        public double Value { get; set; }
        public Color Color { get; set; }
    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BudgetChart : Grid
    {
        public List<OxyPlot.OxyColor> Colors = new List<OxyPlot.OxyColor>
        {
            OxyPlot.OxyColor.Parse("#f1c40f"), OxyPlot.OxyColor.Parse("#2ecc71"), OxyPlot.OxyColor.Parse("#3498db"),
            OxyPlot.OxyColor.Parse("#9b59b6"), OxyPlot.OxyColor.Parse("#1abc9c"), OxyPlot.OxyColor.Parse("#d35400"),
            OxyPlot.OxyColor.Parse("#e74c3c"), OxyPlot.OxyColor.Parse("#34495e"), OxyPlot.OxyColor.Parse("#7f8c8d"),
            OxyPlot.OxyColor.Parse("#bdc3c7"), OxyPlot.OxyColor.Parse("#f39c12"), OxyPlot.OxyColor.Parse("#8e44ad")
        };

        public BudgetChart ()
		{
			InitializeComponent ();
        }

        public void SetData(List<ChartData> data)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                data.Sort((el1, el2) => el2.Value.CompareTo(el1.Value));
                var series = new PieSeries
                {
                    InsideLabelFormat = null,
                    OutsideLabelFormat = null,//"{2:f0}%",
                    StartAngle = 270
                };
                for(int i=0; i< data.Count; i++)
                {
                    series.Slices.Add(new PieSlice(data[i].Label, data[i].Value));
                    data[i].Color = ToColor(Colors[i]);
                }

                var model = new OxyPlot.PlotModel
                {
                    DefaultColors = Colors
                };
                model.Series.Add(series);
                chart.Model = model;

                legend.ItemsSource = data;
            });
        }

        private static Color ToColor(OxyPlot.OxyColor oxyColor) => Color.FromRgb(oxyColor.R, oxyColor.G, oxyColor.B);
        
	}
}