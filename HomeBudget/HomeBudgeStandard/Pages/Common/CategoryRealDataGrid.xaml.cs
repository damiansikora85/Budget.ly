using HomeBudget.Code.Logic;
using HomeBudget.Utils;
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

namespace HomeBudgeStandard.Pages.Common
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CategoryRealDataGrid : ContentView
	{
        private double _totalRowHeight;
        private BaseBudgetCategory _category;
        private ObservableCollection<BudgetViewModelData> realModel;

        public CategoryRealDataGrid ()
		{
			InitializeComponent ();
            SizeChanged += DataGrid_SizeChanged;
        }

        public void Setup(BaseBudgetCategory category)
        {
            _category = category;
            _category.PropertyChanged += OnValueChanged;

            realModel = new ObservableCollection<BudgetViewModelData>();
            var cultureInfoPL = new CultureInfo("pl-PL");
            Name.Text = $"{category.Name}: {string.Format(cultureInfoPL, "{0:c}", category.TotalValues)}";

            Icon.Source = $"Assets/Categories/{_category.IconName}";

            foreach (var subcat in category.subcats)
            {
                var model = new BudgetViewModelData
                {
                    Category = category,
                    Subcat = subcat
                };
                realModel.Add(model);
            }

            DataGrid.GridStyle = new BudgetDataGridStyle();
            DataGrid.ItemsSource = realModel;



            var gridTextColumn = new GridTextColumn()
            {
                MappingName = "Subcat.Name",
                TextAlignment = TextAlignment.Start,
                Padding = new Thickness(24, 0),

                HeaderTemplate = new DataTemplate(() =>
                {
                    var label = new Label
                    {
                        Text = "Kategoria",
                        //FontSize = 12,
                        TextColor = Color.Gray,
                        HorizontalTextAlignment = TextAlignment.Start,
                        VerticalOptions = LayoutOptions.Center
                    };
                    return label;
                }),
                RecordFont = "Lato",
                ColumnSizer = ColumnSizer.Auto
            };

            DataGrid.Columns.Add(gridTextColumn);


            var valueColumn = new GridTextColumn()
            {
                MappingName = "Subcat.Value",
                HeaderTemplate = new DataTemplate(() =>
                {
                    var label = new Label
                    {
                        Text = "Suma",
                        //FontSize = 12,
                        TextColor = Color.Gray,
                        HorizontalOptions = LayoutOptions.Start,
                        VerticalOptions = LayoutOptions.Center
                    };
                    return label;
                }),
                TextAlignment = TextAlignment.End,
                Padding = new Thickness(0, 0, 24, 0),
                AllowEditing = false,
                ColumnSizer = ColumnSizer.Auto,
                Format = "C",
                CultureInfo = new CultureInfo("pl-PL")
            };
            DataGrid.Columns.Add(valueColumn);

            Task.Run(() =>
            {
                for (int i = 0; i < 31; i++)
                {
                    var header = (i + 1).ToString();
                    var column = new GridTextColumn
                    {
                        MappingName = "Subcat.Values[" + i.ToString() + "].Value",
                        ColumnSizer = ColumnSizer.LastColumnFill,
                        AllowEditing = true,
                        Format = "C",
                        RecordFont = "Lato",
                        CultureInfo = new CultureInfo("pl-PL"),
                        HeaderTemplate = new DataTemplate(() =>
                        {
                            var label = new Label()
                            {
                                Text = header,
                                FontSize = 12,
                                TextColor = Color.Gray,
                                HorizontalOptions = LayoutOptions.Start,
                                VerticalOptions = LayoutOptions.Center
                            };
                            return label;
                        }),
                    };
                    Device.BeginInvokeOnMainThread(() => DataGrid.Columns.Add(column));
                }
            });
        }

        private void OnValueChanged(object sender, PropertyChangedEventArgs e)
        {
            var cultureInfoPL = new CultureInfo("pl-PL");
            Name.Text = $"{_category.Name}: {string.Format(cultureInfoPL, "{0:c}", _category.TotalValues)}";
        }

        private void OnHide(object sender, EventArgs args)
        {
            DataGridView.IsVisible = !DataGridView.IsVisible;
            ((Button)sender).Image = DataGridView.IsVisible ? "Assets/chevron-up.png" : "Assets/chevron-down.png";
        }

        private void DataGrid_SizeChanged(object sender, EventArgs e)
        {
            if (_totalRowHeight == 0)
            {
                for (int i = 0; i <= DataGrid.View.Records.Count; i++)
                    _totalRowHeight += DataGrid.RowHeight;

                DataGridView.HeightRequest = _totalRowHeight;
            }
        }
    }
}