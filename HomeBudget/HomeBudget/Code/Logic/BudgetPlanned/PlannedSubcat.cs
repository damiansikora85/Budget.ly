using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Code.Logic
{
    public class PlannedSubcat : BaseBudgetSubcat
    {
        private double plannedValue;

        public override double Value
        {
            get
            {
                return plannedValue;
            }
            set
            {
                plannedValue = value;
                RaisePropertyChanged("Value");
            }
        }

        public static PlannedSubcat Create(string subcatName, int id)
        {
            PlannedSubcat subcat = new PlannedSubcat()
            {
                Name = subcatName,
                Id = id
            };

            return subcat;
        }

        public override byte[] Serialize()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(base.Serialize());
            bytes.AddRange(BitConverter.GetBytes(plannedValue));

            return bytes.ToArray();
        }

        public override void Deserialize(BinaryData binaryData)
        {
            base.Deserialize(binaryData);
            plannedValue = binaryData.GetDouble();
        }
    }
}
