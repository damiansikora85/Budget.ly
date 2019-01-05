using HomeBudget.Code.Logic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace HomeBudget.Pages.Utils
{
    public class BudgetSummaryDataViewModel : ObservableCollection<SummaryListSubcat>, INotifyPropertyChanged
    {
        public string CategoryName => CategoryReal.Name;
        public event PropertyChangedEventHandler PropertyChanged;
        public bool IsExpanded { get; private set; }

        public BaseBudgetCategory CategoryPlanned
        {
            get;
            set;
        }

        private BaseBudgetCategory categoryReal;
        public BaseBudgetCategory CategoryReal
        {
            get { return categoryReal; }
            set
            {
                categoryReal = value;
                this.Clear();
            }
        }

        public async void Expand()
        {
            IsExpanded = true;
            this.Clear();
            int delay = 50;

            foreach (var subcat in categoryReal.subcats)
            {
                var subcatPlanned = CategoryPlanned.GetSubcat(subcat.Id);
                this.Add(new SummaryListSubcat
                {
                    Name = subcat.Name,
                    Amount = subcat.Value,
                    SpendPercentage = subcatPlanned.Value > 0 ? Math.Min((subcat.Value / subcatPlanned.Value), 1) : 0,
                    Id = subcat.Id
                });
                await Task.Delay(10);
                delay = Math.Max(10, delay - 7);
            }
        }

        public void Collapse()
        {
            IsExpanded = false;
            this.Clear();
        }

        private void OnCategoryChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CategoryReal.TotalValues"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SpendPercentage"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SpendPercentageInt"));
        }
        public string IconFile { get; set; }
        public double SpendPercentage =>
                //Random rand = new Random();
                CategoryPlanned.TotalValues > 0 ? Math.Min((CategoryReal.TotalValues / CategoryPlanned.TotalValues), 1) : 0; 

        public int SpendPercentageInt
        {
            get
            {
                return (int)(SpendPercentage * 100);
            }
        }

        public override string ToString()
        {
            return CategoryName;
        }

        public void RaisePropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CategoryReal.TotalValues"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SpendPercentage"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SpendPercentageInt"));
        }
    }
}
