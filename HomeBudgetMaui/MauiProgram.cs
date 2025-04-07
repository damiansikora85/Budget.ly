using CommunityToolkit.Maui;
using HomeBudgetMaui.Platforms.Android.CustomHandlers;
using HomeBudgetStandard.Components;
using Microsoft.Extensions.Logging;
using Mopups.Hosting;
using Sharpnado.Tabs;
using SkiaSharp.Views.Maui.Controls.Hosting;
using Syncfusion.Maui.Core.Hosting;
using Acr.UserDialogs;


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
#endif

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
