using Xamarin.Forms;

namespace HomeBudgeStandard.Effects
{
    public class UnderlineEffect : RoutingEffect
    {
        public const string EffectNamespace = "HomeBudgeStandard.Effects";

        public UnderlineEffect() : base($"{EffectNamespace}.{nameof(UnderlineEffect)}")
        {
        }
    }
}
