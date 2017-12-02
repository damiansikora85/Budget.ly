using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Code.Logic.BaseBudget
{
    public class BaseBudget : INotifyPropertyChanged
    {
        public ObservableCollection<BaseBudgetCategory> Categories { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public BaseBudget()
        {
            Categories = new ObservableCollection<BaseBudgetCategory>();
        }

        public byte[] Serialize()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Categories.Count));
            foreach (BaseBudgetCategory category in Categories)
            {
                bytes.AddRange(category.Serialize());
            }

            return bytes.ToArray();
        }

        public List<BaseBudgetCategory> GetIncomesCategories()
        {
            return Categories.Where<BaseBudgetCategory>((elem) => elem.IsIncome == true).ToList();
        }

        public List<BaseBudgetCategory> GetExpensesCategories()
        {
            return Categories.Where<BaseBudgetCategory>((elem) => elem.IsIncome == false).ToList();
        }

        public BaseBudgetCategory GetBudgetCategory(int categoryId)
        {
            return Categories.Where(elem => elem.Id == categoryId).FirstOrDefault();
        }

        public double GetTotalIncome()
        {
            List<BaseBudgetCategory> incomes = GetIncomesCategories();
            return incomes.Sum(elem => elem.TotalValues);
        }

        public double GetTotalExpenses()
        {
            List<BaseBudgetCategory> expenses = GetExpensesCategories();
            return expenses.Sum(elem => elem.TotalValues);
        }
    }
}
