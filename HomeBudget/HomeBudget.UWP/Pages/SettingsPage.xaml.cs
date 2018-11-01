using HomeBudget.UWP.Utils;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace HomeBudget.UWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
        }

        private void NotificationCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            NotificationsSetEnabled(false);
        }

        private void notificationCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            NotificationsSetEnabled(true);
        }

        private void NotificationsSetEnabled(bool enabled)
        {
            checkBoxMon.IsEnabled = enabled;
            checkBoxTue.IsEnabled = enabled;
            checkBoxWed.IsEnabled = enabled;
            checkBoxThu.IsEnabled = enabled;
            checkBoxFri.IsEnabled = enabled;
            checkBoxSat.IsEnabled = enabled;
            checkBoxSun.IsEnabled = enabled;
            timePicker.IsEnabled = enabled;
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            SaveNotificationSettings();

            var notificationManager = new NotificationManager();
            notificationManager.ReScheduleNotificationsBySettings();
        }

        private void SaveNotificationSettings()
        {
            SaveSetting("NotificationsDisabled", notificationCheckBox.IsChecked.HasValue ? notificationCheckBox.IsChecked.Value : false);
            SaveSetting("NotificationsTime", timePicker.Time);

            SaveSetting("NotificationsMonday", checkBoxMon.IsChecked.HasValue ? checkBoxMon.IsChecked.Value : false);
            SaveSetting("NotificationsTuesday", checkBoxTue.IsChecked.HasValue ? checkBoxTue.IsChecked.Value : false);
            SaveSetting("NotificationsWednesday", checkBoxWed.IsChecked.HasValue ? checkBoxWed.IsChecked.Value : false);
            SaveSetting("NotificationsThursday", checkBoxThu.IsChecked.HasValue ? checkBoxThu.IsChecked.Value : false);
            SaveSetting("NotificationsFriday", checkBoxFri.IsChecked.HasValue ? checkBoxFri.IsChecked.Value : false);
            SaveSetting("NotificationsSaturday", checkBoxSat.IsChecked.HasValue ? checkBoxSat.IsChecked.Value : false);
            SaveSetting("NotificationsSunday", checkBoxSun.IsChecked.HasValue ? checkBoxSun.IsChecked.Value : false);
        }

        private void RestoreFromSettings()
        {
            var notificationsDisabled = GetFromSettings("NotificationsDisabled");
            if (notificationsDisabled is bool check)
                notificationCheckBox.IsChecked = check;

            var notificationTime = GetFromSettings("NotificationsTime");
            if (notificationTime is TimeSpan notificationTimeValue)
                timePicker.Time = notificationTimeValue;

            var notificationMonday = GetFromSettings("NotificationsMonday");
            if (notificationMonday is bool notificationMondayValue)
                checkBoxMon.IsChecked = notificationMondayValue;

            var notificationTuesday = GetFromSettings("NotificationsTuesday");
            if (notificationTuesday is bool notificationTuesdayValue)
                checkBoxTue.IsChecked = notificationTuesdayValue;

            var notificationWednesday = GetFromSettings("NotificationsWednesday");
            if (notificationWednesday is bool notificationWednesdayValue)
                checkBoxWed.IsChecked = notificationWednesdayValue;

            var notificationThursday = GetFromSettings("NotificationsThursday");
            if (notificationThursday is bool notificationThursdayValue)
                checkBoxThu.IsChecked = notificationThursdayValue;

            var notificationFriday = GetFromSettings("NotificationsFriday");
            if (notificationFriday is bool notificationFridayValue)
                checkBoxFri.IsChecked = notificationFridayValue;

            var notificationSaturday = GetFromSettings("NotificationsSaturday");
            if (notificationSaturday is bool notificationSaturdayValue)
                checkBoxSat.IsChecked = notificationSaturdayValue;

            var notificationSunday = GetFromSettings("NotificationsSunday");
            if (notificationSunday is bool notificationSundayValue)
                checkBoxSun.IsChecked = notificationSundayValue;
        }

        private void SaveSetting(string settingName, object value)
        {
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            if (localSettings.Values.ContainsKey(settingName))
                localSettings.Values[settingName] = value;
            else
                localSettings.Values.Add(settingName, value);
        }

        private object GetFromSettings(string settingName)
        {
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings.Values.ContainsKey(settingName))
            {
                return localSettings.Values[settingName];
            }
            else
                return null;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            RestoreFromSettings();
            base.OnNavigatedTo(e);
        }
    }
}
