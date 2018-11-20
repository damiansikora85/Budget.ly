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
        public PlannedSubcat SubcatPlanned { get; set; }

        public BudgetRealCategory CategoryReal { get; set; }
        public bool IsIncome => Category.IsIncome;

        private int test;
        public int Test
        {
            get => test;
            set
            {
                test = value;
                RaisePropertyChanged(nameof(Test));
            }
        }

        public BudgetViewModelData()
        {
            var rand = new Random();
            test = rand.Next(100);
        }

        public BudgetViewModelData Thiz { get { return this; } }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
