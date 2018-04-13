﻿using HomeBudget.Code.Logic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HomeBudget.Code
{
    public class BudgetMonth
	{
        public BudgetPlanned BudgetPlanned { get; private set; }
        public BudgetReal BudgetReal { get; private set; }

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

        public event Action onBudgetPlannedChanged;

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
            BudgetReal.AddExpense(value, date, categoryID, subcatID);
        }

        public void AddIncome(double value, DateTime date, int incomeCategoryID)
        {
            BudgetReal.AddIncome(value, date, incomeCategoryID);
        }

        public void SetPlannedIncome(float value, int incomeCategoryID)
        {
            //GetIncomeByID(incomeCategoryID).SetPlannedIncome(value);
        }

        public void SetPlannedExpense(float value, int categoryID, int subcatID)
        {
            //GetCategoryByID(categoryID).SetPlannedExpense(value, subcatID);
        }

		private BudgetMonth()
		{
            BudgetReal = new BudgetReal();
            BudgetPlanned = new BudgetPlanned();
        }

        private void OnBudgetPlannedChanged()
        {
            onBudgetPlannedChanged();
        }

        private void SetupDate(DateTime date)
		{
			month = date.Month;
			year = date.Year;
		}

		private void SetupCategories(List<BudgetCategoryTemplate> categoriesDesc)
		{
            BudgetPlanned.Setup(categoriesDesc);
            BudgetReal.Setup(categoriesDesc);
		}

        public ObservableCollection<BudgetChartData> GetData()
		{
			ObservableCollection<BudgetChartData> monthData = new ObservableCollection<BudgetChartData>();
			/*foreach (ExpenseCategory category in Categories)
			{
				monthData.Add(new BudgetChartData(category.Name, category.GetExpensesSum()));
			}*/

			return monthData;
		}

        /*public ExpenseCategory GetCategory(int id)
        {
            return GetCategoryByID(id);
        }*/

        public byte[] Serialize()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(month));
            bytes.AddRange(BitConverter.GetBytes(year));
            bytes.AddRange(BudgetPlanned.Serialize());
            bytes.AddRange(BudgetReal.Serialize());

            return bytes.ToArray();
        }

        private void Deserialize(BinaryData binaryData)
        {
            month = binaryData.GetInt();
            year = binaryData.GetInt();
            BudgetPlanned.Deserialize(binaryData);
            BudgetReal.Deserialize(binaryData);
        }

        public double GetTotalIncomeReal()
        {
            return BudgetReal.GetTotalIncome();
        }

        public double GetTotalExpenseReal()
        {
            return BudgetReal.GetTotalExpenses();
        }

        public double GetTotalExpensesPlanned()
        {
            return BudgetPlanned.GetTotalExpenses();
        }

        public double GetTotalIncomePlanned()
        {
            return BudgetPlanned.GetTotalIncome();
        }

        public void UpdatePlannedBudget(BudgetPlanned newBudgetPlanned)
        {
            BudgetPlanned = new BudgetPlanned(newBudgetPlanned);
        }
    }
}