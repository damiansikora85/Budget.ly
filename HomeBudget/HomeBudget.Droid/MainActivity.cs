
using Acr.UserDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using HomeBudget.Droid.Native;
using Plugin.InAppBilling;

namespace HomeBudget.Droid
{
    [Activity(Label = "Budget.ly", Icon = "@drawable/icon", Theme = "@style/SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
        private App _theApp;
		protected override void OnCreate(Bundle bundle)
		{
            base.Window.RequestFeature(WindowFeatures.ActionBar);

            base.SetTheme(Resource.Style.AppTheme_Main);
            TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            Xamarin.Essentials.Platform.Init(this, bundle);
            Rg.Plugins.Popup.Popup.Init(this, bundle);
            UserDialogs.Init(this);
            HUDTutorial.MainActivity = this;
            global::Xamarin.Forms.Forms.Init(this, bundle);

            OxyPlot.Xamarin.Forms.Platform.Android.PlotViewRenderer.Init();
            Xamarin.Forms.DependencyService.Register<AndroidNotificationService>();
            Xamarin.Forms.DependencyService.Register<HUDTutorial>();

            _theApp = new App();
            LoadApplication(_theApp);

            Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity = this;
        }

        public override void OnBackPressed()
        {
            if (Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed))
            {
                // Do something if there are some pages in the `PopupStack`
            }
            else if (!_theApp.OnBackPressed())
                base.OnBackPressed();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            InAppBillingImplementation.HandleActivityResult(requestCode, resultCode, data);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

