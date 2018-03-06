using HomeBudget.Code.Logic;
using HomeBudget.Utils;
using Syncfusion.SfDataGrid.XForms;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeBudgeStandard.Pages.Common
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CategoryPlanDataGrid : ContentView
	{
        private double totalRowHeight = 0;

        public CategoryPlanDataGrid ()
		{
			InitializeComponent ();
            SizeChanged += DataGrid_SizeChanged;
        }

        public void Setup(BudgetPlannedCategory category)
        {
            var plannedModel = new ObservableCollection<BudgetViewModelData>();
            CultureInfo cultureInfoPL = new CultureInfo("pl-PL");
            Name.Text = $"{category.Name}: {string.Format(cultureInfoPL, "{0:c}", category.TotalValues)}";
            foreach(var subcat in category.subcats)
            {
                BudgetViewModelData model = new BudgetViewModelData()
                {
                    Category = category,
                    Subcat = subcat
                };
                plannedModel.Add(model);
            }

            DataGrid.GridStyle = new BudgetDataGridStyle();
            DataGrid.ItemsSource = plannedModel;
            DataGrid.HeaderRowHeight = 0;
            //HeightRequest = category.subcats.Count * 25;

            DataGrid.Columns.Add(new GridTextColumn()
            {
                MappingName = "Subcat.Name",
                TextAlignment = TextAlignment.Start,
                HeaderText = "Kategoria",
                Padding = new Thickness(24, 0),
                HeaderTemplate = new DataTemplate(() =>
                {
                    Label label = new Label()
                    {
                        Text = "Kategoria",
                        //FontSize = 12,
                        TextColor = Color.Gray,
                        HorizontalTextAlignment = TextAlignment.Start,
                        VerticalOptions = LayoutOptions.Center
                    };
                    return label;
                }),
                //HeaderFont = "Cambria",
                //RecordFont = "Cambria",
                //CellTextSize = 12,

                ColumnSizer = ColumnSizer.Auto
            });

            DataGrid.Columns.Add(new GridTextColumn()
            {
                MappingName = "Subcat.Value",
                HeaderText = "Suma",
                HeaderTemplate = new DataTemplate(() =>
                {
                    Label label = new Label()
                    {
                        Text = "Suma",
                        //FontSize = 12,
                        TextColor = Color.Gray,
                        HorizontalOptions = LayoutOptions.Start,
                        VerticalOptions = LayoutOptions.Center
                    };
                    return label;
                }),
                AllowEditing = true,
                ColumnSizer = ColumnSizer.LastColumnFill,
                //HeaderFont = "Cambria",
                //RecordFont = "Cambria",
                Format = "C",
                CultureInfo = new CultureInfo("pl-PL")
            });
        }

        private void OnHide(object sender, EventArgs args)
        {
            DataGrid.IsVisible = !DataGrid.IsVisible;
            ((Button)sender).Text = DataGrid.IsVisible ? "Hide" : "Show";
        }
    
        private void DataGrid_SizeChanged(object sender, EventArgs e)
        {
            if (totalRowHeight == 0)
            {
                for (int i = 0; i <= DataGrid.View.Records.Count; i++)
                    totalRowHeight += DataGrid.RowHeight;

                /*for (int i = 0; i < this.DataGrid.Columns.Count; i++)
                    totalColumnWidth += DataGrid.Columns[i].ActualWidth + (DataGrid.ShowRowHeader ? DataGrid.RowHeaderWidth : 0);*/
                //contentView.WidthRequest = Math.Min(totalColumnWidth, DataGrid.Width);
                DataGrid.HeightRequest = totalRowHeight;//Math.Min(totalRowHeight, DataGrid.Height);
            }
        }
    }
}