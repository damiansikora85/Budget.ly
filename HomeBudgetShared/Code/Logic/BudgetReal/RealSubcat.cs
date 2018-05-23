using Syncfusion.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using ProtoBuf;

namespace HomeBudget.Code.Logic
{
    [ProtoContract]
    public class SubcatValue : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private double subcatValue;
        private int index;

        private SubcatValue() { }
        public SubcatValue(int _index)
        {
            index = _index;
        }

        [ProtoMember(1)]
        public double Value
        {
            get { return subcatValue; }
            set
            {
                subcatValue = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Values["+index+"].Value"));
            }
        }
    };

    [ProtoContract]
    public class RealSubcat : BaseBudgetSubcat
    {
        public static RealSubcat Create(string subcatName, int id)
        {
            var subcat = new RealSubcat
            {
                Name = subcatName,
                Id = id,
                Values = new ObservableCollection<SubcatValue>()
            };

            for (int i = 0; i < 31; i++)
            {
                var subcatVal = new SubcatValue(i)
                {
                    Value = 0
                };
                subcatVal.PropertyChanged += subcat.OnValueChanged;
                subcat.Values.Add(subcatVal);
            }

            return subcat;
        }

        public RealSubcat()
        {
            
        }

        private void OnValueChanged(object sender, PropertyChangedEventArgs e)
        {
            RaiseValueChanged();
        }

        [ProtoMember(1)]
        public ObservableCollection<SubcatValue> Values { get; set; }

        public override double Value
        {
            get
            {
                return Values.Sum(subcatValue => subcatValue.Value);
            }
            set
            {
                //Values[0].Value = value;
            }
        }

        public override byte[] Serialize()
        {
            var bytes = new List<byte>();
            bytes.AddRange(base.Serialize());
            var byteArray = new List<byte>();
            for (int i = 0; i < 31; i++)
                byteArray.AddRange(BitConverter.GetBytes(Values[i].Value));

            bytes.AddRange(byteArray);

            return bytes.ToArray();
        }

        public override void Deserialize(BinaryData binaryData)
        {
            base.Deserialize(binaryData);
            //values.Clear();
            for (int i = 0; i < 31; i++)
            {
                Values[i].Value = binaryData.GetDouble();
                /*
                SubcatValue subcatVal = new SubcatValue(i)
                {
                    Value = binaryData.GetDouble()
                };
                //values[i].Value = 
                values.Add(subcatVal);*/
            }
        }

        public void AddValue(double value, DateTime date)
        {
            Values[date.Day-1].Value += value;
            RaiseValueChanged();
        }
    }
}
