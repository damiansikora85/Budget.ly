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
    public partial class MainPagePC : ContentPage
    {
        private const string TEMPLATE_FILENAME = "HomeBudget.template.json";
        private string selectedCategoryName;
        private int selectedCategoryId;
        private CategoryElement lastCategorySelected;
        private bool shouldAutoLaunchPopup;

        public ICommand SubcatClicked { get; private set; }

        public MainPagePC()
        {
            InitializeComponent();
            //BindingContext = new MainPagePCViewModel();

            //SubcatClicked = new Command<Code.ExpenseSaveData>(HandleSubcatClicked);

            InitBudget();
            CreateCategoriesBar();
        }

        private void InitBudget()
        {
            var assembly = typeof(MainPage).GetTypeInfo().Assembly;

            Stream stream = assembly.GetManifestResourceStream(TEMPLATE_FILENAME);
            string jsonString = "";
            using (var reader = new System.IO.StreamReader(stream))
            {
                jsonString = reader.ReadToEnd();
                MainBudget.Instance.InitWithJson(jsonString);
            }
        }

        private async void OnPlanClick(object sender, EventArgs args)
        {

        }

        private async void OnAnalyticsClick(object sender, EventArgs e)
        {
            NavigationPage analizePage = new NavigationPage(new AnalyticsPagePC());
            await Navigation.PushModalAsync(analizePage);
        }

        protected override void OnAppearing()
        {
            if (shouldAutoLaunchPopup)
            {
                shouldAutoLaunchPopup = false;
                var popup = new AddExpensePopup(selectedCategoryName, "test", DateTime.Now, SetAutoLaunchPopup);
                Navigation.PushPopupAsync(popup);
            }
        }

        private async void OnIncomeClick(object sender, EventArgs e)
        {   
            /*if (show)
                await categoriesScroll.TranslateTo(-150, 0);
            else
                await categoriesScroll.TranslateTo(0, 0);

            show = !show;*/
        }

        private async void OnExpenseClick(object sender, EventArgs e)
        {
            await categoriesScroll.TranslateTo(0, 0);
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
            selectedCategoryName = categoryElement.Name;
            selectedCategoryId = categoryElement.Id;
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

        private ExpenseSaveData CreateExpenseData(BudgetCategoryTemplate category, string subcategoryName, int subcategoryID)
        {
            ExpenseSaveData expenseData = new Code.ExpenseSaveData()
            {
                Category = new CategoryData(category.Name, category.Id),
                Subcategory = new CategoryData(subcategoryName, subcategoryID),
                Date = DateTime.Now
            };
            return expenseData;
        }

        private async Task OnSubcatClicked(int id, CategoryElement element)
        {
            ExpenseSaveData expenseData = new Code.ExpenseSaveData()
            {
                Category = new CategoryData(selectedCategoryName, selectedCategoryId),
                Subcategory = new CategoryData(element.Name, element.Id),
                Date = DateTime.Now
            };

            Code.MainBudget.Instance.CurrentExpenseSaveData = expenseData;

            await subcat.TranslateTo(-185, 0);
            await categoriesScroll.TranslateTo(-150, 0);

            var popup = new AddExpensePopup(selectedCategoryName, element.Name, DateTime.Now, SetAutoLaunchPopup);
            await Navigation.PushPopupAsync(popup);   
        }

        public void SetAutoLaunchPopup()
        {
            shouldAutoLaunchPopup = true;
        }
    }

    class MainPagePCViewModel : INotifyPropertyChanged
    {

        public MainPagePCViewModel()
        {
            IncreaseCountCommand = new Command(IncreaseCount);
        }

        int count;

        string countDisplay = "You clicked 0 times.";
        public string CountDisplay
        {
            get { return countDisplay; }
            set { countDisplay = value; OnPropertyChanged(); }
        }

        public ICommand IncreaseCountCommand { get; }

        void IncreaseCount() =>
            CountDisplay = $"You clicked {++count} times";


        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName]string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
