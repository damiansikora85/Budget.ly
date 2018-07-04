using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace HomeBudget.Code.Logic.BaseBudget
{
    [ProtoContract]
    [ProtoInclude(7, typeof(BudgetPlanned))]
    [ProtoInclude(8, typeof(BudgetReal))]
    public class BaseBudget : INotifyPropertyChanged
    {
        [ProtoMember(1)]
        public ObservableCollection<BaseBudgetCategory> Categories { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public BaseBudget()
        {
            Categories = new ObservableCollection<BaseBudgetCategory>();
        }

        protected void OnCategoryModified(object sender, PropertyChangedEventArgs e)
        {
            
        }

        public void Prepare()
        {
            foreach (var category in Categories)
                category.Prepare();
        }

        public List<BaseBudgetCategory> GetIncomesCategories()
        {
            return Categories.Where((elem) => elem.IsIncome).ToList();
        }

        public List<BaseBudgetCategory> GetExpensesCategories()
        {
            return Categories.Where((elem) => !elem.IsIncome).ToList();
        }

        public BaseBudgetCategory GetBudgetCategory(int categoryId)
        {
            return Categories.FirstOrDefault(elem => elem.Id == categoryId);
        }

        public double GetTotalIncome()
        {
            var incomes = GetIncomesCategories();
            return incomes.Sum(elem => elem.TotalValues);
        }

        public double GetTotalExpenses()
        {
            var expenses = GetExpensesCategories();
            return expenses.Sum(elem => elem.TotalValues);
        }
    }
}
