using Syncfusion.SfChart.XForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;
using System.Diagnostics;

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
            public double ExpensesSum { get; set; }

            public BudgetChartData(string name, double sum)
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

        public static BudgetMonth CreateFromBinaryData(BinaryData binaryData)
        {
            BudgetMonth month = new BudgetMonth();
            month.Deserialize(binaryData);

            return month;
        }

		public void AddExpense(float value, ExpenseSaveData expenseSaveData)
		{
            GetCategoryByID(expenseSaveData.Category.Id).AddExpense(value, expenseSaveData.Subcategory.Id, expenseSaveData.Date);
		}

        private BudgetCategory GetCategoryByID(int id)
        {
            BudgetCategory category = categories.Find(element => element.Id == id);
            return category;
        }

		private BudgetMonth()
		{
            categories = new List<BudgetCategory>();
        }

		private void SetupDate(DateTime date)
		{
			month = date.Month;
			year = date.Year;
		}

		private void SetupCategories(List<BudgetCategoryTemplate> categoriesDesc)
		{
			foreach(BudgetCategoryTemplate category in categoriesDesc)
			{
				BudgetCategory budgetCategory = BudgetCategory.Create(category);
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

        public byte[] Serialize()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(month));
            bytes.AddRange(BitConverter.GetBytes(year));
            bytes.AddRange(BitConverter.GetBytes(categories.Count));
            foreach (BudgetCategory category in categories)
                bytes.AddRange(category.Serialize());

            return bytes.ToArray();
        }

        private void Deserialize(BinaryData binaryData)
        {
            month = binaryData.GetInt();
            year = binaryData.GetInt();
            int categoriesNum = binaryData.GetInt();

            for(int i=0; i<categoriesNum; i++)
            {
                BudgetCategory budgetCategory = BudgetCategory.CreateFromBinaryData(binaryData);
                categories.Add(budgetCategory);
            }
        }
	}
}
