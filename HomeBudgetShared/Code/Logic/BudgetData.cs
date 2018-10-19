using System;
using System.Collections.Generic;
using ProtoBuf;

namespace HomeBudget.Code.Logic
{
    [ProtoContract]
    public class BudgetData
    {
        [ProtoMember(1)]
        public int Version { get; set; }

        [ProtoMember(2)]
        public DateTime TimeStamp { get; set; }

        [ProtoMember(3)]
        public BudgetPlanned  BudgetPlanned { get; set; }

        [ProtoMember(4)]
        public List<BudgetMonth> Months { get; set; }

        [ProtoMember(5)]
        public bool IsSynchronized { get; set; }
    }
}
