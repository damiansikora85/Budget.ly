using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Code
{
	public class BudgetCategory
	{
		private List<BudgetSubcategory> subcategories;
		public string Name;

		public static BudgetCategory Create(BudgetCategoryTemplate categoryDesc)
		{
			BudgetCategory budgetCategory = new BudgetCategory();
			budgetCategory.SetupSubCategories(categoryDesc.subcategories);

			return budgetCategory;
		}

		private void SetupSubCategories(List<string> subcategoriesDesc)
		{
			subcategories = new List<BudgetSubcategory>();

			foreach(string sub in subcategoriesDesc)
			{
				BudgetSubcategory subcategory = new BudgetSubcategory();
				subcategories.Add(subcategory);
			}
		}

		public void AddExpense(float value, int subcategoryID, int dayOfMonth)
		{
			if (subcategoryID < subcategories.Count)
				subcategories[subcategoryID].AddExpense(value, dayOfMonth);
		}

		public float GetExpensesSum()
		{
			float result = 0;
			foreach (BudgetSubcategory subcategory in subcategories)
				result += subcategory.GetSum();
			return result;
		}
	}
}
