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
        private List<BudgetIncome> incomes;
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

		public static BudgetMonth Create(List<BudgetCategoryTemplate> categories, List<BudgetIncomeTemplate> incomes, DateTime date)
		{
			BudgetMonth month = new BudgetMonth();
			month.SetupCategories(categories);
            month.SetupIncomes(incomes);
			month.SetupDate(date);

			return month;
		}

        public static BudgetMonth CreateFromBinaryData(BinaryData binaryData)
        {
            BudgetMonth month = new BudgetMonth();
            month.Deserialize(binaryData);

            return month;
        }

        public void AddExpense(double value, DateTime date, int categoryID, int subcatID)
        {
            GetCategoryByID(categoryID).AddExpense(value, subcatID, date);
        }

        public void AddIncome(double value, DateTime date, int incomeCategoryID)
        {
            GetIncomeByID(incomeCategoryID).AddIncome(value, date);
        }

        public void SetPlannedIncome(float value, int incomeCategoryID)
        {
            GetIncomeByID(incomeCategoryID).SetPlannedIncome(value);
        }

        public void SetPlannedExpense(float value, int categoryID, int subcatID)
        {
            GetCategoryByID(categoryID).SetPlannedExpense(value, subcatID);
        }

        private BudgetCategory GetCategoryByID(int id)
        {
            BudgetCategory category = categories.Find(element => element.Id == id);
            return category;
        }

        private BudgetIncome GetIncomeByID(int incomeID)
        {
            BudgetIncome income = incomes.Find(element => element.Id == incomeID);
            return income;
        }

		private BudgetMonth()
		{
            categories = new List<BudgetCategory>();
            incomes = new List<BudgetIncome>();
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

        private void SetupIncomes(List<BudgetIncomeTemplate> incomesDesc)
        {
            foreach(BudgetIncomeTemplate incomeDesc in incomesDesc)
            {
                BudgetIncome income = BudgetIncome.Create(incomeDesc);
                incomes.Add(income);
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

        public BudgetCategory GetCategory(int id)
        {
            return GetCategoryByID(id);
        }

        public byte[] Serialize()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(month));
            bytes.AddRange(BitConverter.GetBytes(year));
            bytes.AddRange(BitConverter.GetBytes(categories.Count));
            foreach (BudgetCategory category in categories)
                bytes.AddRange(category.Serialize());

            bytes.AddRange(BitConverter.GetBytes(incomes.Count));
            foreach (BudgetIncome income in incomes)
                bytes.AddRange(income.Serialize());

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

            int incomesNum = binaryData.GetInt();
            for(int i=0; i< incomesNum; i++)
            {
                BudgetIncome income = BudgetIncome.CreateWithBinaryData(binaryData);
                incomes.Add(income);
            }
        }

        public double GetTotalIncome()
        {
            double totalIncome = 0.0;
            foreach (BudgetIncome income in incomes)
                totalIncome += income.GetTotal();

            return totalIncome;
        }

        public double GetTotalExpense()
        {
            double totalExpense = 0.0;
            foreach (BudgetCategory expenseCategory in categories)
                totalExpense += expenseCategory.GetTotalExpense();

            return totalExpense;
        }
    }
}
