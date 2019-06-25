using HomeBudgeStandard.Views;
using HomeBudget.Code;
using HomeBudget.Converters;
using HomeBudget.Utils;
using Syncfusion.Data;
using Syncfusion.SfDataGrid.XForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeBudgeStandard.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BudgetTemplateEditPage : ContentPage
    {
        private SfDataGrid _dataGrid;
        public List<BudgetCategoryForEdit> BudgetTemplate { get; private set; }

        public BudgetTemplateEditPage()
        {
            InitializeComponent();

            BudgetTemplate = MainBudget.Instance.GetCurrentMonthData().GetBudgetTemplateEdit();
            listView.ItemsSource = BudgetTemplate;
            //_dataGrid = CreateDataGrid();
            //_dataGrid.ItemsSource = BudgetTemplate;
            //mainLayout.Children.Add(_dataGrid);
        }

        private SfDataGrid CreateDataGrid()
        {
            var dataGrid = new SfDataGrid
            {
                EnableDataVirtualization = true,
                AutoGenerateColumns = false,
                AutoExpandGroups = false,
                AllowGroupExpandCollapse = true,
                LiveDataUpdateMode = LiveDataUpdateMode.AllowSummaryUpdate,
                SelectionMode = SelectionMode.SingleDeselect,
                NavigationMode = NavigationMode.Cell,
                EditTapAction = TapAction.OnTap,
                GridStyle = new BudgetDataGridStyle(),
                Margin = new Thickness(12, 0),
                AllowEditing = true
            };

            dataGrid.CurrentCellEndEdit += CategoryEditEnd;

            /*dataGrid.SortComparers.Add(new SortComparer
            {
                PropertyName = "Category.Name",
                Comparer = new BudgetCategorySortComparer()
            });*/

            dataGrid.GroupColumnDescriptions.Add(new GroupColumnDescription
            {
                ColumnName = "CategoryName"
            });

            dataGrid.CaptionSummaryTemplate = new DataTemplate(() =>
            {
                var stackLayout = new StackLayout { Orientation = StackOrientation.Horizontal, Margin = new Thickness(5, 0) };
                var label = new Label { FontFamily = "FiraSans-Regular.otf#Fira Sans Regular", VerticalTextAlignment = TextAlignment.Center, FontSize = 16, TextColor = Color.Black };

                var icon = new Image { HeightRequest = 25 };
                icon.SetBinding(Image.SourceProperty, new Binding(".", BindingMode.Default, new BudgetGridIconConverter(), dataGrid));

                var converter = new BudgetDataGridSummaryConverter();
                var binding = new Binding(".", BindingMode.Default, converter, dataGrid);
                label.SetBinding(Label.TextProperty, binding);

                stackLayout.Children.Add(icon);
                stackLayout.Children.Add(label);

                return new ViewCell { View = stackLayout };
            });

            var gridSummaryRow = new GridGroupSummaryRow
            {
                ShowSummaryInRow = true,
                Title = "{Key}",
                SummaryColumns = new ObservableCollection<ISummaryColumn>
                {
                    new GridSummaryColumn
                    {
                        Name = "Total",
                        MappingName="Subcat.Value",
                        SummaryType= SummaryType.Custom,
                        CustomAggregate = new CurrencyDataGridHeader(),
                        Format = "{Currency}"
                    }
                }
            };
            dataGrid.CaptionSummaryRow = gridSummaryRow;

            dataGrid.Columns.Add(new GridTextColumn
            {
                MappingName = "SubcatName",
                HeaderText = "Kategoria",
                HeaderFont = "FiraSans-Bold.otf#Fira Sans Bold",
                RecordFont = "FiraSans-Regular.otf#Fira Sans Regular",
                HeaderCellTextSize = 16,
                LoadUIView = true,
                ColumnSizer = ColumnSizer.Star,
                TextAlignment = TextAlignment.Start
                //Width = 100
            });

            //dataGrid.SetBinding(SfDataGrid.ItemsSourceProperty, nameof(BudgetTemplate));

            return dataGrid;
        }

        private void CategoryEditEnd(object sender, GridCurrentCellEndEditEventArgs e)
        {
            _dataGrid.View.Refresh();
        }
    }
}