using Android.App;
using Android.Content;
using Android.Media;
using Android.Net;
using Android.OS;
using Android.Support.V4.App;
using HomeBudgeStandard.Interfaces;
using System;
using Xamarin.Forms;

[assembly: Dependency(typeof(HomeBudget.Droid.Native.AndroidNotificationService))]
namespace HomeBudget.Droid.Native
{
    public class AndroidNotificationService : INotificationService
    {
        private const string CHANNEL_ID = "com.darktower.homebudget.notification";
        private static int _notificationId = 1;
        public static int NotificationTag = 123;

        readonly DateTime _jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public AndroidNotificationService()
        {
            _notificationId = 0;
        }

        public DateTime GetNextNotificationDateTime()
        {
            var context = Android.App.Application.Context;
            var alarmManager = context.GetSystemService(Context.AlarmService) as AlarmManager;
            return DateTime.FromFileTimeUtc(alarmManager.NextAlarmClock != null ? alarmManager.NextAlarmClock.TriggerTime : 0);
        }

        public void ClearAllNotifications()
        {
            var context = Android.App.Application.Context;
            var alarmManager = context.GetSystemService(Context.AlarmService) as AlarmManager;
            var days = new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday };

            foreach(var day in days)
            {
                var intent = new Intent(context, typeof(NotificationBroadcastReceiver));
                intent.SetData(Android.Net.Uri.Parse($"notification: {day.ToString()}"));

                var pendingIntent = PendingIntent.GetBroadcast(context, 0, intent, PendingIntentFlags.Immutable);
                alarmManager.Cancel(pendingIntent);
            }
        }

        public void Initialize()
        {
            
        }

        public static void ShowNotification()
        {
            try
            {
                var resultIntent = new Intent(Android.App.Application.Context, typeof(MainActivity));
                resultIntent.AddFlags(ActivityFlags.NewTask);

                var resultPendingIntent = PendingIntent.GetActivity(Android.App.Application.Context,
                    0 /* Request code */, resultIntent, PendingIntentFlags.UpdateCurrent);


                var builder = new NotificationCompat.Builder(Android.App.Application.Context)
                    .SetSmallIcon(Resource.Drawable.LogoSmall)
                    .SetContentTitle("Budget.ly")
                    .SetContentText("Zapisz wydatki!")
                    .SetAutoCancel(true)
                    .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Notification))
                    .SetContentIntent(resultPendingIntent);


                var notificationManager = Android.App.Application.Context.GetSystemService(Context.NotificationService) as NotificationManager;
                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    if (notificationManager.GetNotificationChannel(CHANNEL_ID) == null)
                    {
                        CreateNotificationChannel();                  
                    }
                    builder.SetChannelId(CHANNEL_ID);
                }
                var notification = builder.Build();
                notificationManager.Notify(_notificationId++, builder.Build());
            }
            catch(Exception exc)
            {
                var msg = exc.Message;
            }
        }

        public void ScheduleNotification(string text, DayOfWeek[] days, TimeSpan time)
        {
            var context = Android.App.Application.Context;
            var alarmManager = context.GetSystemService(Context.AlarmService) as AlarmManager;

            foreach (var day in days)
            {
                var notificationDateTime = (GetNextWeekday(DateTime.Now, day).Date.ToUniversalTime() - _jan1st1970 + time);

                var intent = new Intent(context, typeof(NotificationBroadcastReceiver));
                intent.SetData(Android.Net.Uri.Parse($"notification: {day.ToString()}"));

                var pendingIntent = PendingIntent.GetBroadcast(context, 0, intent, PendingIntentFlags.Immutable);

                var totalMilliSeconds = (long)(DateTime.Now.Date.ToUniversalTime() - _jan1st1970 + time).TotalMilliseconds;

                alarmManager.SetRepeating(AlarmType.RtcWakeup, (long)notificationDateTime.TotalMilliseconds, AlarmManager.IntervalDay * 7, pendingIntent);
            }
        }

        private static void CreateNotificationChannel()
        {
            // Create the NotificationChannel, but only on API 26+ because
            // the NotificationChannel class is new and not in the support library
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                //CharSequence name = getString(R.string.channel_name);
                //String description = getString(R.string.channel_description);
                //int importance = NotificationManager.ImportanceDefault;
                var channel = new NotificationChannel(CHANNEL_ID, "Budget", NotificationImportance.High)
                {
                    Description = "Powiadomienia",
                    LockscreenVisibility = NotificationVisibility.Public,
                };
                channel.EnableVibration(true);
                channel.EnableLights(true);
                channel.SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Notification), new AudioAttributes.Builder().SetUsage(AudioUsageKind.Notification).Build());
                channel.LockscreenVisibility = NotificationVisibility.Public;

                // Register the channel with the system; you can't change the importance
                // or other notification behaviors after this
                var notificationManager = Android.App.Application.Context.GetSystemService(Context.NotificationService) as NotificationManager;
                notificationManager.CreateNotificationChannel(channel);
            }
        }

        private long NotifyTimeInMilliseconds(DateTime notifyTime)
        {
            var utcTime = TimeZoneInfo.ConvertTimeToUtc(notifyTime);
            var epochDifference = (new DateTime(1970, 1, 1) - DateTime.MinValue).TotalSeconds;

            var utcAlarmTimeInMillis = utcTime.AddSeconds(-epochDifference).Ticks / 10000;
            return utcAlarmTimeInMillis;
        }

        public static DateTime GetNextWeekday(DateTime start, DayOfWeek day)
        {
            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysToAdd = ((int)day - (int)start.DayOfWeek + 7) % 7;
            return start.AddDays(daysToAdd);
        }
    }
}