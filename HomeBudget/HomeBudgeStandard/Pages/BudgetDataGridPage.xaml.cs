using HomeBudgeStandard.Views;
using HomeBudget.Code;
using HomeBudget.Code.Logic;
using HomeBudget.Converters;
using HomeBudget.Utils;
using Syncfusion.Data;
using Syncfusion.SfDataGrid.XForms;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeBudget.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class BudgetDataGridPage : ContentPage
	{
        public ObservableCollection<BudgetViewModelData> BudgetData { get; private set; }
        private SfDataGrid _dataGrid;
        private DateTime _date;

		public BudgetDataGridPage (DateTime date)
		{
            _date = date;
            BudgetData = new ObservableCollection<BudgetViewModelData>();
            InitializeComponent ();
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            //MessagingCenter.Send(this, "Landscape");
            Device.BeginInvokeOnMainThread(async () =>
                {
                    CreateDataGrid();
                    await SetupDataGrid(_date);
                });
        }

        protected override void OnDisappearing()
        {
            MessagingCenter.Send(this, "Portrait");
        }

        private async Task SetupDataGrid(DateTime date)
        {
            var budget = new ObservableCollection<BudgetViewModelData>();
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    var budgetReal = MainBudget.Instance.GetMonth(date).BudgetReal;

                    foreach (var category in budgetReal.Categories)
                    {
                        foreach (var subcat in category.subcats)
                        {
                            subcat.CheckIfValid();
                            var model = new BudgetViewModelData
                            {
                                Category = category,
                                Subcat = subcat,
                                SubcatReal = subcat as RealSubcat,
                                Name = category.Name
                            };
                            budget.Add(model);
                        }
                    }
                }
                catch (Exception e)
                {
                    var msg = e.Message;
                    return;
                }
            });

            Device.BeginInvokeOnMainThread(() =>
            {
                BudgetData = budget;
                OnPropertyChanged(nameof(BudgetData));
            });
        }

        private void CreateDataGrid()
        {
            _dataGrid = new SfDataGrid
            {
                EnableDataVirtualization = true,
                AutoGenerateColumns = false,
                AutoExpandGroups = false,
                AllowGroupExpandCollapse = true,
                LiveDataUpdateMode = LiveDataUpdateMode.AllowSummaryUpdate,
                SelectionMode = Syncfusion.SfDataGrid.XForms.SelectionMode.SingleDeselect,
                NavigationMode = NavigationMode.Cell,
                FrozenColumnsCount = 2,
                EditTapAction = TapAction.OnTap,
                GridStyle = new BudgetDataGridStyle()
            };

            _dataGrid.SortComparers.Add(new SortComparer
            {
                PropertyName = "Category.Name",
                Comparer = new BudgetCategorySortComparer()

            });

            _dataGrid.GroupColumnDescriptions.Add(new GroupColumnDescription
            {
                ColumnName = "Category.Name",
            });

            var gridSummaryRow = new GridGroupSummaryRow
            {
                ShowSummaryInRow = true,
                Title = "{Key}: {Total}",
                SummaryColumns = new ObservableCollection<ISummaryColumn>
                {
                    new GridSummaryColumn
                    {
                        Name = "Total",
                        MappingName="Subcat.Value",
                        SummaryType= SummaryType.Custom,
                        CustomAggregate = new CurrencyDataGridHeader(),
                        Format = "{Currency}",
                    }
                },
            };
            _dataGrid.CaptionSummaryRow = gridSummaryRow;

            _dataGrid.CaptionSummaryTemplate = new DataTemplate(() =>
            {
                var stackLayout = new StackLayout { Orientation = StackOrientation.Horizontal, Margin = new Thickness(5, 0) };
                var label = new Label { FontFamily = "FiraSans-Regular.otf#Fira Sans Regular", VerticalTextAlignment = TextAlignment.Center, FontSize = 16, TextColor = Color.Black };

                var icon = new Image { HeightRequest = 25 };
                icon.SetBinding(Image.SourceProperty, new Binding(".", BindingMode.Default, new BudgetGridIconConverter(), _dataGrid));

                var converter = new BudgetDataGridSummaryConverter();
                var binding = new Binding(".", BindingMode.Default, converter, _dataGrid);
                label.SetBinding(Label.TextProperty, binding);

                stackLayout.Children.Add(icon);
                stackLayout.Children.Add(label);

                return new ViewCell { View = stackLayout };
            });

            _dataGrid.Columns.Add(new GridTextColumn
            {
                MappingName = "Subcat.Name",
                HeaderText = "Kategoria",
                Width = 100,
                HeaderFont = "FiraSans-Bold.otf#Fira Sans Bold",
                RecordFont = "FiraSans-Regular.otf#Fira Sans Regular",
                HeaderCellTextSize = 16,
                LoadUIView = true
            });

            _dataGrid.Columns.Add(new GridNumericColumn
            {
                MappingName = "SubcatReal.Value",
                HeaderText = "Suma",
                HeaderFont = "FiraSans-Bold.otf#Fira Sans Bold",
                RecordFont = "FiraSans-Bold.otf#Fira Sans Bold",
                LoadUIView = true,
                CellTextSize = 10,
                HeaderCellTextSize = 16,
                DisplayBinding = new Binding() { Path = "SubcatReal.Value", Converter = new CurrencyValueConverter() }
            });

            for (int i = 0; i < 31; i++)
            {
                _dataGrid.Columns.Add(new GridNumericColumn
                {
                    MappingName = $"SubcatReal.Values[{i}].Value",
                    HeaderText = (i + 1).ToString(),
                    AllowEditing = false,
                    //LoadUIView = true,
                    HeaderFont = "FiraSans-Bold.otf#Fira Sans Bold",
                    RecordFont = "FiraSans-Regular.otf#Fira Sans Regular",
                    CellTextSize = 10,
                    HeaderCellTextSize = 16,
                    HeaderFontAttribute = FontAttributes.Bold,
                    DisplayBinding = new Binding() { Path = $"SubcatReal.Values[{i}].Value", Converter = new CurrencyValueConverter() }
                });
            }

            _dataGrid.SetBinding(SfDataGrid.ItemsSourceProperty, nameof(BudgetData));

            mainLayout.Children.Add(_dataGrid);
        }
    }
}