using HomeBudgeStandard.Interfaces;
using HomeBudget;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HomeBudgeStandard.Utils
{
    public class NotificationManager
    {
        public static void ReScheduleNotificationsBySettings()
        {
            ClearAllNotifications();
            if (GetFromSettings("NotificationsDisabled") is bool notificationsDisabled && !notificationsDisabled)
            {
                if (GetFromSettings("NotificationsTime") is TimeSpan notificationTime)
                {
                    var notificationDays = new List<DayOfWeek>();
                    if (GetFromSettings("NotificationsMonday") is bool notificationsMonday && notificationsMonday)
                        notificationDays.Add(DayOfWeek.Monday);

                    if (GetFromSettings("NotificationsTuesday") is bool notificationsTuesday && notificationsTuesday)
                        notificationDays.Add(DayOfWeek.Tuesday);

                    if (GetFromSettings("NotificationsWednesday") is bool notificationsWednesday && notificationsWednesday)
                        notificationDays.Add(DayOfWeek.Wednesday);

                    if (GetFromSettings("NotificationsThursday") is bool notificationsThursday && notificationsThursday)
                        notificationDays.Add(DayOfWeek.Thursday);

                    if (GetFromSettings("NotificationsFriday") is bool notificationsFriday && notificationsFriday)
                        notificationDays.Add(DayOfWeek.Friday);

                    if (GetFromSettings("NotificationsSaturday") is bool notificationsSaturday && notificationsSaturday)
                        notificationDays.Add(DayOfWeek.Saturday);

                    if (GetFromSettings("NotificationsSunday") is bool notificationsSunday && notificationsSunday)
                        notificationDays.Add(DayOfWeek.Sunday);

                    ScheduleNotification(notificationDays.ToArray(), notificationTime);
                }
            }
        }

        private static object GetFromSettings(string settingName)
        {
            return App.Current.Properties.ContainsKey(settingName) ? App.Current.Properties[settingName] : null;
        }

        public static void ClearAllNotifications()
        {
            DependencyService.Get<INotificationService>().ClearAllNotifications();
        }

        public static void ScheduleNotification(DayOfWeek[] days, TimeSpan time)
        {
            if (days.Length == 0) return;

            var notificationService = DependencyService.Get<INotificationService>();
            var now = DateTime.Now;

            notificationService.ScheduleNotification("Zapisz wydatki!", days, time);
        }

        public static void ScheduleDefaultNotifications()
        {
            var notificationTime = TimeSpan.Parse("20:00");
            App.Current.Properties.Add("NotificationsWednesday", true);
            App.Current.Properties.Add("NotificationsSaturday", true);
            App.Current.Properties.Add("NotificationsTime", notificationTime);

            ScheduleNotification(new DayOfWeek[] {DayOfWeek.Wednesday, DayOfWeek.Saturday }, notificationTime);
        }
    }
}
