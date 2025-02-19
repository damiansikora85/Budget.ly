using CommunityToolkit.Maui;
using Mopups.Hosting;
using Plugin.Maui.DebugRainbows;

namespace HomeBudget;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            //.UseDebugRainbows()
            .UseMauiCommunityToolkit()
            .ConfigureMopups();
            
        return builder.Build();
    }
}