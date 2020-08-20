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
using HomeBudget.Droid.Native;

namespace HomeBudget.Droid.Receivers
{
    [BroadcastReceiver(Enabled = true, Exported = true)]
    [IntentFilter(new[] { Intent.ActionBootCompleted }, Categories = new[] { Intent.CategoryDefault })]
    public class BootCompleteReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            if (intent?.Action == "android.intent.action.BOOT_COMPLETED")
            {
                var notificationService = new AndroidNotificationService();
                notificationService.ReScheduleNotificationAfterRestart();
            }
        }
    }
}