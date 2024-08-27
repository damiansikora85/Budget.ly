using Android.Content;
using AndroidX.Core.Content.Resources;
using HomeBudgetStandard.Components;

//[assembly: ExportRenderer(typeof(GradientProgressBar), typeof(HomeBudget.Droid.Renderers.GradientProgressBarRenderer))]
//namespace HomeBudget.Droid.Renderers
//{
//    public class GradientProgressBarRenderer : ProgressBarRenderer
//    {
//        public GradientProgressBarRenderer(Context context) : base(context)
//        {
//        }

//        protected override Android.Widget.ProgressBar CreateNativeControl()
//        {
//            var progressbar = new Android.Widget.ProgressBar(Context, null, global::Android.Resource.Attribute.ProgressBarStyleHorizontal)
//            {
//                Indeterminate = false,
//                Max = 10000,
//                //Background = ResourcesCompat.GetDrawable(Context.Resources, Resource.Drawable.progressGradient, null)
//                ProgressDrawable = ResourcesCompat.GetDrawable(Context.Resources, Resource.Drawable.progressGradient, null)
//            };

//            return progressbar;
//        }
//    }
//}