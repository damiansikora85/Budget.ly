using Syncfusion.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace HomeBudget.Code.Logic
{
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

        public double Value
        {
            get { return subcatValue; }
            set
            {
                subcatValue = value;
                if(PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Values["+index+"].Value"));
            }
        }
    };

    public class RealSubcat : BaseBudgetSubcat
    {
        private ObservableCollection<SubcatValue> values;// = new double[31];

        public static RealSubcat Create(string subcatName, int id)
        {
            RealSubcat subcat = new RealSubcat()
            {
                Name = subcatName,
                Id = id,
                
            };

            return subcat;
        }

        public RealSubcat()
        {
            values = new ObservableCollection<SubcatValue>();

            for (int i =0; i<31;i++)
            {
                SubcatValue subcatVal = new SubcatValue(i)
                {
                    Value = 0
                };
                values.Add(subcatVal);
            }
        }

        public ObservableCollection<SubcatValue> Values
        {
            get
            {
                return values;
            }
            set
            {
                values = value;
            }
        }

        public override double Value
        {
            get
            {
                return values.Sum(subcatValue => subcatValue.Value);
            }
            set
            {
                values[0].Value = value;
            }
        }

        private int test;
        public int Test
        {
            get
            {
                return test;
            }
            set
            {
                test = value;
            }
        }

        public override byte[] Serialize()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(base.Serialize());
            var byteArray = new List<byte>();
            for (int i = 0; i < 31; i++)
                byteArray.AddRange(BitConverter.GetBytes(values[i].Value));
            //Buffer.BlockCopy(values, 0, byteArray, 0, byteArray.Length);
            bytes.AddRange(byteArray);

            return bytes.ToArray();
        }

        public override void Deserialize(BinaryData binaryData)
        {
            base.Deserialize(binaryData);
            //values.Clear();
            for (int i = 0; i < 31; i++)
            {
                values[i].Value = binaryData.GetDouble();
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
            values[date.Day-1].Value += value;
            RaiseValueChanged();
        }
    }
}
