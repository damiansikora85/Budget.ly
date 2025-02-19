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
    }
}
