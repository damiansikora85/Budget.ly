using HomeBudget.Code.Logic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ProtoBuf;

namespace HomeBudget.Code
{
    [ProtoContract]
    public class BudgetMonth
	{
        [ProtoMember(1)]
        public BudgetPlanned BudgetPlanned { get; set; }
        [ProtoMember(2)]
        public BudgetReal BudgetReal { get; set; }
        [ProtoMember(3)]
        public int Month { get; set; }
        [ProtoMember(4)]
        public int Year { get; set; }

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

		public static BudgetMonth Create(List<BudgetCategoryTemplate> categories, DateTime date)
		{
			var month = new BudgetMonth();
			month.SetupCategories(categories);
            month.SetupDate(date);

			return month;
		}

        public static BudgetMonth CreateFromBinaryData(BinaryData binaryData)
        {
            var month = new BudgetMonth();
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
			Month = date.Month;
			Year = date.Year;
		}

		private void SetupCategories(List<BudgetCategoryTemplate> categoriesDesc)
		{
            BudgetPlanned.Setup(categoriesDesc);
            BudgetReal.Setup(categoriesDesc);
		}

        public ObservableCollection<BudgetChartData> GetData()
		{
			var monthData = new ObservableCollection<BudgetChartData>();
			/*foreach (ExpenseCategory category in Categories)
			{
				monthData.Add(new BudgetChartData(category.Name, category.GetExpensesSum()));
			}*/

			return monthData;
		}

        public byte[] Serialize()
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Month));
            bytes.AddRange(BitConverter.GetBytes(Year));
            bytes.AddRange(BudgetPlanned.Serialize());
            bytes.AddRange(BudgetReal.Serialize());

            return bytes.ToArray();
        }

        private void Deserialize(BinaryData binaryData)
        {
            Month = binaryData.GetInt();
            Year = binaryData.GetInt();
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
