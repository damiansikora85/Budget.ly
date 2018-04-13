using FFImageLoading.Forms;
using FFImageLoading.Transformations;
using FFImageLoading.Work;
using HomeBudget.Code;
using HomeBudget.Code.Logic;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using Xamarin.Forms;

namespace HomeBudget.Pages.Utils
{
    public class BudgetSummaryDataViewModel : INotifyPropertyChanged
    {
        public string CategoryName => CategoryReal.Name;

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
                categoryReal.PropertyChanged += OnCategoryChanged;
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
            get => "ms-appx:///" + icon;
            set => icon = value;
        }
        public double SpendPercentage
        {
            get
            {
                //Random rand = new Random();
                return CategoryPlanned.TotalValues > 0 ? (CategoryReal.TotalValues / CategoryPlanned.TotalValues) : 0; //rand.NextDouble();
            }
        }

        public int SpendPercentageInt
        {
            get
            {
                return (int)(SpendPercentage * 100);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public BudgetSummaryDataViewModel()
        {

        }
    }

}
