using HomeBudget.Code;
using HomeBudget.Utils;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace HomeBudget.UWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AnalyticsPage : Page
    {
        ObservableCollection<BudgetViewModelData> Budget = new ObservableCollection<BudgetViewModelData>();

        public AnalyticsPage()
        {
            InitializeComponent();

            var budgetReal = MainBudget.Instance.GetCurrentMonthData().BudgetReal;

            foreach (var category in budgetReal.Categories)
            {
                foreach (var subcat in category.subcats)
                {
                    var model = new BudgetViewModelData
                    {
                        Category = category,
                        Subcat = subcat
                    };
                    Budget.Add(model);
                }
            }

            var gridTextColumn = new GridTextColumn
            {
                MappingName = "Subcat.Name",
                TextAlignment = TextAlignment.Start,
                //Padding = new Thickness(24, 0),

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
                //RecordFont = "Lato",
                //ColumnSizer = ColumnSizer.Auto
            };

            DataGrid.Columns.Add(gridTextColumn);
        }
    }
}
