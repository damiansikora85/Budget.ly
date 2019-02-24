using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            OxyPlot.OxyColor.Parse("#5CBAE6"), OxyPlot.OxyColor.Parse("#B6D957"), OxyPlot.OxyColor.Parse("#FAC364"),
            OxyPlot.OxyColor.Parse("#8CD3FF"), OxyPlot.OxyColor.Parse("#D998CB"), OxyPlot.OxyColor.Parse("#F2D249"),
            OxyPlot.OxyColor.Parse("#93B9C6"), OxyPlot.OxyColor.Parse("#CCC5A8"), OxyPlot.OxyColor.Parse("#D32030"),
            OxyPlot.OxyColor.Parse("#DBDB46"), OxyPlot.OxyColor.Parse("#98AAFB"), OxyPlot.OxyColor.Parse("#8e44ad")
        };

        public BudgetChart ()
		{
			InitializeComponent ();
            BindingContext = this;
        }

        public void SetData(List<ChartData> data)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                data.Sort((el1, el2) => el2.Value.CompareTo(el1.Value));
                var series = new PieSeries
                {
                    InsideLabelFormat = "{2:f0}%",
                    //OutsideLabelFormat = "{1}({2:f0}%)",//"{2:f0}%",
                    OutsideLabelFormat = null,
                    StartAngle = 270,
                    Diameter=0.90,
                    InnerDiameter = 0.5,
                    InsideLabelPosition = 1.2
                };

                var model = new OxyPlot.PlotModel();

                if (data.Sum(el => el.Value) == 0)
                {
                    series.Slices.Add(new PieSlice("", 1));
                    series.InsideLabelFormat = "{1}";
                    model.DefaultColors = new List<OxyPlot.OxyColor> { OxyPlot.OxyColor.Parse("#ACACAC") };
                    legend.IsVisible = false;
                    noDataLabel.IsVisible = true;
                }
                else
                {
                    for (int i = 0; i < data.Count; i++)
                    {
                        series.Slices.Add(new PieSlice(data[i].Label, data[i].Value));
                        data[i].Color = ToColor(Colors[i]);
                    }
                    model.DefaultColors = Colors;
                    //legend.ItemsSource = data;
                    CreateLegend(data);

                    legend.IsVisible = true;
                    noDataLabel.IsVisible = false;
                }

                model.Series.Add(series);
                chart.Model = model;
            });
        }

        private void CreateLegend(List<ChartData> data)
        {
            legend.Children.Clear();
            for(var i=0; i<data.Count; i++)
            {
                var layout = new StackLayout { Orientation = StackOrientation.Horizontal, HeightRequest = 20 };
                var box = new BoxView { Color = data[i].Color, WidthRequest = 10, HeightRequest = 10 };
                var label = new Label { Text = data[i].Label, FontSize = 12 };
                layout.Children.Add(box);
                layout.Children.Add(label);
                Grid.SetColumn(layout, i%2);
                Grid.SetRow(layout, i/2);
                legend.Children.Add(layout);
            }
        }

        private static Color ToColor(OxyPlot.OxyColor oxyColor) => Color.FromRgb(oxyColor.R, oxyColor.G, oxyColor.B);
	}
}