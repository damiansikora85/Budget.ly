using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace HomeBudget.UWP.Utils
{
    public class NotificationManager
    {
        private ToastNotifier _notificationManager;
        private Windows.Storage.ApplicationDataContainer _localSettings;

        public NotificationManager()
        {
            _notificationManager = ToastNotificationManager.CreateToastNotifier();
            _localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        }

        public void ReScheduleNotificationsBySettings()
        {
            ClearAllNotifications();
            if (GetFromSettings("NotificationsDisabled") is bool notificationsDisabled && !notificationsDisabled)
            {
                if (GetFromSettings("NotificationsTime") is TimeSpan notificationTime)
                {
                    if (GetFromSettings("NotificationsMonday") is bool notificationsMonday && notificationsMonday)
                        ScheduleNotification(DayOfWeek.Monday, notificationTime);

                    if (GetFromSettings("NotificationsTuesday") is bool notificationsTuesday && notificationsTuesday)
                        ScheduleNotification(DayOfWeek.Tuesday, notificationTime);

                    if (GetFromSettings("NotificationsWednesday") is bool notificationsWednesday && notificationsWednesday)
                        ScheduleNotification(DayOfWeek.Wednesday, notificationTime);

                    if (GetFromSettings("NotificationsThursday") is bool notificationsThursday && notificationsThursday)
                        ScheduleNotification(DayOfWeek.Thursday, notificationTime);

                    if (GetFromSettings("NotificationsFriday") is bool notificationsFriday && notificationsFriday)
                        ScheduleNotification(DayOfWeek.Friday, notificationTime);

                    if (GetFromSettings("NotificationsSaturday") is bool notificationsSaturday && notificationsSaturday)
                        ScheduleNotification(DayOfWeek.Saturday, notificationTime);

                    if (GetFromSettings("NotificationsSunday") is bool notificationsSunday && notificationsSunday)
                        ScheduleNotification(DayOfWeek.Sunday, notificationTime);
                }
            }
        }

        private object GetFromSettings(string settingName)
        {
            if (_localSettings.Values.ContainsKey(settingName))
            {
                return _localSettings.Values[settingName];
            }
            else
                return null;
        }

        public void ClearAllNotifications()
        {
            if (_notificationManager != null)
            {
                var scheduledNotifications = _notificationManager.GetScheduledToastNotifications();
                foreach (var notification in scheduledNotifications)
                    _notificationManager.RemoveFromSchedule(notification);
            }
        }

        public void ScheduleNotification(DayOfWeek dayOfWeek, TimeSpan time)
        {
            var now = DateTime.Now;
            ScheduledToastNotification notification = null;
            var notificationDate = GetNextWeekday(now, dayOfWeek).Date + time;
            if (notificationDate > now)
            {
                notification = CreateNotification(notificationDate);
                _notificationManager.AddToSchedule(notification);
            }
            notification = CreateNotification(notificationDate.AddDays(7));
            _notificationManager.AddToSchedule(notification);

            notification = CreateNotification(notificationDate.AddDays(14));
            _notificationManager.AddToSchedule(notification);

            notification = CreateNotification(notificationDate.AddDays(21));
            _notificationManager.AddToSchedule(notification);

            notification = CreateNotification(notificationDate.AddDays(28));
            _notificationManager.AddToSchedule(notification);
            
        }

        public static DateTime GetNextWeekday(DateTime start, DayOfWeek day)
        {
            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysToAdd = ((int)day - (int)start.DayOfWeek + 7) % 7;
            return start.AddDays(daysToAdd);
        }

        private ScheduledToastNotification CreateNotification(DateTime dateTime)
        {
            var visual = new ToastVisual()
            {
                BindingGeneric = new ToastBindingGeneric()
                {
                    Children =
                    {
                        new AdaptiveText()
                        {
                            Text = "Budget.ly"
                        },

                        new AdaptiveText()
                        {
                            Text = "Notification test"
                        },
                    },
                }
            };

            // Now we can construct the final toast content
            ToastContent toastContent = new ToastContent()
            {
                Visual = visual,
            };

            // And create the toast notification
            return new ScheduledToastNotification(toastContent.GetXml(), new DateTimeOffset(dateTime));
        }
    }
}
