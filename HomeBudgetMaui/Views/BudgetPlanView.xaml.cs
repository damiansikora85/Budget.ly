using Acr.UserDialogs;
using HomeBudget.Code;
using HomeBudget.Converters;
using HomeBudgetStandard.Views.ViewModels;
using Syncfusion.Maui.Data;
using Syncfusion.Maui.DataGrid;

namespace HomeBudgetStandard.Views
{
	public partial class BudgetPlanView : ContentPage//, IActiveAware
    {
        private BudgetPlanViewModel _viewModel;
        private bool _setupDone;   
        private SfDataGrid _dataGrid;
        private Grid _mainGrid;

        public event EventHandler IsActiveChanged;

        bool _isActive;
        public virtual bool IsActive
        {
            get
            {
                return _isActive;
            }

            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                }
            }
        }

        public BudgetPlanView ()
		{
            _viewModel = new BudgetPlanViewModel();
            BindingContext = _viewModel;
            InitializeComponent();

            SetupVariables();
            //CreateDataGrid();

            //try
            //{

            //    if (MainBudget.Instance.IsDataLoaded && !_setupDone)
            //    {
            //        Setup();
            //    }
            //    else
            //    {
            //        _viewModel.UpdateCharts();
            //        _viewModel.ForceSwitchChart();
            //    }

            //}
            //catch (Exception exc)
            //{
            //    var msg = exc.Message;
            //}

            //_setupDone = true;
        }

        protected override void OnAppearing()
        {
            //if (!IsActive)
            //{
            //    return;
            //}

            MainBudget.Instance.BudgetDataChanged -= MarkDataChanged;

            if (MainBudget.Instance.IsDataLoaded && !_setupDone)
            {
                _viewModel.Setup();
                Setup();

            }
            else
            {
                _viewModel.UpdateCharts();
                _viewModel.ForceSwitchChart();
            }

            _setupDone = true;
        }

        protected override void OnDisappearing()
        {
            MainBudget.Instance.BudgetDataChanged += MarkDataChanged;
        }

        private void MarkDataChanged(bool obj)
        {
            _setupDone = false;
        }

        public async Task Activate()
        {
            try
            {
                UserDialogs.Instance.ShowLoading("");

                var dataTemplate = (DataTemplate)Resources["ContentTemplate"];
                View view = null;
                await Task.Factory.StartNew(() =>
                {
                    view = (View)dataTemplate.CreateContent();
                });
                Content = view;
                BindingContext = _viewModel;

                SetupVariables();
                CreateDataGrid();

                if (MainBudget.Instance.IsDataLoaded && !_setupDone)
                {
                    Setup();
                }
                else
                {
                    _viewModel.UpdateCharts();
                    _viewModel.ForceSwitchChart();
                }

                _setupDone = true;

                UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
        }

        private void SetupVariables()
        {
            _mainGrid = Content.FindByName<Grid>("mainGrid");
        }

        private void Setup()
        {
            _viewModel.Setup();
        }

        private void CreateDataGrid()
        {
            _dataGrid = new SfDataGrid
            {
                //EnableDataVirtualization = true,
                AutoGenerateColumnsMode = AutoGenerateColumnsMode.None,
                AutoExpandGroups = false,
                AllowGroupExpandCollapse = true,
                LiveDataUpdateMode = LiveDataUpdateMode.AllowSummaryUpdate,
                SelectionMode = DataGridSelectionMode.Single,
                NavigationMode = DataGridNavigationMode.Cell,
                EditTapAction = DataGridTapAction.OnTap,
                //GridStyle = new BudgetDataGridStyle(),
                Margin = new Thickness(12, 0),
            };

            _dataGrid.SortComparers.Add(new SortComparer
            {
                PropertyName = "Category.Name",
                Comparer = new BudgetCategorySortComparer()
            });

            _dataGrid.CurrentCellEndEdit += DataGrid_CurrentCellEndEdit;
            _dataGrid.GroupColumnDescriptions.Add(new GroupColumnDescription
            {
                ColumnName = "Category.Name"
            });

            _dataGrid.CaptionSummaryTemplate = new DataTemplate(() =>
            {
                var stackLayout = new StackLayout { Orientation = StackOrientation.Horizontal, Margin = new Thickness(5, 0) };
                var label = new Label { FontFamily = "FiraSans-Regular.otf#Fira Sans Regular", VerticalTextAlignment = TextAlignment.Center, FontSize = 16, TextColor = Colors.Black };

                var icon = new Image { HeightRequest = 25 };
                icon.SetBinding(Image.SourceProperty, new Binding(".", BindingMode.Default, new BudgetGridIconConverter(), _dataGrid));

                var converter = new BudgetDataGridSummaryConverter();
                var binding = new Binding(".", BindingMode.Default, converter, _dataGrid);
                label.SetBinding(Label.TextProperty, binding);

                stackLayout.Children.Add(icon);
                stackLayout.Children.Add(label);

                return new ViewCell { View = stackLayout };
            });

            //ar gridSummaryRow = new GridGroupSummaryRow
            //{
            //    ShowSummaryInRow = true,
            //    Title = "{Key}: {Total}",
            //    SummaryColumns = new ObservableCollection<ISummaryColumn>
            //    {
            //        new GridSummaryColumn
            //        {
            //            Name = "Total",
            //            MappingName="Subcat.Value",
            //            SummaryType= SummaryType.Custom,
            //            CustomAggregate = new CurrencyDataGridHeader(),
            //            Format = "{Currency}"
            //        }
            //    }
            //};
            //_dataGrid.CaptionSummaryRow = gridSummaryRow;

            _dataGrid.Columns.Add(new DataGridTextColumn
            {
                MappingName = "Subcat.Name",
                HeaderText = "Kategoria",
                //HeaderFont = "FiraSans-Bold.otf#Fira Sans Bold",
                //RecordFont = "FiraSans-Regular.otf#Fira Sans Regular",
                //HeaderCellTextSize = 16,
                //LoadUIView = true,
                //ColumnSizer = ColumnSizer.Star,
                ColumnWidthMode = ColumnWidthMode.FitByCell,
                Width = 100
            });

            try
            {
                _dataGrid.Columns.Add(new DataGridNumericColumn
                {
                    MappingName = "SubcatPlanned.Value",
                    HeaderText = "Suma",
                    AllowEditing = true,
                    //HeaderFont = "FiraSans-Bold.otf#Fira Sans Bold",
                    //RecordFont = "FiraSans-Regular.otf#Fira Sans Regular",
                    ////LoadUIView = true,
                    //CellTextSize = 14,
                    //HeaderCellTextSize = 16,
                    //ColumnSizer = ColumnSizer.Star,
                    ColumnWidthMode = ColumnWidthMode.FitByCell,
                    Width = 100

                    //DisplayBinding = new Binding() { Path = "SubcatPlanned.Value", Converter = new CurrencyValueConverter() }
                });
            }
            catch (Exception exc)
            {
                var msg = exc.Message;
            }

            _dataGrid.SetBinding(SfDataGrid.ItemsSourceProperty, new Binding(path: nameof(_viewModel.Budget), source: _viewModel));

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(3, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = 50 });

            grid.Children.Add(_dataGrid);
            var button = new Button { Text = "Użyj w kolejnych miesiącach", Style = (Style)Application.Current.Resources["ButtonStyle"], VerticalOptions = LayoutOptions.End, Margin= new Thickness(12, 3) };
            button.SetBinding(Button.CommandProperty, nameof(_viewModel.SaveCommand));
            grid.Children.Add(button);
            Grid.SetRow(button, 1);

            Grid.SetRow(grid, 2);
            _mainGrid.Children.Add(grid);
        }

        private void DataGrid_CurrentCellEndEdit(object? sender, DataGridCurrentCellEndEditEventArgs e)
        {
            if (e.OldValue is double oldValue && e.NewValue is double newValue && oldValue == newValue)
            {
                return;
            }

            Task.Run(() => MainBudget.Instance.Save());

            if (sender is SfDataGrid dataGrid)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Task.Delay(100);

                    dataGrid.View.TopLevelGroup.UpdateCaptionSummaries();
                    dataGrid.View.Refresh();

                    _viewModel.UpdateCharts();
                });
            }
        }
    }
}