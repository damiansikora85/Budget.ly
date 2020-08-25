
using Acr.UserDialogs;
using HomeBudgeStandard.Utils;
using HomeBudget;
using System;
using System.ComponentModel;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeBudgeStandard.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage, INotifyPropertyChanged
    {
        public SettingsPage ()
		{
            InitializeComponent ();
            BindingContext = this;
		}

        protected override async void OnAppearing()
        {
            RestoreFromSettings();
            base.OnAppearing();
        }

        private void OnSaveClicked(object sender, EventArgs e)
        {
            SaveNotificationSettings();

            NotificationManager.ReScheduleNotificationsBySettings();
            UserDialogs.Instance.Toast("Zapisano");
        }

        private void RestoreFromSettings()
        {
            var notificationsDisabled = GetFromSettings("NotificationsDisabled", false);
            if (notificationsDisabled is bool check)
                NotificationsCheckbox.IsChecked = check;

            var notificationTime = GetFromSettings("NotificationsTime", 0.0);
            if (notificationTime is double notificationTimeValue && notificationTimeValue > 0)
                NotificationTimePicker.Time = TimeSpan.FromMilliseconds(notificationTimeValue);

            var notificationMonday = GetFromSettings($"notification_{DayOfWeek.Monday}", false);
            if (notificationMonday is bool notificationMondayValue)
                NotificationMonday.IsChecked = notificationMondayValue;

            var notificationTuesday = GetFromSettings($"notification_{DayOfWeek.Tuesday}", false);
            if (notificationTuesday is bool notificationTuesdayValue)
                NotificationTuesday.IsChecked = notificationTuesdayValue;

            var notificationWednesday = GetFromSettings($"notification_{DayOfWeek.Wednesday}", false);
            if (notificationWednesday is bool notificationWednesdayValue)
                NotificationWednesday.IsChecked = notificationWednesdayValue;

            var notificationThursday = GetFromSettings($"notification_{DayOfWeek.Thursday}", false);
            if (notificationThursday is bool notificationThursdayValue)
                NotificationThursday.IsChecked = notificationThursdayValue;

            var notificationFriday = GetFromSettings($"notification_{DayOfWeek.Friday}", false);
            if (notificationFriday is bool notificationFridayValue)
                NotificationFriday.IsChecked = notificationFridayValue;

            var notificationSaturday = GetFromSettings($"notification_{DayOfWeek.Saturday}", false);
            if (notificationSaturday is bool notificationSaturdayValue)
                NotificationSaturday.IsChecked = notificationSaturdayValue;

            var notificationSunday = GetFromSettings($"notification_{DayOfWeek.Sunday}", false);
            if (notificationSunday is bool notificationSundayValue)
                NotificationSunday.IsChecked = notificationSundayValue;
        }

        private void NotificationsSetEnabled(bool enabled)
        {
            NotificationTimePicker.IsEnabled = enabled;
            NotificationMonday.IsEnabled = enabled;
            NotificationTuesday.IsEnabled = enabled;
            NotificationWednesday.IsEnabled = enabled;
            NotificationThursday.IsEnabled = enabled;
            NotificationFriday.IsEnabled = enabled;
            NotificationSaturday.IsEnabled = enabled;
            NotificationSunday.IsEnabled = enabled;
        }

        private void SaveNotificationSettings()
        {
            SaveSetting("NotificationsDisabled", NotificationsCheckbox.IsChecked);
            SaveSetting("NotificationsTime", NotificationTimePicker.Time.TotalMilliseconds);

            SaveSetting($"notification_{DayOfWeek.Monday}", NotificationMonday.IsChecked);
            SaveSetting($"notification_{DayOfWeek.Tuesday}", NotificationTuesday.IsChecked);
            SaveSetting($"notification_{DayOfWeek.Wednesday}", NotificationWednesday.IsChecked);
            SaveSetting($"notification_{DayOfWeek.Thursday}", NotificationThursday.IsChecked);
            SaveSetting($"notification_{DayOfWeek.Friday}", NotificationFriday.IsChecked);
            SaveSetting($"notification_{DayOfWeek.Saturday}", NotificationSaturday.IsChecked);
            SaveSetting($"notification_{DayOfWeek.Sunday}", NotificationSunday.IsChecked);

            //await App.Current.SavePropertiesAsync();
        }

        private void SaveSetting(string settingName, bool value)
        {
            Preferences.Set(settingName, value);
        }

        private bool GetFromSettings(string settingName, bool defaultValue)
        {
            return Preferences.Get(settingName, defaultValue);
        }

        private void SaveSetting(string settingName, double value)
        {
            Preferences.Set(settingName, value);
        }

        private double GetFromSettings(string settingName, double defaultValue)
        {
            return Preferences.Get(settingName, defaultValue);
        }

        private void NotificationsCheckbox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            NotificationsSetEnabled(!e.Value);
        }
    }
}