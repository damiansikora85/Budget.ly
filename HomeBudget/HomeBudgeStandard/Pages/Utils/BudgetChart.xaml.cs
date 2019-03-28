using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeBudget.Utils
{
    public class ChartData
    {
        public string Label { get; set; }
        public double Value { get; set; }
        public Color Color { get; set; }
        public double Percentage { get; set; }
        public string LabelAndPercentage => $"{Label} ({Math.Round(Percentage*100, 1)}%)";
    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BudgetChart : Grid
    {
        public enum LegendPositionEnum
        {
            LeftSide,
            RightSide,
            Top,
            Bottom
        }

        public static BindableProperty LegendPositionProperty = BindableProperty.Create(nameof(LegendPosition), typeof(LegendPositionEnum), typeof(BudgetChart), LegendPositionEnum.Bottom);

        public LegendPositionEnum LegendPosition
        {
            get => (LegendPositionEnum)GetValue(LegendPositionProperty);
            set => SetValue(LegendPositionProperty, value);
        }

        public List<OxyPlot.OxyColor> Colors = new List<OxyPlot.OxyColor>
        {
            OxyPlot.OxyColor.Parse("#5CBAE6"), OxyPlot.OxyColor.Parse("#B6D957"), OxyPlot.OxyColor.Parse("#FAC364"),
            OxyPlot.OxyColor.Parse("#8CD3FF"), OxyPlot.OxyColor.Parse("#D998CB"), OxyPlot.OxyColor.Parse("#F2D249"),
            OxyPlot.OxyColor.Parse("#93B9C6"), OxyPlot.OxyColor.Parse("#CCC5A8"), OxyPlot.OxyColor.Parse("#D32030"),
            OxyPlot.OxyColor.Parse("#DBDB46"), OxyPlot.OxyColor.Parse("#98AAFB"), OxyPlot.OxyColor.Parse("#8e44ad")
        };

        private bool _isGridCreated;
        private View _legendView;

        public BudgetChart ()
		{
			InitializeComponent ();
            BindingContext = this;
        }

        public void SetData(List<ChartData> data)
        {
            if (!_isGridCreated)
                CreateGrid();

            Device.BeginInvokeOnMainThread(() =>
            {
                data.Sort((el1, el2) => el2.Value.CompareTo(el1.Value));
                var series = new PieSeries
                {
                    InsideLabelFormat = null,//"{2:f0}%",
                    //OutsideLabelFormat = "{1}({2:f0}%)",//"{2:f0}%",
                    OutsideLabelFormat = null,
                    StartAngle = 270,
                    Diameter = 0.90,
                    InnerDiameter = 0.5,
                    InsideLabelPosition = 0.8,
                };

                var model = new OxyPlot.PlotModel();

                if (data.Sum(el => el.Value) == 0)
                {
                    series.Slices.Add(new PieSlice("", 1));
                    series.InsideLabelFormat = "{1}";
                    model.DefaultColors = new List<OxyPlot.OxyColor> { OxyPlot.OxyColor.Parse("#ACACAC") };
                    noDataLabel.IsVisible = true;
                    _legendView.IsVisible = false;
                }
                else
                {
                    for (int i = 0; i < data.Count; i++)
                    {
                        series.Slices.Add(new PieSlice(data[i].Label, data[i].Value));
                        data[i].Color = ToColor(Colors[i]);
                    }
                    model.DefaultColors = Colors;
                    SetupLegend(data);

                    noDataLabel.IsVisible = false;
                }

                model.Series.Add(series);
                chart.Model = model;
            });
        }

        private void CreateGrid()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (LegendPosition == LegendPositionEnum.Bottom || LegendPosition == LegendPositionEnum.Top)
                {
                    this.RowDefinitions.Add(new RowDefinition { Height = new GridLength(5, GridUnitType.Star) });
                    this.RowDefinitions.Add(new RowDefinition { Height = new GridLength(4, GridUnitType.Star) });
                    CreateLegendAsGrid();
                }
                else
                {
                    this.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    this.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    CreateLegendAsList();
                }
#pragma warning disable CC0120 // Your Switch maybe include default clause
                switch (LegendPosition)
                {
                    case LegendPositionEnum.LeftSide:
                        Grid.SetColumn(chart, 1);
                        Grid.SetColumn(_legendView, 0);
                        Grid.SetColumn(noDataLabel, 0);
                        break;
                    case LegendPositionEnum.RightSide:
                        Grid.SetColumn(chart, 0);
                        Grid.SetColumn(_legendView, 1);
                        Grid.SetColumn(noDataLabel, 1);
                        break;
                    case LegendPositionEnum.Top:
                        Grid.SetRow(noDataLabel, 0);
                        Grid.SetRow(_legendView, 0);
                        Grid.SetRow(chart, 1);
                        break;
                    case LegendPositionEnum.Bottom:
                        Grid.SetRow(noDataLabel, 1);
                        Grid.SetRow(_legendView, 1);
                        Grid.SetRow(chart, 0);
                        break;
                }
#pragma warning restore CC0120 // Your Switch maybe include default clause
                Children.Add(_legendView);
                _isGridCreated = true;
            });
        }

        private void CreateLegendAsGrid()
        {
            var legendGrid = new Grid();
            legendGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            legendGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

            legendGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            legendGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            legendGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            legendGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            legendGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            legendGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            legendGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });

            _legendView = legendGrid;
        }

        private void CreateLegendAsList()
        {
            var listView = new ListView
            {
                RowHeight = 35,
                ItemTemplate = new DataTemplate(() =>
                  {
                      var viewcell = new ViewCell();
                      var layout = new StackLayout { Orientation = StackOrientation.Horizontal };
                      var box = new BoxView { WidthRequest = 20, Margin = new Thickness(0, 11) };
                      box.SetBinding(BoxView.ColorProperty, new Binding("Color"));
                      var label = new Label { FontSize = 12, VerticalTextAlignment = TextAlignment.Center };
                      label.SetBinding(Label.TextProperty, new Binding("LabelAndPercentage"));

                      layout.Children.Add(box);
                      layout.Children.Add(label);

                      viewcell.View = layout;
                      return viewcell;
                  })
            };

            _legendView = listView;
        }

        private void SetupLegend(List<ChartData> data)
        {
            _legendView.IsVisible = true;
            if (LegendPosition == LegendPositionEnum.LeftSide || LegendPosition == LegendPositionEnum.RightSide)
            {
                if (_legendView is ListView legendList)
                {
                    legendList.ItemsSource = data;
                }
            }
            else if(_legendView is Grid legendGrid)
            {
                legendGrid.Children.Clear();
                for (var i = 0; i < data.Count; i++)
                {
                    var layout = new StackLayout { Orientation = StackOrientation.Horizontal, HeightRequest = 20 };
                    var box = new BoxView { Color = data[i].Color, WidthRequest = 10, HeightRequest = 10 };
                    var label = new Label { Text = $"{data[i].Label} ({Math.Round(data[i].Percentage*100, 1)}%)", FontSize = 12 };
                    layout.Children.Add(box);
                    layout.Children.Add(label);
                    Grid.SetColumn(layout, i % 2);
                    Grid.SetRow(layout, i / 2);
                    legendGrid.Children.Add(layout);
                }
            }
        }

        private static Color ToColor(OxyPlot.OxyColor oxyColor) => Color.FromRgb(oxyColor.R, oxyColor.G, oxyColor.B);
    }
}