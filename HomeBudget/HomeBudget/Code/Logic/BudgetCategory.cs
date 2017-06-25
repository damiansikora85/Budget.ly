using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Code
{
	public class BudgetCategory
	{
		private List<BudgetSubcategory> subcategories;
        public List<BudgetSubcategory> Subcategories
        {
            get { return subcategories; }
        }
		public string Name;
        public int Id;

        private BudgetCategory()
        {
            subcategories = new List<BudgetSubcategory>();
        }

		public static BudgetCategory Create(BudgetCategoryTemplate categoryDesc)
		{
			BudgetCategory budgetCategory = new BudgetCategory();
            budgetCategory.Name = categoryDesc.Name;
            budgetCategory.Id = categoryDesc.Id;
			budgetCategory.SetupSubCategories(categoryDesc.subcategories);

			return budgetCategory;
		}

        public static BudgetCategory CreateFromBinaryData(BinaryData binaryData)
        {
            BudgetCategory budgetCategory = new BudgetCategory();
            budgetCategory.Deserialize(binaryData);

            return budgetCategory;
        }

		private void SetupSubCategories(List<string> subcategoriesDesc)
		{
			foreach(string sub in subcategoriesDesc)
			{
				BudgetSubcategory subcategory = new BudgetSubcategory();
				subcategories.Add(subcategory);
			}
		}

		public void AddExpense(double value, int subcategoryID, DateTime date)
		{
			if (subcategoryID < subcategories.Count)
				subcategories[subcategoryID].AddExpense(value, date.Day);
		}

        public void SetPlannedExpense(float value, int subcatID)
        {
            if (subcatID < subcategories.Count)
                subcategories[subcatID].SetPlannedExpense(value);
        }

        public double GetExpensesSum()
		{
			double result = 0;
			foreach (BudgetSubcategory subcategory in subcategories)
				result += subcategory.GetSum();
			return result;
		}

        public byte[] Serialize()
        {
            List<byte> bytes = new List<byte>();

            bytes.AddRange(BinaryData.GetBytes(Name));
            bytes.AddRange(BitConverter.GetBytes(Id));
            bytes.AddRange(BitConverter.GetBytes(subcategories.Count));
            foreach (BudgetSubcategory subcategory in subcategories)
                bytes.AddRange(subcategory.Serialize());

            return bytes.ToArray();
        }

        private void Deserialize(BinaryData binaryData)
        {
            Name = binaryData.GetString();
            Id = binaryData.GetInt();
            int subcatNum = binaryData.GetInt();

            for (int i = 0; i < subcatNum; i++)
            {
                BudgetSubcategory subcategory = new BudgetSubcategory();
                subcategory.Deserialize(binaryData);
                subcategories.Add(subcategory);
            }
        }

        public double GetTotalExpense()
        {
            double expenseSum = 0;

            foreach (BudgetSubcategory subcat in subcategories)
                expenseSum += subcat.GetSum();

            return expenseSum;
        }
    }
}
