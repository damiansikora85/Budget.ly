using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Code.Logic
{
    public class BudgetReal
    {
        private List<BudgetRealCategory> categories;

        public BudgetReal()
        {
            categories = new List<BudgetRealCategory>();
        }

        public void Setup(List<BudgetCategoryTemplate> categoriesDesc)
        {
            foreach (BudgetCategoryTemplate categoryDesc in categoriesDesc)
            {
                BudgetRealCategory realCategory = BudgetRealCategory.Create(categoryDesc);
                categories.Add(realCategory);
            }
        }

        public byte[] Serialize()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(categories.Count));
            foreach (BudgetRealCategory category in categories)
            {
                bytes.AddRange(category.Serialize());
            }

            return bytes.ToArray();
        }

        public void Deserialize(BinaryData binaryData)
        {
            categories.Clear();
            int categoriesNum = binaryData.GetInt();
            for (int i = 0; i < categoriesNum; i++)
            {
                BudgetRealCategory category = new BudgetRealCategory();
                category.Deserialize(binaryData);
                categories.Add(category);
            }
        }

        private BudgetRealCategory GetBudgetCategory(int categoryId) 
        {
            return categories.Find(elem => elem.Id == categoryId);
        }

        private List<BudgetRealCategory> GetIncomesCategories()
        {
            return categories.FindAll((elem) => elem.IsIncome == true);
        }

        public void AddExpense(double value, DateTime date, int categoryId, int subcatId)
        {
            GetBudgetCategory(categoryId).AddValue(value, date, subcatId);
        }

        public void AddIncome(double value, DateTime date, int incomeCategoryId)
        {
            GetIncomesCategories()[0].AddValue(value, date, incomeCategoryId);
        }

        private List<BudgetRealCategory> GetExpensesCategories()
        {
            return categories.FindAll((elem) => elem.IsIncome == false);
        }

        public double GetTotalIncome()
        {
            List<BudgetRealCategory> incomes = GetIncomesCategories();
            return incomes.Sum(elem => elem.TotalValues);
        }

        public double GetTotalExpenses()
        {
            List<BudgetRealCategory> expenses = GetExpensesCategories();
            return expenses.Sum(elem => elem.TotalValues);
        }
    }
}
