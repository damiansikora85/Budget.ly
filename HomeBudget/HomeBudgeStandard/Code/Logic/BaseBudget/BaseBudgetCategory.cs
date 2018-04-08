using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Code.Logic
{
    public class BaseBudgetCategory : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public int Id { get; protected set; }
        public bool IsIncome { get; protected set; }
        public List<BaseBudgetSubcat> subcats;
        public string IconName { get; protected set; }
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

        public virtual byte[] Serialize()
        {
            var bytes = new List<byte>();
            bytes.AddRange(BinaryData.GetBytes(Name));
            bytes.AddRange(BitConverter.GetBytes(Id));
            bytes.AddRange(BitConverter.GetBytes(IsIncome));
            bytes.AddRange(BinaryData.GetBytes(IconName));
            bytes.AddRange(BitConverter.GetBytes(subcats.Count));
            foreach (BaseBudgetSubcat subcat in subcats)
                bytes.AddRange(subcat.Serialize());

            return bytes.ToArray();
        }

        public virtual void Deserialize(BinaryData binaryData)
        {
            Name = binaryData.GetString();
            Id = binaryData.GetInt();
            IsIncome = binaryData.GetBool();
            IconName = binaryData.GetString();
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
