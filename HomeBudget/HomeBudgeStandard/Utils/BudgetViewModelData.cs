using HomeBudget.Code.Logic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Utils
{
    public class BudgetViewModelData : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public BaseBudgetCategory Category { get; set; }
        public BaseBudgetSubcat Subcat { get; set; }
        public RealSubcat SubcatReal { get; set; }

        public BudgetViewModelData Thiz { get { return this; } }

        public event PropertyChangedEventHandler PropertyChanged;
        void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
