using HomeBudget.Code;
using HomeBudget.Pages;
using HomeBudget.Pages.PC;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeBudget
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPagePC : ContentPage
    {
        public enum EMode
        {
            Expense,
            Income,
            Planning
        }

        private EMode mode;

        private const string TEMPLATE_FILENAME = "HomeBudget.template.json";
        private string selectedCategoryName;
        private int selectedCategoryId;
        private DateTime selectedDate;
        private CategoryElement lastCategorySelected;
        private bool shouldAutoLaunchPopup;
        private MainPagePCViewModel viewModel;
        private int selectedSubcatId;

        public ICommand SubcatClicked { get; private set; }

        public MainPagePC()
        {
            InitializeComponent();
            viewModel = new MainPagePCViewModel();
            BindingContext = viewModel;

            InitBudget();
            CreateCategoriesBar();
            CreateIncomesBar();

            DateTime currentDate = DateTime.Now;
            dateText.Text = currentDate.ToString("MMMM") + " " + currentDate.Year;
        }

        private void InitBudget()
        {
            Calculator.IsVisible = false;
            MainBudget.Instance.onBudgetLoaded += OnBudgetLoaded;
            var assembly = typeof(MainPage).GetTypeInfo().Assembly;

            Stream stream = assembly.GetManifestResourceStream(TEMPLATE_FILENAME);
            string jsonString = "";
            using (var reader = new System.IO.StreamReader(stream))
            {
                jsonString = reader.ReadToEnd();
                MainBudget.Instance.InitWithJson(jsonString);
            }
        }

        private void OnBudgetLoaded()
        {
            SetupBudgetSummary();
        }

        private async void OnPlanClick(object sender, EventArgs args)
        {
            NavigationPage planPage = new NavigationPage(new PlanningPage());
            await Navigation.PushModalAsync(planPage);
        }

        private async void OnAnalyticsClick(object sender, EventArgs e)
        {
            NavigationPage analizePage = new NavigationPage(new AnalyticsPagePC());
            await Navigation.PushModalAsync(analizePage);
        }

        private async void OnIncomeClick(object sender, EventArgs e)
        {
            mode = EMode.Income;
            await incomes.TranslateTo(0, 0);
        }

        private async void OnExpenseClick(object sender, EventArgs e)
        {
            mode = EMode.Expense;
            await categoriesScroll.TranslateTo(0, 0);
        }

        private void CreateIncomesBar()
        {
            string iconsPathPrefix = "Assets\\";
            int incomesNum = MainBudget.Instance.BudgetDescription.Incomes.Count;
            for(int i=0; i<incomesNum; i++)
            {
                Grid grid = CreateCategoryGrid();
                BudgetIncomeTemplate incomeTemplate = MainBudget.Instance.BudgetDescription.Incomes[i];
                CategoryElement element = CategoryElement.CreateAndAddToGrid(incomeTemplate.Id, incomeTemplate.Name, iconsPathPrefix+incomeTemplate.IconFileName, grid);
                element.onClickFunc += OnIncomeCategoryClick;
                incomes.Children.Add(grid);
            }
        }

        private async Task OnIncomeCategoryClick(int incomeCategoryID, CategoryElement categoryElement)
        {
            selectedCategoryId = incomeCategoryID;
            selectedDate = DateTime.Now;
            categoryElement.Deselect();
            await incomes.TranslateTo(-150, 0);

            ShowCalculatorView("Dochód", selectedDate);
        }

        private void CreateCategoriesBar()
        {
            int categoriesNum = MainBudget.Instance.BudgetDescription.Categories.Count;
            string iconsPathPrefix = "Assets\\Icons\\";
            for(int i=0; i< categoriesNum; i++)
            {
                Grid grid = CreateCategoryGrid();

                CategoryElement element = CategoryElement.CreateAndAddToGrid(MainBudget.Instance.BudgetDescription.Categories[i].Id, MainBudget.Instance.BudgetDescription.Categories[i].Name, iconsPathPrefix + MainBudget.Instance.BudgetDescription.Categories[i].IconFileName, grid);
                element.onClickFunc += OnCategoryClicked;

                categories.Children.Add(grid); 
            }
        }

        private async Task OnCategoryClicked(int categoryID, CategoryElement categoryElement)
        {
            mode = EMode.Expense;
            selectedCategoryName = categoryElement.Name;
            selectedCategoryId = categoryElement.Id;
            selectedDate = DateTime.Now;
            CreateSubCat(categoryID);
            await subcat.TranslateTo(150, 0);

            if (lastCategorySelected != null)
                lastCategorySelected.Deselect();

            lastCategorySelected = categoryElement;
        }

        private void CreateSubCat(int categoryID)
        {
            subcat.Children.Clear();

            string iconsPathPrefix = "Assets\\Icons\\";
            int subcatNum = MainBudget.Instance.BudgetDescription.Categories[categoryID].subcategories.Count;

            for(int i=0; i<subcatNum; i++)
            {
                Grid grid = CreateCategoryGrid();
                CategoryElement element = CategoryElement.CreateAndAddToGrid(i, MainBudget.Instance.BudgetDescription.Categories[categoryID].subcategories[i], iconsPathPrefix + MainBudget.Instance.BudgetDescription.Categories[categoryID].IconFileName, grid);
                element.onClickFunc += OnSubcatClicked;

                subcat.Children.Add(grid);
            }
        }

        private Grid CreateCategoryGrid()
        {
            Grid grid = new Grid()
            {
                HeightRequest = 100,
            };
            ColumnDefinition c0 = new ColumnDefinition()
            {
                Width = new GridLength(1, GridUnitType.Star)
            };
            ColumnDefinition c1 = new ColumnDefinition()
            {
                Width = new GridLength(2, GridUnitType.Star)
            };
            ColumnDefinition c2 = new ColumnDefinition()
            {
                Width = new GridLength(1, GridUnitType.Star)
            };
            grid.ColumnDefinitions.Add(c0);
            grid.ColumnDefinitions.Add(c1);
            grid.ColumnDefinitions.Add(c2);

            RowDefinition r0 = new RowDefinition()
            {
                Height = new GridLength(1, GridUnitType.Star)
            };
            RowDefinition r1 = new RowDefinition()
            {
                Height = new GridLength(1, GridUnitType.Star)
            };
            grid.RowDefinitions.Add(r0);
            grid.RowDefinitions.Add(r1);

            return grid;
        }

        private async Task OnSubcatClicked(int id, CategoryElement element)
        {
            selectedSubcatId = id;
            lastCategorySelected.Deselect();
            element.Deselect();
            await subcat.TranslateTo(-185, 0);
            await categoriesScroll.TranslateTo(-150, 0);

            ShowCalculatorView(lastCategorySelected.Name, selectedDate);
        }

        private void SetupBudgetSummary()
        {
            BudgetMonth budgetMonth = MainBudget.Instance.GetCurrentMonthData();
            double monthExpenses = budgetMonth.GetTotalExpense();
            double monthIncomes = budgetMonth.GetTotalIncome();
            double diff = monthIncomes - monthExpenses;
            expansesText.Text = "Wydatki: " + monthExpenses;
            incomeText.Text = "Dochody: " + monthIncomes;
            diffText.Text = "Różnica: " + diff;
        }

        private async void OnOk(object sender, EventArgs e)
        {
            Calculator.IsVisible = false;
            if (mode == EMode.Income)
                await MainBudget.Instance.AddIncome(float.Parse(viewModel.CalculationText), selectedDate, selectedCategoryId);
            else if (mode == EMode.Expense)
                await MainBudget.Instance.AddExpense(float.Parse(viewModel.CalculationText), selectedDate, selectedCategoryId, selectedSubcatId);

            SetupBudgetSummary();
        }

        private async void OnCancel(object sender, EventArgs e)
        {
            Calculator.IsVisible = false;
        }

        private async void OnChangeDate(object sender, EventArgs e)
        {
            var calendar = new CalendarPopup(SelectedDateChanged);
            await Navigation.PushModalAsync(calendar);
        }

        private void ShowCalculatorView(string header, DateTime date)
        {
            Calculator.IsVisible = true;

            Header.Text = header;
            DateButton.Text = date.ToString("d");
        }

        private void SelectedDateChanged(DateTime newDate)
        {
            selectedDate = newDate;
            DateButton.Text = selectedDate.ToString("d");
        }
    }



    public class MainPagePCViewModel : INotifyPropertyChanged
    {
        public enum CalculatorKey
        {
            Zero = 0,
            One = 1,
            Two = 2,
            Three = 3,
            Four = 4,
            Five = 5,
            Six = 6,
            Seven = 7,
            Eight = 8,
            Nine = 9,
            Backspace = 20,
            Clear,
            PlusMinus,
            Divide,
            Multiply,
            Minus,
            Plus,
            Equal,
            Point,
            Ok,
            Cancel,
            Calendar
        }

        public ICommand KeyPressed { get; private set; }
        private String calculationText;
        private string categoryText;
        private string dateText;

        public event PropertyChangedEventHandler PropertyChanged;

        public string CategoryText
        {
            get { return categoryText; }
            set
            {
                categoryText = value;
                if (string.IsNullOrEmpty(categoryText))
                {
                    categoryText = " ";
                }
                OnPropertyChanged("CategoryText");
            }
        }

        public MainPagePCViewModel()
        {
            KeyPressed = new Command<string>(HandleKeyPressed);
            CalculationText = "";
        }

        public string CalculationText
        {
            get { return calculationText; }
            set
            {
                calculationText = value;
                if (string.IsNullOrEmpty(calculationText))
                {
                    calculationText = " "; // HACK to force grid view to allocate space.
                }
                OnPropertyChanged("CalculationText");
            }
        }

        public string DateText
        {
            get { return dateText; }
            set
            {
                dateText = value;
                if (string.IsNullOrEmpty(dateText))
                {
                    dateText = " ";
                }
            }
        }

        void HandleKeyPressed(string value)
        {
            var calculatorKey = (CalculatorKey)Enum.Parse(typeof(CalculatorKey), value, true);

            switch (calculatorKey)
            {
                case CalculatorKey.One:
                case CalculatorKey.Two:
                case CalculatorKey.Three:
                case CalculatorKey.Four:
                case CalculatorKey.Five:
                case CalculatorKey.Six:
                case CalculatorKey.Seven:
                case CalculatorKey.Eight:
                case CalculatorKey.Nine:
                case CalculatorKey.Zero:
                    CalculationText += ((int)calculatorKey).ToString();
                    break;
                case CalculatorKey.Equal:
                    break;
                case CalculatorKey.Point:
                    CalculationText += '.';
                    break;
            }
            return;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
