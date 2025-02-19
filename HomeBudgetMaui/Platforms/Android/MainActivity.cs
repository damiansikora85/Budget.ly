using Android.App;
using Android.Content.PM;
using Android.OS;
using Firebase;

namespace HomeBudgetMaui
{
    [Activity(Theme = "@style/SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            var app = FirebaseApp.InitializeApp(this);
            base.OnCreate(savedInstanceState);
        }
    }
}
