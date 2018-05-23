using System;
using System.Collections.Generic;
using ProtoBuf;

namespace HomeBudget.Code.Logic
{
    [ProtoContract]
    public class PlannedSubcat : BaseBudgetSubcat
    {
        private double plannedValue;

        public PlannedSubcat() { }

        public PlannedSubcat(BaseBudgetSubcat subcat) : base(subcat)
        {
            Value = subcat.Value;
        }

        [ProtoMember(1)]
        public override double Value
        {
            get
            {
                return plannedValue;
            }
            set
            {
                plannedValue = value;
                RaiseValueChanged();
            }
        }

        public static PlannedSubcat Create(string subcatName, int id)
        {
            var subcat = new PlannedSubcat
            {
                Name = subcatName,
                Id = id
            };

            return subcat;
        }

        public override byte[] Serialize()
        {
            var bytes = new List<byte>();
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
