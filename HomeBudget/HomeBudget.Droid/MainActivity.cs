using Android.App;
using Android.Content.PM;
using Android.OS;

namespace HomeBudget.Droid
{
    [Activity(Label = "Budget.ly", Icon = "@drawable/icon", Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : MauiAppCompatActivity
    {
        private App _theApp;
		protected override void OnCreate(Bundle bundle)
		{
   //         base.Window.RequestFeature(WindowFeatures.ActionBar);

   //         base.SetTheme(Resource.Style.AppTheme_Main);
   //         TabLayoutResource = Resource.Layout.Tabbar;
			//ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

#if (!CUSTOM)
            //FirebaseApp.InitializeApp(this);
#endif
            //Xamarin.Essentials.Platform.Init(this, bundle);
            //Rg.Plugins.Popup.Popup.Init(this, bundle);
            //UserDialogs.Init(this);
            //global::Xamarin.Forms.Forms.Init(this, bundle);

            //OxyPlot.Xamarin.Forms.Platform.Android.PlotViewRenderer.Init();
            //Xamarin.Forms.DependencyService.Register<AndroidNotificationService>();
            //DependencyService.Register<CustomFirebaseRemoteConfig>();

            //_theApp = new App();
            //LoadApplication(_theApp);

            //MessagingCenter.Subscribe<BudgetDataGridPage>(this, "Landscape", Page =>
            //{
            //    RequestedOrientation = ScreenOrientation.Landscape;
            //});
            //MessagingCenter.Subscribe<BudgetDataGridPage>(this, "Portrait", Page =>
            //{
            //    RequestedOrientation = ScreenOrientation.Portrait;
            //});
        }

        public override void OnBackPressed()
        {
            //if (Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed))
            //{
            //    // Do something if there are some pages in the `PopupStack`
            //}
            //else if (!_theApp.OnBackPressed())
            //    base.OnBackPressed();
        }

        //protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        //{
        //    base.OnActivityResult(requestCode, resultCode, data);
        //}

        //public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        //{
        //    Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        //    base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        //}
    }
}

