using Newtonsoft.Json;
using Syncfusion.SfChart.XForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Code
{
	public class MainBudget
	{
		private List<BudgetMonth> months;
		private int currentCategoryID;
		private int currentSubcategoryID;

		private BudgetDescription budgetDescription;
		public BudgetDescription BudgetDescription
		{
			get { return budgetDescription; }
		}

		static MainBudget instance;
		public static MainBudget Instance
		{
			get
			{
				if (instance == null)
					instance = new MainBudget();

				return instance;
			}
		}

		private MainBudget()
		{
			months = new List<BudgetMonth>();
		}

		public void InitWithJson(string jsonString)
		{
			budgetDescription = JsonConvert.DeserializeObject<BudgetDescription>(jsonString);
		}

        public void Save()
        {

        }

        public void Load()
        {
              
        }

		public void SetCurrentExpenseData(int categoryID, int subcategoryID)
		{
			currentCategoryID = categoryID;
			currentSubcategoryID = subcategoryID;
		}

		public void AddExpense(DateTime date, float value)
		{
			BudgetMonth month = GetMonth(date);
			month.AddExpense(value, currentCategoryID, currentSubcategoryID, date.Day);
		}

		private BudgetMonth GetMonth(DateTime date)
		{
			BudgetMonth month = months.Find(x => x.Month == date.Month && x.Year == date.Year);
			if (month == null)
			{
				month = BudgetMonth.Create(budgetDescription.Categories, date);
				months.Add(month);
			}

			return month;
		}

		public ObservableCollection<BudgetMonth.BudgetChartData> GeCurrentMonthData()
		{
			return GetMonth(DateTime.Now).GetData();
		}
	}
}
