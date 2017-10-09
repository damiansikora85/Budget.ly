using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Code.Logic
{
    public class BudgetPlanned : INotifyPropertyChanged
    {
        public ObservableCollection<BudgetPlannedCategory> Categories { get; private set; }

        public BudgetPlanned()
        {
            Categories = new ObservableCollection<BudgetPlannedCategory>();
        }

        public BudgetPlanned(BudgetPlanned budgetPlanned)
        {
            Categories = new ObservableCollection<BudgetPlannedCategory>();

            foreach (BudgetPlannedCategory category in budgetPlanned.Categories)
            {
                BudgetPlannedCategory newCategory = new BudgetPlannedCategory(category);
                newCategory.PropertyChanged += OnBudgetPlannedChanged;
                Categories.Add(newCategory);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Setup(List<BudgetCategoryTemplate> categoriesDesc)
        {
            foreach (BudgetCategoryTemplate categoryDesc in categoriesDesc)
            {
                BudgetPlannedCategory plannedCategory = BudgetPlannedCategory.Create(categoryDesc);
                plannedCategory.PropertyChanged += OnBudgetPlannedChanged;
                Categories.Add(plannedCategory);
            }
        }

        public byte[] Serialize()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Categories.Count));
            foreach (BudgetPlannedCategory category in Categories)
            {
                bytes.AddRange(category.Serialize());
            }

            return bytes.ToArray();
        }

        public void Deserialize(BinaryData binaryData)
        {
            Categories.Clear();
            int categoriesNum = binaryData.GetInt();
            for(int i=0; i<categoriesNum; i++)
            {
                BudgetPlannedCategory category = new BudgetPlannedCategory();
                category.Deserialize(binaryData);
                category.PropertyChanged += OnBudgetPlannedChanged;
                Categories.Add(category);
            }
        }

        public List<BudgetPlannedCategory> GetIncomesCategories()
        {
            return Categories.Where<BudgetPlannedCategory>((elem) => elem.IsIncome == true).ToList();
        }

        public List<BudgetPlannedCategory> GetExpensesCategories()
        {
            return Categories.Where<BudgetPlannedCategory>((elem) => elem.IsIncome == false).ToList();
        }

        public double GetTotalIncome()
        {
            List<BudgetPlannedCategory> incomes = GetIncomesCategories();
            return incomes.Sum(elem => elem.TotalValues);
        }

        public double GetTotalExpenses()
        {
            List<BudgetPlannedCategory> expenses = GetExpensesCategories();
            return expenses.Sum(elem => elem.TotalValues);
        }

        private void OnBudgetPlannedChanged(object sender, PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("budgetPlanned"));
        }
    }
}
