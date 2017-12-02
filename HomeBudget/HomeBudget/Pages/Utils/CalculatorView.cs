using HomeBudget.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Pages.Utils
{
    public class CalculatorViewModel : INotifyPropertyChanged
    {
        private BudgetViewModelData budgetData;

        public event PropertyChangedEventHandler PropertyChanged;

        public void Init(BudgetViewModelData data, DateTime date)
        {

        }
    }
}
