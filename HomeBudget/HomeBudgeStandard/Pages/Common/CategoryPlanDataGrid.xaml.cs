using HomeBudget.Code.Logic;
using HomeBudget.Utils;
using Syncfusion.SfDataGrid.XForms;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeBudgeStandard.Pages.Common
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CategoryPlanDataGrid : ContentView
	{
        private double _totalRowHeight;
        private BudgetPlannedCategory _category;
        private ObservableCollection<BudgetViewModelData> plannedModel;

        public CategoryPlanDataGrid ()
		{
			InitializeComponent ();
            SizeChanged += DataGrid_SizeChanged;
        }

        private void OnEditCompleted(object sender, GridCurrentCellEndEditEventArgs e)
        {
            if(!e.Cancel)
            {
                var cultureInfoPL = new CultureInfo("pl-PL");
                var ccc = plannedModel[0].Category;
                Name.Text = $"{_category.Name}: {string.Format(cultureInfoPL, "{0:c}", _category.TotalValues)}";
            }
        }

        public void Setup(BudgetPlannedCategory category)
        {
            _category = category;
            _category.PropertyChanged += OnValueChanged;

            plannedModel = new ObservableCollection<BudgetViewModelData>();
            var cultureInfoPL = new CultureInfo("pl-PL");
            Name.Text = $"{category.Name}: {string.Format(cultureInfoPL, "{0:c}", category.TotalValues)}";

            Icon.Source = $"Assets/Categories/{_category.IconName}";

            foreach(var subcat in category.subcats)
            {
                var model = new BudgetViewModelData
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

            using (var gridTextColumn = new GridTextColumn
            {
                MappingName = "Subcat.Name",
                TextAlignment = TextAlignment.Start,
                Padding = new Thickness(24, 0),
                
                /*HeaderTemplate = new DataTemplate(() =>
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
                }),*/
                RecordFont = "Lato",
                ColumnSizer = ColumnSizer.Auto
            })

            DataGrid.Columns.Add(gridTextColumn);


            using (var gridTextColumn = new GridTextColumn
            {
                MappingName = "Subcat.Value",
                /*HeaderTemplate = new DataTemplate(() =>
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
                }),*/
                TextAlignment = TextAlignment.End,
                Padding = new Thickness(0,0,24,0),
                AllowEditing = true,
                ColumnSizer = ColumnSizer.LastColumnFill,
                RecordFont = "Lato",
                Format = "C",
                CultureInfo = new CultureInfo("pl-PL")
            })
            DataGrid.Columns.Add(gridTextColumn);
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
                for (int i = 0; i < DataGrid.View.Records.Count; i++)
                    _totalRowHeight += DataGrid.RowHeight;

                DataGridView.HeightRequest = _totalRowHeight;//Math.Min(totalRowHeight, DataGrid.Height);
            }
        }
    }
}