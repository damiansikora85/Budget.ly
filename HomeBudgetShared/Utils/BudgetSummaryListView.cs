using HomeBudget.Code.Logic;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace HomeBudget.Pages.Utils
{
    public class BudgetSummaryDataViewModel : ObservableCollection<SummaryListSubcat>, INotifyPropertyChanged
    {
        public string CategoryName => CategoryReal != null ? CategoryReal.Name : "";
        public event PropertyChangedEventHandler PropertyChanged;
        public bool IsExpanded { get; private set; }
        public bool IsExpanding { get; private set; }
        private List<SummaryListSubcat> _sublist;

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

        public void Init()
        {
            if (categoryReal != null)
            {
                _sublist = new List<SummaryListSubcat>();
                foreach (var subcat in categoryReal.subcats)
                {
                    var subcatPlanned = CategoryPlanned.GetSubcat(subcat.Id);
                    _sublist.Add(new SummaryListSubcat
                    {
                        Name = subcat.Name,
                        SubcatReal = (RealSubcat)subcat,
                        SubcatPlan = (PlannedSubcat)subcatPlanned,
                        Id = subcat.Id,
                        Icon = IconFile
                    });
                }
            }
        }

        public async void Expand()
        {
            try
            {
                if (IsExpanding) return;
                IsExpanded = true;
                IsExpanding = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsExpanded)));

                foreach (var subcat in _sublist)
                {
                    this.Add(subcat);
                }

                foreach (var subcat in this)
                {
                    subcat.Expand();
                    await Task.Delay(100);
                }

                IsExpanding = false;
            }
            catch (Exception exc)
            {
                Crashes.TrackError(exc);
                IsExpanding = false;
                IsExpanded = false;
            }
        }

        public void Collapse()
        {
            if (IsExpanding) return;

            IsExpanded = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsExpanded)));
            foreach (var subcat in this)
                subcat.Collapse();
            this.Clear();
        }

        private void OnCategoryChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CategoryReal.TotalValues"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CategoryPlanned.TotalValues"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SpendPercentage)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SpendPercentageInt)));
        }
        public string IconFile { get; set; }
        public double SpendPercentage
        {
            get
            {
                //Random rand = new Random();
                if (categoryReal == null)
                {
                    return 0;
                }
                else
                {
                    return CategoryReal.TotalValues == 0 ? 0 :
                    CategoryPlanned.TotalValues > 0 ? Math.Min((CategoryReal.TotalValues / CategoryPlanned.TotalValues), 1) : 1;
                }
            }
        }

        public int SpendPercentageInt
        {
            get
            {
                return (int)(SpendPercentage * 100);
            }
        }

        public bool IsEmpty { get; internal set; }

        public override string ToString()
        {
            return CategoryName;
        }

        public void RaisePropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CategoryReal.TotalValues"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CategoryPlanned.TotalValues"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SpendPercentage)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SpendPercentageInt)));
        }
    }
}
