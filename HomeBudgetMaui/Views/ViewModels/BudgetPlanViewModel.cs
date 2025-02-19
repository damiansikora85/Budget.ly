using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using HomeBudget.Code;
using HomeBudget.Code.Logic;
using HomeBudget.Utils;
using HomeBudgetStandard.Views.ViewModels;
using static Java.Lang.Character;

namespace HomeBudgetStandard.Views.ViewModels
{
    public class BudgetPlanViewModel : BaseViewModel
    {
        public ObservableCollection<BudgetViewModelData> Budget
        {
            get; set;
        }

        public ObservableCollection<string> Plan { get; set; }

        public bool ExpensesVisible { get; private set; } = true;
        public bool IncomesVisible { get; private set; } = false;
        public TextDecorations ExpensesChartTextDecorations { get; private set; } = TextDecorations.Underline;
        public TextDecorations IncomesChartTextDecorations { get; private set; } = TextDecorations.None;

        public ICommand ExpensesChartCommand { get; private set; }
        public ICommand IncomesChartCommand { get; private set; }
        public ICommand PrevMonthCommand { get; private set; }
        public ICommand NextMonthCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }

        private ObservableCollection<OrderInfo> orderInfo;
        public ObservableCollection<OrderInfo> OrderInfoCollection
        {
            get
            {
                return orderInfo;
            }
            set
            {
                this.orderInfo = value;
            }
        }


        public List<ChartData> ExpensesChartData
        {
            get; private set;
        }
        public List<ChartData> IncomesChartData
        {
            get; private set;
        }

        private DateTime _currentMonth;

        private readonly CultureInfo _cultureInfoPL = new CultureInfo("pl-PL");
        public string Date
        {
            get => _currentMonth.ToString("MMMM yyyy", _cultureInfoPL);
        }

        public BudgetPlanViewModel()
        {
            _currentMonth = DateTime.Now;
            ExpensesChartCommand = new Command(ForceSwitchChart);
            IncomesChartCommand = new Command(ForceSwitchChart);
            PrevMonthCommand = new AsyncRelayCommand(OnPrevMonth);
            NextMonthCommand = new AsyncRelayCommand(OnNextMonth);
            SaveCommand = new AsyncRelayCommand(OnSave);
            Budget = new ObservableCollection<BudgetViewModelData>();
            Plan = new ObservableCollection<string>
            {
                "qwe",
                "fgfd",
                "sadfsd"
            };

            orderInfo = new ObservableCollection<OrderInfo>();
            orderInfo.Add(new OrderInfo("1001", "Maria Anders", "Germany", "ALFKI", "Berlin"));
            orderInfo.Add(new OrderInfo("1002", "Ana Trujillo", "Mexico", "ANATR", "Mexico D.F."));
            orderInfo.Add(new OrderInfo("1003", "Ant Fuller", "Mexico", "ANTON", "Mexico D.F."));
            orderInfo.Add(new OrderInfo("1004", "Thomas Hardy", "UK", "AROUT", "London"));
            orderInfo.Add(new OrderInfo("1005", "Tim Adams", "Sweden", "BERGS", "London"));
            orderInfo.Add(new OrderInfo("1006", "Hanna Moos", "Germany", "BLAUS", "Mannheim"));
            orderInfo.Add(new OrderInfo("1007", "Andrew Fuller", "France", "BLONP", "Strasbourg"));
            orderInfo.Add(new OrderInfo("1008", "Martin King", "Spain", "BOLID", "Madrid"));
            orderInfo.Add(new OrderInfo("1009", "Lenny Lin", "France", "BONAP", "Marsiella"));
            orderInfo.Add(new OrderInfo("1010", "John Carter", "Canada", "BOTTM", "Lenny Lin"));
            orderInfo.Add(new OrderInfo("1011", "Laura King", "UK", "AROUT", "London"));
            orderInfo.Add(new OrderInfo("1012", "Anne Wilson", "Germany", "BLAUS", "Mannheim"));
            orderInfo.Add(new OrderInfo("1013", "Martin King", "France", "BLONP", "Strasbourg"));
            orderInfo.Add(new OrderInfo("1014", "Gina Irene", "UK", "AROUT", "London"));
        }

        public void Setup()
        {
            _currentMonth = DateTime.Now;
            SetupDataGrid(DateTime.Now);
            UpdateCharts();
        }

        public void ForceSwitchChart()
        {
            ExpensesVisible = !ExpensesVisible;
            IncomesVisible = !IncomesVisible;
            OnPropertyChanged(nameof(ExpensesVisible));
            OnPropertyChanged(nameof(IncomesVisible));

            ExpensesChartTextDecorations = ExpensesChartTextDecorations == TextDecorations.None ? TextDecorations.Underline : TextDecorations.None;
            IncomesChartTextDecorations = IncomesChartTextDecorations == TextDecorations.None ? TextDecorations.Underline : TextDecorations.None;
            OnPropertyChanged(nameof(ExpensesChartTextDecorations));
            OnPropertyChanged(nameof(IncomesChartTextDecorations));
        }

        public void UpdateCharts()
        {
            var budgetPlanned = MainBudget.Instance.GetMonth(_currentMonth).BudgetPlanned;

            IncomesChartData = new List<ChartData>();
            var incomesCategories = budgetPlanned.GetIncomesCategories();
            var totalIncome = incomesCategories.Sum(el => el.TotalValues);
            foreach (var category in incomesCategories.Cast<BudgetPlannedCategory>())
            {
                foreach (var subcat in category.subcats)
                {
                    if (subcat.Value > 0)
                    {
                        IncomesChartData.Add(new ChartData { Label = subcat.Name, Value = subcat.Value, Percentage = subcat.Value / totalIncome });
                    }
                }

            }
            ExpensesChartData = new List<ChartData>();
            var totalExpense = budgetPlanned.Categories.Sum(el => el.TotalValues) - totalIncome;
            foreach (var category in budgetPlanned.Categories.Cast<BudgetPlannedCategory>())
            {
                if (!category.IsIncome && category.TotalValues > 0)
                {
                    ExpensesChartData.Add(new ChartData { Label = category.Name, Value = category.TotalValues, Percentage = category.TotalValues / totalExpense });
                }
            }

            OnPropertyChanged(nameof(IncomesChartData));
            OnPropertyChanged(nameof(ExpensesChartData));
        }

        private async Task OnSave()
        {
            await MainBudget.Instance.UpdateMainPlannedBudget(_currentMonth);
            //UserDialogs.Instance.ShowToast("Ten plan budżetu będzie używany w kolejnych miesiącach");
        }

        private async Task OnNextMonth()
        {
            _currentMonth = _currentMonth.AddMonths(1);
            await RefreshAsync();
        }

        private async Task OnPrevMonth()
        {
            _currentMonth = _currentMonth.AddMonths(-1);
            await RefreshAsync();
        }

        private async Task RefreshAsync()
        {
            OnPropertyChanged(nameof(Date));
            //await SetupDataGrid(_currentMonth);
            UpdateCharts();
        }

        private void SetupDataGrid(DateTime date)
        {
            var budget = new ObservableCollection<BudgetViewModelData>();
            try
            {
                var budgetPlanned = MainBudget.Instance.GetMonth(date).BudgetPlanned;

                foreach (var category in budgetPlanned.Categories)
                {
                    foreach (var subcat in category.subcats)
                    {
                        var model = new BudgetViewModelData
                        {
                            Category = category,
                            Subcat = subcat,
                            SubcatPlanned = subcat as PlannedSubcat,
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

            Budget = budget;
            OnPropertyChanged(nameof(Budget));

            Device.BeginInvokeOnMainThread(() =>
            {
                //_previousMonthButton.IsEnabled = MainBudget.Instance.HasMonthData(_currentMonth.AddMonths(-1));
                //_nextMonthButton.IsEnabled = MainBudget.Instance.HasMonthData(_currentMonth.AddMonths(1));
            });
        }

        public class OrderInfo
        {
            private string orderID;
            private string customerID;
            private string customer;
            private string shipCity;
            private string shipCountry;

            public string OrderID
            {
                get
                {
                    return orderID;
                }
                set
                {
                    this.orderID = value;
                }
            }

            public string CustomerID
            {
                get
                {
                    return customerID;
                }
                set
                {
                    this.customerID = value;
                }
            }

            public string ShipCountry
            {
                get
                {
                    return shipCountry;
                }
                set
                {
                    this.shipCountry = value;
                }
            }

            public string Customer
            {
                get
                {
                    return this.customer;
                }
                set
                {
                    this.customer = value;
                }
            }

            public string ShipCity
            {
                get
                {
                    return shipCity;
                }
                set
                {
                    this.shipCity = value;
                }
            }

            public OrderInfo(string orderId, string customerId, string country, string customer, string shipCity)
            {
                this.OrderID = orderId;
                this.CustomerID = customerId;
                this.Customer = customer;
                this.ShipCountry = country;
                this.ShipCity = shipCity;
            }
        }
    }
}
