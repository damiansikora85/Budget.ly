using System.Collections.ObjectModel;
using HomeBudget.Code;
using HomeBudget.Code.Logic;
using HomeBudget.Converters;
using HomeBudget.Utils;
using Syncfusion.Maui.DataGrid;

namespace HomeBudget.Pages
{
    public partial class BudgetDataGridPage : ContentPage
	{
        public ObservableCollection<BudgetViewModelData> BudgetData { get; private set; }
        private DateTime _date;

		public BudgetDataGridPage (DateTime date)
		{
            _date = date;
            BudgetData = new ObservableCollection<BudgetViewModelData>();
            InitializeComponent ();
            BindingContext = this;

            for (var i = 0; i < 31; i++)
            {
                dataGrid.Columns.Add(new DataGridNumericColumn
                {
                    MappingName = $"SubcatReal.Values[{i}].Value",
                    HeaderText = (i + 1).ToString(),
                    AllowEditing = false,
                    DisplayBinding = new Binding() { Path = $"SubcatReal.Values[{i}].Value", Converter = new CurrencyValueConverter() },
                    Width = 80
                });
            }
        }

        protected override async void OnAppearing()
        {
            //MessagingCenter.Send(this, "Landscape");
            MainThread.BeginInvokeOnMainThread(async () =>
                {
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

            MainThread.BeginInvokeOnMainThread(() =>
            {
                BudgetData = budget;
                OnPropertyChanged(nameof(BudgetData));
            });
        }
    }
}