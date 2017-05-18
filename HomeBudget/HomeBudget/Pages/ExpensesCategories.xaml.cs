using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Rg.Plugins.Popup.Extensions;
using System.ComponentModel;
using System.Windows.Input;
using HomeBudget.Code;

namespace HomeBudget
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExpensesCategories : ContentPage
    {
        private const int CATEGORIES_MAX_COLS = 3;
        private const int SUBCATEGORIES_MAX_COLS = 3;
        private const int CATEGORY_GRID_HEIGHT = 100;
        private const int SUBCATEGORY_GRID_HEIGHT = 60;

        private Code.BudgetDescription budgetDescription;
        private Code.BudgetView budgetView;
        private List<Code.CategoryView> categoryViews = new List<Code.CategoryView>();
        private ExpensesCategoriesViewModel expensesCategoriesViewModel;

        public ExpensesCategories()
        {
            InitializeComponent();

            expensesCategoriesViewModel = new ExpensesCategoriesViewModel();
            expensesCategoriesViewModel.Navigation = Navigation;
            BindingContext = expensesCategoriesViewModel;

            CreateCategoriesView();
        }

        protected override void OnAppearing()
        {
            expensesCategoriesViewModel.OnAppearing();
        }

        private void CreateCategoriesView()
        {
            budgetView = new Code.BudgetView();
            budgetDescription = Code.MainBudget.Instance.BudgetDescription;
            int numCategories = budgetDescription.Categories.Count;
            int categoriesGridNum = numCategories / CATEGORIES_MAX_COLS;
            if (numCategories % CATEGORIES_MAX_COLS > 0)
                categoriesGridNum++;

            for (int i = 0; i < categoriesGridNum; i++)
            {
                //create category grid
                Grid categoryGrid = CreateCategoryGrid();

                MainLayout.Children.Add(categoryGrid);

                //fill category grid
                FillCategoryGrid(categoryGrid, i * CATEGORIES_MAX_COLS);
            }
        }

        private Grid CreateCategoryGrid()
        {
            Grid grid = new Grid();

            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(CATEGORY_GRID_HEIGHT, GridUnitType.Absolute) });

            for (int i = 0; i < CATEGORIES_MAX_COLS; i++)
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            return grid;
        }

        private void FillCategoryGrid(Grid grid, int startCategoryID)
        {
            for (int i = 0; i < CATEGORIES_MAX_COLS; i++)
            {
                if (i + startCategoryID < budgetDescription.Categories.Count)
                {
                    Button categoryButton = new Button();
                    Code.CategoryView categoryView = new Code.CategoryView();
                    Code.BudgetCategoryTemplate category = budgetDescription.Categories[startCategoryID + i];
                    categoryButton.Text = category.Name;
                    categoryButton.Clicked += categoryView.OnClick;
                    categoryView.OnClickBtn += budgetView.OnClickCategory;
                    categoryButton.Style = App.Current.Resources["ButtonStyle"] as Style;

                    grid.Children.Add(categoryButton, i, 0);
                    categoryViews.Add(categoryView);

                    Grid subGrid = CreateSubGrid(category.subcategories.Count);
                    categoryView.subGrid = subGrid;

                    FillSubGrid(subGrid, category);
                    subGrid.IsVisible = false;

                    MainLayout.Children.Add(subGrid);
                }
            }
        }

        private Grid CreateSubGrid(int numSubcategories)
        {
            Grid grid = new Grid();

            int subcategoriesRowsNum = numSubcategories / SUBCATEGORIES_MAX_COLS;
            if (numSubcategories % SUBCATEGORIES_MAX_COLS > 0)
                subcategoriesRowsNum++;

            for (int i = 0; i < subcategoriesRowsNum; i++)
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(SUBCATEGORY_GRID_HEIGHT, GridUnitType.Absolute) });

            for (int i = 0; i < SUBCATEGORIES_MAX_COLS; i++)
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            return grid;
        }

        private void FillSubGrid(Grid grid, Code.BudgetCategoryTemplate category)
        {
            int row = 0;
            int col = 0;

            for (int i = 0; i < category.subcategories.Count; i++)
            {
                Button btn = new Button();
                string subcategoryName = category.subcategories[i];
                btn.Text = subcategoryName;
                btn.Command = expensesCategoriesViewModel.ButtonClicked;
                btn.CommandParameter = CreateExpenseData(category, subcategoryName, i);
                btn.Style = App.Current.Resources["ButtonStyleSub"] as Style;

                grid.Children.Add(btn, col, row);

                col++;
                if (col >= SUBCATEGORIES_MAX_COLS)
                {
                    row++;
                    col = 0;
                }
            }
        }

        private Code.ExpenseSaveData CreateExpenseData(BudgetCategoryTemplate category, string subcategoryName, int subcategoryID)
        {
            Code.ExpenseSaveData expenseData = new Code.ExpenseSaveData()
            {
                Category = new CategoryData(category.Name, category.Id),
                Subcategory = new CategoryData(subcategoryName, subcategoryID),
                Date = DateTime.Now
            };
            return expenseData;
        }

        private async void OnBack(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }



    public class ExpensesCategoriesViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand ButtonClicked { get; private set; }
        public INavigation Navigation;

        private bool shouldAutoLaunchPopup;

        public ExpensesCategoriesViewModel()
        {
            ButtonClicked = new Command<Code.ExpenseSaveData>(HandleButtonClicked);
            shouldAutoLaunchPopup = false;
        }

        private async void HandleButtonClicked(Code.ExpenseSaveData expenseSaveData)
        {
            Code.MainBudget.Instance.CurrentExpenseSaveData = expenseSaveData;
            //var popup = new AddExpensePopup(SetAutoLaunchPopup);
            //await Navigation.PushPopupAsync(popup);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SetAutoLaunchPopup()
        {
            shouldAutoLaunchPopup = true;
        }

        public async void OnAppearing()
        {
            if (shouldAutoLaunchPopup)
            {
                shouldAutoLaunchPopup = false;
                //var popup = new AddExpensePopup(SetAutoLaunchPopup);
                //await Navigation.PushPopupAsync(popup);
            }
        }
    }
}
