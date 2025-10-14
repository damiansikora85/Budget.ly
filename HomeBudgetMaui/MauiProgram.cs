using Acr.UserDialogs;
using Android.Graphics.Drawables;
using CommunityToolkit.Maui;
using HomeBudgetMaui.Extensions;
using HomeBudgetMaui.Platforms.Android.CustomHandlers;
using HomeBudgetStandard.Components;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;
using Mopups.Hosting;
using Sharpnado.Tabs;
using SkiaSharp.Views.Maui.Controls.Hosting;
using Syncfusion.Maui.Core.Hosting;


namespace HomeBudgetMaui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
            .UseMauiApp<App>()
            //.UseDebugRainbows()
            .UseMauiCommunityToolkit()
            .ConfigureMopups()
            .UseSkiaSharp()
            //.UseOxyPlotSkia()
            .UseSharpnadoTabs(loggerEnable: false)
            .ConfigureSyncfusionCore()
            .ConfigureMauiHandlers(handlers =>
            {
                handlers.AddHandler(typeof(CustomDatePicker), typeof(CustomDatePickerHandler));
            })
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Font Awesome 5 Free-Regular-400.otf", "FontAwesomeRegular");
                fonts.AddFont("Font Awesome 5 Free-Solid-900.otf", "FontAwesomeSolid");
            });

#if ANDROID
            UserDialogs.Init(() => Platform.CurrentActivity);
            builder.Services.AddSingleton(UserDialogs.Instance);

            ProgressBarHandler.Mapper.AppendToMapping("GradientProgress", (handler, view) =>
            {
                if (view is BindableObject bindable && !ProgressBarExtensions.GetUseGradient(bindable))
                {
                    return;
                }

                var nativeBar = handler.PlatformView as Android.Widget.ProgressBar;

                var background = new GradientDrawable();
                background.SetColor(Android.Graphics.Color.ParseColor("#E0E0E0")); // gray track
                background.SetCornerRadius(20f);

                // Create a horizontal gradient
                var gradient = new GradientDrawable(
                    GradientDrawable.Orientation.LeftRight,
                    new int[] {
                        Android.Graphics.Color.ParseColor("#00A9FF"), // start color (orange)
                        Android.Graphics.Color.ParseColor("#00D7C4")  // end color (green)
                    });

                // Optional: round the corners
                //gradient.SetCornerRadius(20f);

                var progressClip = new ClipDrawable(gradient, Android.Views.GravityFlags.Left, ClipDrawableOrientation.Horizontal);

                var layerDrawable = new LayerDrawable(new Drawable[] { background, progressClip });

                // Assign Android layer IDs correctly
                layerDrawable.SetId(0, Android.Resource.Id.Background);
                layerDrawable.SetId(1, Android.Resource.Id.Progress);

                // Apply the composed drawable
                nativeBar.SetProgressDrawableTiled(layerDrawable);

                // Set the background (track color)
                //nativeBar.SetBackgroundColor(Android.Graphics.Color.ParseColor("#E0E0E0"));
            });
#endif

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
