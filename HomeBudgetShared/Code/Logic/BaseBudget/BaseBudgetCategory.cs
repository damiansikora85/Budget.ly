using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace HomeBudget.Code.Logic
{
    [ProtoContract]
    [ProtoInclude(7, typeof(BudgetPlannedCategory))]
    [ProtoInclude(8, typeof(BudgetRealCategory))]
    public class BaseBudgetCategory : INotifyPropertyChanged
    {
        [ProtoMember(1)]
        public string Name { get; set; }
        [ProtoMember(2)]
        public int Id { get; set; }
        [ProtoMember(3)]
        public bool IsIncome { get; set; }
        [ProtoMember(4)]
        public List<BaseBudgetSubcat> subcats;
        [ProtoMember(5)]
        public string IconName { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public Action onCategoryValueChanged;

        public double TotalValues
        {
            get
            {
                double sum = 0;
                foreach (BaseBudgetSubcat subcat in subcats)
                    sum += subcat.Value;

                return sum;
            }
        }

        public BaseBudgetCategory()
        {
            subcats = new List<BaseBudgetSubcat>();
        }

        public BaseBudgetCategory(BaseBudgetCategory category)
        {
            subcats = new List<BaseBudgetSubcat>();
            Name = category.Name;
            Id = category.Id;
            IsIncome = category.IsIncome;
            IconName = category.IconName;
        }

        public virtual void Prepare()
        {
            foreach (var subcat in subcats)
            {
                subcat.Prepare();
                subcat.PropertyChanged += OnSubcatChanged;
            }
        }

        protected void OnSubcatChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged("TotalValues");
        }

        protected void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
