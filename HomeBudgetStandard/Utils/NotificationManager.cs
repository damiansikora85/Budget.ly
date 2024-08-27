using HomeBudgetStandard.Interfaces;

namespace HomeBudgetStandard.Utils
{
    public class NotificationManager
    {
        public static void ReScheduleNotificationsBySettings()
        {
            ClearAllNotifications();
            if (GetFromSettings("NotificationsDisabled", false) is bool notificationsDisabled && !notificationsDisabled)
            {
                if (GetFromSettings("NotificationsTime", 0.0) is double notificationTimeMilisec && notificationTimeMilisec > 0)
                {
                    var notificationDays = new List<DayOfWeek>();
                    if (GetFromSettings($"notification_{DayOfWeek.Monday}", false) is bool notificationsMonday && notificationsMonday)
                        notificationDays.Add(DayOfWeek.Monday);

                    if (GetFromSettings($"notification_{DayOfWeek.Tuesday}", false) is bool notificationsTuesday && notificationsTuesday)
                        notificationDays.Add(DayOfWeek.Tuesday);

                    if (GetFromSettings($"notification_{DayOfWeek.Wednesday}", false) is bool notificationsWednesday && notificationsWednesday)
                        notificationDays.Add(DayOfWeek.Wednesday);

                    if (GetFromSettings($"notification_{DayOfWeek.Thursday}", false) is bool notificationsThursday && notificationsThursday)
                        notificationDays.Add(DayOfWeek.Thursday);

                    if (GetFromSettings($"notification_{DayOfWeek.Friday}", false) is bool notificationsFriday && notificationsFriday)
                        notificationDays.Add(DayOfWeek.Friday);

                    if (GetFromSettings($"notification_{DayOfWeek.Saturday}", false) is bool notificationsSaturday && notificationsSaturday)
                        notificationDays.Add(DayOfWeek.Saturday);

                    if (GetFromSettings($"notification_{DayOfWeek.Sunday}", false) is bool notificationsSunday && notificationsSunday)
                        notificationDays.Add(DayOfWeek.Sunday);

                    ScheduleNotification(notificationDays.ToArray(), TimeSpan.FromMilliseconds(notificationTimeMilisec));
                }
            }
        }

        private static bool GetFromSettings(string settingName, bool defaultValue)
        {
            return Preferences.Get(settingName, defaultValue);
        }

        private static double GetFromSettings(string settingName, double defaultValue)
        {
            return Preferences.Get(settingName, defaultValue);
        }

        public static void ClearAllNotifications()
        {
            Preferences.Set("NotificationsTime", -1.0);
            foreach (var day in Enum.GetValues(typeof(DayOfWeek)))
            {
                Preferences.Set($"notification_{day}", false);
            }
            DependencyService.Get<INotificationService>().ClearAllNotifications();
        }

        public static void ScheduleNotification(DayOfWeek[] days, TimeSpan time)
        {
            if (days.Length == 0) return;

            Preferences.Set("NotificationsTime", time.TotalMilliseconds);
            var notificationService = DependencyService.Get<INotificationService>();
            notificationService.ScheduleNotifications("Pora uzupełnić budżet", days, time);
        }

        public static void ScheduleDefaultNotifications()
        {
            ClearAllNotifications();
            var notificationTime = TimeSpan.Parse("20:00");
            Preferences.Set("NotificationsTime", notificationTime.TotalMilliseconds);
            ScheduleNotification(new DayOfWeek[] {DayOfWeek.Wednesday, DayOfWeek.Saturday }, notificationTime);
        }
    }
}
