using HomeBudget.Code.Logic;
using System;
using System.ComponentModel;

namespace HomeBudget.Pages.Utils
{
    public class BudgetSummaryDataViewModel : INotifyPropertyChanged
    {
        public string CategoryName => CategoryReal.Name;
        public event PropertyChangedEventHandler PropertyChanged;

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
                //categoryReal.PropertyChanged += OnCategoryChanged;
            }
        }

        private void OnCategoryChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CategoryReal.TotalValues"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SpendPercentage"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SpendPercentageInt"));
        }

        private string icon;
        public string IconFile
        {
            get => icon;
            set => icon = value;
        }
        public double SpendPercentage =>
                //Random rand = new Random();
                CategoryPlanned.TotalValues > 0 ? Min((CategoryReal.TotalValues / CategoryPlanned.TotalValues), 1) : 0; //rand.NextDouble();

        private double Min(double v1, double v2)
        {
            return v1 > v2 ? v2 : v1;
        }

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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SpendPercentage"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SpendPercentageInt"));
        }
    }
}
