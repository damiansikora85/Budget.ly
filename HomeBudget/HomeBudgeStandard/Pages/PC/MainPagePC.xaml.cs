using Acr.UserDialogs;
using Dropbox.Api;
using HomeBudget.Code;
using HomeBudget.Code.Logic;
using HomeBudget.Pages.Utils;
using HomeBudget.Utils;
using HomeBudget.ViewModels;
using Syncfusion.SfCalendar.XForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace HomeBudget.Pages.PC
{
    //[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPagePC : ContentPage
    {
        public enum EMode
        {
            Expense,
            Income,
            Planning
        }

        private BudgetSummaryListView budgetSummaryElement;
        //private BudgetSideBar categoriesSideBar;

        private EMode mode;
        private string selectedCategoryName;
        private int selectedCategoryId;
        private DateTime selectedDate;
        private CategoryElement lastCategorySelected;
        private MainPagePcViewModel viewModel;
        private CalculatorViewModel calculatorViewModel;
        private int selectedSubcatId;
        private bool lockTapGesture;

        //Dropbox variables
        private const string RedirectUri = "https://localhost/authorize";
        private string appKey = "p6cayskxetnkx1a";
        private string oauth2State;

        public ICommand SubcatClicked { get; private set; }

        public MainPagePC()
        {
            InitializeComponent();
            viewModel = new MainPagePcViewModel();
            BindingContext = viewModel;
            addButton.GestureRecognizers.Add(new TapGestureRecognizer(OnAddClick));
            calculatorViewModel = new CalculatorViewModel();
            //sideBar.SetMode(Views.SideBarPC.EMode.Home);

            UserDialogs.Instance.ShowLoading("Loading...");

            if (!MainBudget.Instance.IsInitialized)
                InitBudget();
            else
                OnBudgetLoaded();
            
            lockTapGesture = false;

            var cultureInfoPL = new CultureInfo("pl-PL");
            var currentDate = DateTime.Now;
            dateText.Text = currentDate.ToString("dd MMMM yyyy", cultureInfoPL);

            calendar.Locale = cultureInfoPL;
            //calendar.ShowHeader = false;
            var monthViewSettings = new MonthViewSettings
            {
                HeaderBackgroundColor = Color.FromHex("#D2F3DF"),
                //HeaderFont = Font.,
                HeaderTextColor = Color.FromHex("#232825")
            };
            calendar.MonthViewSettings = monthViewSettings;
            calendar.OnCalendarTapped += SelectedDateChanged;
            calendar.MonthChanged += SelectedMonthChanged;

        }

        private void SelectedMonthChanged(object sender, MonthChangedEventArgs args)
        {
            var d = args.args.CurrentValue;
        }

        private void InitBudget()
        {
            MainBudget.Instance.Init();
            MainBudget.Instance.onBudgetLoaded += OnBudgetLoaded;

            if (string.IsNullOrEmpty(Helpers.Settings.DropboxAccessToken) == false)
            {
                DropboxButtonChangeVisibility(false);
                //DropboxLoginElement.IsVisible = false;
                DropboxManager.Instance.DownloadData();
            }
            else
            {
                DropboxButtonChangeVisibility(true);
                MainBudget.Instance.Load();
            }
        }

        private void DropboxButtonChangeVisibility(bool isVisible)
        {
            DropboxButon.IsVisible = isVisible;
            DropboxIcon.IsVisible = isVisible;
        }

        private void OnBudgetLoaded()
        {
            CreateCategoriesBar();
            CreateIncomesBar();
            SetupBudgetSummary();
            budgetSummaryElement = new BudgetSummaryListView();
            budgetSummaryElement.Setup(listView);
            DropboxButtonChangeVisibility(false);

            UserDialogs.Instance.HideLoading();
        }

        private void SetupBudgetSummary()
        {
            BudgetMonth budgetMonth = MainBudget.Instance.GetCurrentMonthData();
            double monthExpenses = budgetMonth.GetTotalExpenseReal();
            double monthIncomes = budgetMonth.GetTotalIncomeReal();
            double diff = monthIncomes - monthExpenses;
            CultureInfo cultureInfoPL = new CultureInfo("pl-PL");
            expansesText.Text = string.Format(cultureInfoPL, "{0:c}", monthExpenses);
            incomeText.Text = string.Format(cultureInfoPL, "{0:c}", monthIncomes);
            diffText.Text = string.Format(cultureInfoPL, "{0:c}", diff);

            double monthExpensesPlanned = budgetMonth.GetTotalExpensesPlanned();
            double monthIncomePlanned = budgetMonth.GetTotalIncomePlanned();
            double diffPlanned = monthIncomePlanned - monthExpensesPlanned;
            expansesPlannedText.Text = string.Format(cultureInfoPL, "{0:c}", monthExpensesPlanned);
            incomePlannedText.Text = string.Format(cultureInfoPL, "{0:c}", monthIncomePlanned);
            diffPlannedText.Text = string.Format(cultureInfoPL, "{0:c}", diffPlanned);
        }


        private async void OnPlanClick(object sender, EventArgs args)
        {
            await Navigation.PushModalAsync(new PlanningPage());
        }

        private async void OnAnalyticsClick(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new AnalyticsPagePC());
        }

        private async void OnAddClick(View arg1, object arg2)
        {
            mode = EMode.Expense;
            lockTapGesture = true;
            await categories.TranslateTo(0, 0);
            lockTapGesture = false;
        }

        private async void OnIncomeClick(object sender, EventArgs e)
        {
            mode = EMode.Income;
            lockTapGesture = true;
            await incomes.TranslateTo(0, 0);
            lockTapGesture = false;
        }

        private async void OnExpenseClick(object sender, EventArgs e)
        {
            mode = EMode.Expense;
            lockTapGesture = true;
            await categories.TranslateTo(0, 0);
            lockTapGesture = false;
        }

        private void CreateIncomesBar()
        {
            string iconsPathPrefix = "Assets\\";
            int categoriesNum = MainBudget.Instance.BudgetDescription.Categories.Count;
            List<BudgetCategoryTemplate> categoriesDesc = MainBudget.Instance.BudgetDescription.Categories;

            for (int i = 0; i < categoriesNum; i++)
            {
                if (categoriesDesc[i].IsIncome)
                {
                    BudgetCategoryTemplate incomeTemplate = categoriesDesc[i];
                    int index = 0;
                    foreach (string subcatName in incomeTemplate.subcategories)
                    {
                        Grid grid = CreateCategoryGrid();
                        CategoryElement element = CategoryElement.CreateAndAddToGrid(index++, subcatName, iconsPathPrefix + incomeTemplate.IconFileName, grid);
                        element.onClickFunc += OnIncomeCategoryClick;
                        incomes.Children.Add(grid);
                    }
                }
            }
        }

        private async Task OnIncomeCategoryClick(int incomeCategoryID, CategoryElement categoryElement)
        {
            selectedCategoryId = incomeCategoryID;
            selectedDate = DateTime.Now;
            categoryElement.Deselect();
            await incomes.TranslateTo(-150, 0);

            //ShowCalculatorView("Dochód", selectedDate);
        }

        private void CreateCategoriesBar()
        {
            var catagoriesData = new ObservableCollection<BudgetViewModelData>();
            var budgetReal = MainBudget.Instance.GetCurrentMonthData().BudgetReal;
            foreach(BaseBudgetCategory category in budgetReal.Categories)
            {
                var model = new BudgetViewModelData()
                {
                    Name = category.Name,
                    Category = category
                };
                catagoriesData.Add(model);
            }

            categories.ItemsSource = catagoriesData;

            /*int categoriesNum = MainBudget.Instance.BudgetDescription.Categories.Count;
            List<BudgetCategoryTemplate> categoriesDesc = MainBudget.Instance.BudgetDescription.Categories;
            string iconsPathPrefix = "Assets\\Categories\\";
            for(int i=0; i< categoriesNum; i++)
            {
                //if (categoriesDesc[i].IsIncome == false)
                {
                    Grid grid = CreateCategoryGrid();
                    CategoryElement element = CategoryElement.CreateAndAddToGrid(categoriesDesc[i].Id, categoriesDesc[i].Name, iconsPathPrefix + categoriesDesc[i].IconFileName, grid);
                    element.onClickFunc += OnCategoryClicked;

                    categories.Children.Add(grid);
                }
            }*/
        }

        void OnCategorySelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
            {
                return; //ItemSelected is called on deselection, which results in SelectedItem being set to null
            }

            selectedDate = DateTime.Now;
            BudgetViewModelData selectedCategory = (BudgetViewModelData)e.SelectedItem;
            CreateSubcatsList(selectedCategory.Category.Id);
            Header.Text = selectedCategory.Name;
            subcat.TranslateTo(150, 0);
        }

        private async Task OnCategoryClicked(int categoryID, CategoryElement categoryElement)
        {
            mode = EMode.Expense;
            selectedCategoryName = categoryElement.Name;
            selectedCategoryId = categoryElement.Id;
            selectedDate = DateTime.Now;
            CreateSubcatsList(categoryID);
            await subcat.TranslateTo(150, 0);

            if (lastCategorySelected != null)
                lastCategorySelected.Deselect();

            lastCategorySelected = categoryElement;
        }

        private void CreateSubcatsList(int categoryID)
        {
            ObservableCollection<BudgetViewModelData> subcatsData = new ObservableCollection<BudgetViewModelData>();
            BudgetReal budgetReal = MainBudget.Instance.GetCurrentMonthData().BudgetReal;
            BaseBudgetCategory category = budgetReal.GetBudgetCategory(categoryID);
            foreach (BaseBudgetSubcat subcat in category.subcats)
            {
                BudgetViewModelData model = new BudgetViewModelData()
                {
                    Name = subcat.Name,
                    Category = category,
                    Subcat = subcat
                };
                subcatsData.Add(model);
            }

            subcat.ItemsSource = subcatsData;
        }

        private Grid CreateCategoryGrid()
        {
            Grid grid = new Grid()
            {
                HeightRequest = 150,
            };
            ColumnDefinition c0 = new ColumnDefinition()
            {
                Width = new GridLength(1, GridUnitType.Star)
            };

            grid.ColumnDefinitions.Add(c0);

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

        private void OnSubcatClicked(object sender, SelectedItemChangedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
            categories.SelectedItem = null;
            subcat.TranslateTo(-250, 0);
            categories.TranslateTo(-200, 0);

            BudgetViewModelData selectedCategory = (BudgetViewModelData)e.SelectedItem;

            ShowCalculatorView(selectedCategory.Name, selectedCategory);
        }

        private async void OnOk(object sender, EventArgs e)
        {
            HideCalculator();

            if (mode == EMode.Income)
                await MainBudget.Instance.AddIncome(float.Parse(viewModel.CalculationResultText), selectedDate, selectedCategoryId);
            else if (mode == EMode.Expense)
                await MainBudget.Instance.AddExpense(float.Parse(viewModel.CalculationResultText), selectedDate, selectedCategoryId, selectedSubcatId);

            SetupBudgetSummary();
        }

        private async void OnCancel(object sender, EventArgs e)
        {
            HideCalculator();
        }

        private void ShowCalculator()
        {
            background.IsVisible = true;
            Calculator.IsVisible = true;
        }

        private void HideCalculator()
        {
            background.IsVisible = false;
            Calculator.IsVisible = false;
        }

        private void OnTap(object sender, EventArgs args)
        {
            if (lockTapGesture)
                return;

            if (lastCategorySelected != null)
                lastCategorySelected.Deselect();

            subcat.TranslateTo(-185, 0);
            categories.TranslateTo(-150, 0);
            incomes.TranslateTo(-150, 0);
        }

        private async void OnChangeDateClicked(object sender, EventArgs e)
        {
            BudgetDatePicker.IsVisible = true;
        }

        private void ShowCalculatorView(string subcatName, BudgetViewModelData data)
        {
            viewModel.CalculationResultText = "0";
            viewModel.FormulaText = "0";
            viewModel.OnSaveValue = (double calculationResult) =>
            {
                var subcat = (RealSubcat)data.Subcat;
                subcat.AddValue(calculationResult, selectedDate);

                HideCalculator();
                MainBudget.Instance.Save();
                SetupBudgetSummary();

                //if month changed
                //var budgetMonth = MainBudget.Instance.GetMonth(selectedDate);
                //var subcatNew = (RealSubcat)budgetMonth.BudgetReal.Categories[data.Category.Id].subcats[data.Subcat.Id];
                //subcatNew.AddValue(calculationResult, selectedDate); 
            };

            ShowCalculator();

            Description.Text = subcatName;

            CultureInfo cultureInfoPL = new CultureInfo("pl-PL");
            DateButton.Text = selectedDate.ToString("dd.MM.yyyy", cultureInfoPL);
        }

        private void SelectedDateChanged(object sender, CalendarTappedEventArgs args)
        {
            selectedDate = args.datetime;
            CultureInfo cultureInfoPL = new CultureInfo("pl-PL");
            DateButton.Text = selectedDate.ToString("dd.MM.yyyy", cultureInfoPL);
            BudgetDatePicker.IsVisible = false;
        }

        private void OnValueEdited(object sender, EventArgs e)
        {
            //var formula = ((Entry)sender).Text;
            //viewModel.FormulaText = formula;
            viewModel.ForceCalculateFormula();
        }

        private async void OnDropboxClick(object sender, EventArgs e)
        {
            this.oauth2State = Guid.NewGuid().ToString("N");
            var authorizeUri = DropboxOAuth2Helper.GetAuthorizeUri(OAuthResponseType.Token, appKey, new Uri(RedirectUri), state: oauth2State);

            var webView = new WebView { Source = new UrlWebViewSource { Url = authorizeUri.AbsoluteUri } };
            webView.Navigating += this.WebViewOnNavigating;
            var contentPage = new ContentPage { Content = webView };
            await Navigation.PushModalAsync(contentPage);
        }

        private async void WebViewOnNavigating(object sender, WebNavigatingEventArgs e)
        {
            if (!e.Url.StartsWith(RedirectUri, StringComparison.OrdinalIgnoreCase))
            {
                // we need to ignore all navigation that isn't to the redirect uri.
                return;
            }

            try
            {
                var result = DropboxOAuth2Helper.ParseTokenFragment(new Uri(e.Url));

                if (result.State != this.oauth2State)
                {
                    return;
                }

                Helpers.Settings.DropboxAccessToken = result.AccessToken;
                DropboxManager.Instance.Init();
                //DropboxLoginElement.IsVisible = false;
                DropboxButtonChangeVisibility(false);
                await DropboxManager.Instance.DownloadData();
            }
            catch (ArgumentException argExc)
            {
                string msg = argExc.Message;
                msg += "error";
                // There was an error in the URI passed to ParseTokenFragment
            }
            finally
            {
                e.Cancel = true;
                await Application.Current.MainPage.Navigation.PopModalAsync();
            }
        }
    }
}
