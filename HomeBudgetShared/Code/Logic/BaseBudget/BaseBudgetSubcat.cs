using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Code.Logic
{
    public class BaseBudgetSubcat : INotifyPropertyChanged
    {
        public string Name { get; protected set; }
        public int Id { get; protected set; }

        public virtual double Value { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public BaseBudgetSubcat() { }

        public BaseBudgetSubcat(BaseBudgetSubcat subcat)
        {
            Name = subcat.Name;
            Id = subcat.Id;
        }

        public virtual byte[] Serialize()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(BinaryData.GetBytes(Name));
            bytes.AddRange(BitConverter.GetBytes(Id));

            return bytes.ToArray();
        }

        /*public BaseBudgetSubcat()
        {
            Value = 0;
        }*/

        public virtual void Deserialize(BinaryData binaryData)
        {
            Name = binaryData.GetString();
            Id = binaryData.GetInt();
        }

        protected void RaiseValueChanged()
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("Value"));
        }
    }
}
