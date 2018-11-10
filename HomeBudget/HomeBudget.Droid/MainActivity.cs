using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using System.Threading.Tasks;
using HomeBudget.Code;
using HomeBudgeStandard.Utils;
using Acr.UserDialogs;
using Android.Content;
using HomeBudget.Droid.Native;
using Plugin.LocalNotifications;

namespace HomeBudget.Droid
{
	[Activity(Label = "HomeBudget", Icon = "@drawable/icon", Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
        private App _theApp;
		protected override void OnCreate(Bundle bundle)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate(bundle);

            var notificationManager = Application.Context.GetSystemService(Context.NotificationService) as Android.App.NotificationManager;
            notificationManager.CancelAll();

            UserDialogs.Init(this);
            global::Xamarin.Forms.Forms.Init(this, bundle);

            Xamarin.Forms.DependencyService.Register<AndroidNotificationService>();

            _theApp = new App();
            LoadApplication(_theApp);
		}

        public override void OnBackPressed()
        {
            if(!_theApp.OnBackPressed())
                base.OnBackPressed();
        }
    }
}

