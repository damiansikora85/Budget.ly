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
    [ProtoInclude(7, typeof(PlannedSubcat))] 
    [ProtoInclude(8, typeof(RealSubcat))]
    public class BaseBudgetSubcat : INotifyPropertyChanged
    {
        [ProtoMember(1)]
        public string Name { get; set; }
        [ProtoMember(2)]
        public int Id { get; set; }

        [ProtoMember(3)]
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
            var bytes = new List<byte>();
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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Value"));
        }
    }
}
