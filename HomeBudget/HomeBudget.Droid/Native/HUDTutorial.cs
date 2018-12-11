using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using HomeBudgeStandard.Interfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(HomeBudget.Droid.Native.HUDTutorial))]
namespace HomeBudget.Droid.Native
{
    public class HUDTutorial : IHUD
    {
        public static Activity MainActivity;

        private Dialog currentDialog;

        public void ShowTestDialog()
        {
            //var context = Android.App.Application.;
            currentDialog = new Dialog(MainActivity);
            currentDialog.RequestWindowFeature((int)WindowFeatures.NoTitle);
            currentDialog.Window.ClearFlags(WindowManagerFlags.DimBehind);

            var inflater = LayoutInflater.FromContext(MainActivity);
            var view = inflater.Inflate(Resource.Layout.test, null);

            currentDialog.SetContentView(view);

            currentDialog.Show();
        }
    }
}