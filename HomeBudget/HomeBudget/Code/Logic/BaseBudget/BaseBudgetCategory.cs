using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Code.Logic
{
    public class BaseBudgetCategory
    {
        public string Name { get; protected set; }
        public int Id { get; protected set; }
        public bool IsIncome { get; protected set; }
        public List<BaseBudgetSubcat> subcats;
        public event Action onSubcatChanged;

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

        public virtual byte[] Serialize()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(BinaryData.GetBytes(Name));
            bytes.AddRange(BitConverter.GetBytes(Id));
            bytes.AddRange(BitConverter.GetBytes(IsIncome));
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
        }

        /*public decimal GetTotalValues()
        {
            return subcats.Sum(elem => Convert.ToDecimal(elem.Value));
        }*/

        public void OnSubcatChanged(object sender, PropertyChangedEventArgs e)
        {
            onSubcatChanged();
        }
    }
}
