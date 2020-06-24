using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace HomeBudget.Effects
{
    public abstract class BudgetBaseEffect : RoutingEffect
    {
        public const string EffectsNamespace = "HomeBudget.Effects";

        protected BudgetBaseEffect(string effectId) : base(effectId)
        {
        }
    }
}
