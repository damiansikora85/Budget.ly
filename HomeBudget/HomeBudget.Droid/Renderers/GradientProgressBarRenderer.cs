using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content.Res;
using Android.Views;
using Android.Widget;
using HomeBudgeStandard.Components;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(GradientProgressBar), typeof(HomeBudget.Droid.Renderers.GradientProgressBarRenderer))]
namespace HomeBudget.Droid.Renderers
{
    public class GradientProgressBarRenderer : ProgressBarRenderer
    {
        public GradientProgressBarRenderer(Context context) : base(context)
        {
        }

        protected override Android.Widget.ProgressBar CreateNativeControl()
        {
            var progressbar = new Android.Widget.ProgressBar(Context, null, global::Android.Resource.Attribute.ProgressBarStyleHorizontal)
            {
                Indeterminate = false,
                Max = 10000,
                //Background = ResourcesCompat.GetDrawable(Context.Resources, Resource.Drawable.progressGradient, null)
                ProgressDrawable = ResourcesCompat.GetDrawable(Context.Resources, Resource.Drawable.progressGradient, null)
            };

            return progressbar;
        }
    }
}