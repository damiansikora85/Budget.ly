using HomeBudget.Code.Logic;
using ProtoBuf;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace HomeBudget.Utils
{
    public class BudgetViewModelData : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public BaseBudgetCategory Category { get; set; }
        public BaseBudgetSubcat Subcat { get; set; }
        public RealSubcat SubcatReal { get; set; }
        public BudgetRealCategory CategoryReal { get; set; }

        public BudgetViewModelData Thiz { get { return this; } }

        public event PropertyChangedEventHandler PropertyChanged;
        void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
