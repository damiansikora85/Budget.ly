using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using Xamarin.Forms;
using Rg.Plugins.Popup.Extensions;

namespace HomeBudget
{
	public partial class JsonCategories : ContentPage
	{
		private const int CATEGORIES_MAX_COLS = 3;
		private const int SUBCATEGORIES_MAX_COLS = 3;
		private const int CATEGORY_GRID_HEIGHT = 100;
		private const int SUBCATEGORY_GRID_HEIGHT = 60;
		private Code.BudgetDescription budgetDescription;
		private Code.BudgetView budgetView;
		private List<Code.CategoryView> categoryViews = new List<Code.CategoryView>();

		public JsonCategories()
		{
			InitializeComponent();
			CreateCategoriesView();
		}

		private void CreateCategoriesView()
		{
			budgetView = new Code.BudgetView();
			budgetDescription = Code.MainBudget.Instance.BudgetDescription;
			int numCategories = budgetDescription.Categories.Count;
			int categoriesGridNum = numCategories / CATEGORIES_MAX_COLS;
			if (numCategories % CATEGORIES_MAX_COLS > 0)
				categoriesGridNum++;

			for(int i=0; i<categoriesGridNum; i++)
			{
				//create category grid
				Grid categoryGrid = CreateCategoryGrid();

				MainLayout.Children.Add(categoryGrid);

				//fill category grid
				FillCategoryGrid(categoryGrid, i*CATEGORIES_MAX_COLS);		
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

					FillSubGrid(subGrid, category, startCategoryID + i);
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

			for(int i=0; i<subcategoriesRowsNum; i++)
				grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(SUBCATEGORY_GRID_HEIGHT, GridUnitType.Absolute) });

			for (int i = 0; i < SUBCATEGORIES_MAX_COLS; i++)
				grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

			return grid;
		}

		private void FillSubGrid(Grid grid, Code.BudgetCategoryTemplate category, int categoryID)
		{
			int row = 0;
			int col = 0;

			for (int i = 0; i < category.subcategories.Count; i++)
			{
				Button btn = new Button();
				Code.SubcategoryView subcategoryView = new Code.SubcategoryView();
				subcategoryView.Category = categoryID;
				subcategoryView.Subcategory = i;
				string subcategoryName = category.subcategories[i];
				btn.Text = subcategoryName;
				btn.Clicked += OnAddExpense;
				btn.Clicked += subcategoryView.OnClick;
				btn.Style = App.Current.Resources["ButtonStyleSub"] as Style;

				grid.Children.Add(btn, col, row);

				col++;
				if(col >= SUBCATEGORIES_MAX_COLS)
				{
					row++;
					col = 0;
				}
			}
		}

        private async void OnBack(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }


        private async void OnAddExpense(object sender, EventArgs e)
		{
			var page = new AddExpensePopup();
			var viewModel = new AddExpenseViewModel();
			page.BindingContext = viewModel;

			await Navigation.PushPopupAsync(page);
		}
	}
}
