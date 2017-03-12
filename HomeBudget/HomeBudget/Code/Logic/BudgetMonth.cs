using Syncfusion.SfChart.XForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Code
{
	public class BudgetMonth
	{
		private List<BudgetCategory> categories;
		private int month;
		public int Month
		{
			get { return month; }
		}

		private int year;
		public int Year
		{
			get { return year; }
		}

        public class BudgetChartData
        {
            public string CategoryName { get; set; }
            public float ExpensesSum { get; set; }

            public BudgetChartData(string name, float sum)
            {
                CategoryName = name;
                ExpensesSum = sum;
            }
        }

		public static BudgetMonth Create(List<BudgetCategoryTemplate> categories, DateTime date)
		{
			BudgetMonth month = new BudgetMonth();
			month.SetupCategories(categories);
			month.SetupDate(date);

			return month;
		}

		public void AddExpense(float value, int categoryID, int subcategoryID, int dayOfMonth)
		{
			if (categoryID < categories.Count)
				categories[categoryID].AddExpense(value, subcategoryID, dayOfMonth);
		}

		private BudgetMonth()
		{

		}

		private void SetupDate(DateTime date)
		{
			month = date.Month;
			year = date.Year;
		}

		private void SetupCategories(List<BudgetCategoryTemplate> categoriesDesc)
		{
			categories = new List<BudgetCategory>();

			foreach(BudgetCategoryTemplate category in categoriesDesc)
			{
				BudgetCategory budgetCategory = BudgetCategory.Create(category);
				budgetCategory.Name = category.Name;
				categories.Add(budgetCategory);
			}
		}

		public ObservableCollection<BudgetChartData> GetData()
		{
			ObservableCollection<BudgetChartData> monthData = new ObservableCollection<BudgetChartData>();
			foreach (BudgetCategory category in categories)
			{
				monthData.Add(new BudgetChartData(category.Name, category.GetExpensesSum()));
			}

			return monthData;
		}

	}
}
