using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Maui;

namespace HomeBudget.Effects
{
    public abstract class BudgetBaseEffect : RoutingEffect
    {
        public const string GroupName = "HomeBudget.Effects";

        protected BudgetBaseEffect(string effectId) : base(effectId)
        {
        }
    }
}
