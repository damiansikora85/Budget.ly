using ProtoBuf;
using System;

namespace HomeBudget.Code.Logic
{
    [ProtoContract]
    public class BudgetTransaction
    {
        [ProtoMember(1)]
        public DateTime Date { get; set; }
        [ProtoMember(2)]
        public string Note { get; set; }
        [ProtoMember(3)]
        public int CategoryId { get; set; }
        [ProtoMember(4)]
        public int SubcatId { get; set; }
        [ProtoMember(5)]
        public double Amount { get; set; }
    }
}
