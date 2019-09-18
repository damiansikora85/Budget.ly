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
                Id = id
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
            values = new ObservableCollection<SubcatValue>();
        }

        public override void Prepare()
        {
            foreach (var val in Values)
                val.PropertyChanged += OnValueChanged;
        }

        private void OnValueChanged(object sender, PropertyChangedEventArgs e)
        {
            RaiseValueChanged();
        }

        private ObservableCollection<SubcatValue> values;

        [ProtoMember(1)]
        public ObservableCollection<SubcatValue> Values
        {
            get => values;
            set
            {
                values = value;
            }
        }

        public override double Value
        {
            get => Values.Sum(subcatValue => subcatValue.Value);
        }

        public void AddValue(double value, DateTime date)
        {
            if (Values.Count < 31)
            {
                for (int i = Values.Count; i < 31; i++)
                {
                    var subcatVal = new SubcatValue(i)
                    {
                        Value = 0
                    };
                    subcatVal.PropertyChanged += OnValueChanged;
                    Values.Add(subcatVal);
                }
            }
            Values[date.Day-1].Value += value;
            RaiseValueChanged();
        }

        public override void CheckIfValid()
        {
            if(Values.Count < 31)
            {
                for (int i = Values.Count; i < 31; i++)
                {
                    var subcatVal = new SubcatValue(i)
                    {
                        Value = 0
                    };
                    subcatVal.PropertyChanged += OnValueChanged;
                    Values.Add(subcatVal);
                }
            }
        }
    }
}
