using Acr.UserDialogs;
<<<<<<< HEAD
using Dropbox.Api;
using Dropbox.Api.Files;
using HomeBudgeStandard.Interfaces.Impl;
using HomeBudget.Code;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
=======
using HomeBudgeStandard.Utils;
using HomeBudget;
using System;
>>>>>>> master

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeBudgeStandard.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage, INotifyPropertyChanged
    {
<<<<<<< HEAD
        private const string _redirectUri = "https://localhost/authorize";
        private string _appKey = "p6cayskxetnkx1a";
        private string _oauth2State;
        private bool _checkDropboxFileExist;

        public bool HasAccessToken
        {
            get => !string.IsNullOrEmpty(HomeBudget.Helpers.Settings.DropboxAccessToken);
        }

        public string BuyTitle { get; set; }

        public bool DropboxSynchroBought { get; set; }
        public bool IsNotBoughtYet => !DropboxSynchroBought;

=======
>>>>>>> master
        public SettingsPage ()
		{
			InitializeComponent ();
            BindingContext = this;
		}

<<<<<<< HEAD
        protected override async void OnAppearing()
        {
            var purchaseService = new PurchaseService();
            var info = await purchaseService.GetProductInfo("com.darktower.homebudget.dropbox");
            if (info != null)
            {
                BuyTitle = $"{info.Name} {info.LocalizedPrice}";
                OnPropertyChanged(nameof(BuyTitle));
            }

            DropboxSynchroBought = await purchaseService.IsProductAlreadyBought("com.darktower.homebudget.dropbox");
            OnPropertyChanged(nameof(IsNotBoughtYet));
        }

        private async void OnLoginDropbox(object sender, EventArgs e)
        {
            var purchaseService = new PurchaseService();
            await purchaseService.MakePurchase("com.darktower.homebudget.dropbox");
            //_checkDropboxFileExist = false;
            //await LoginToDropbox();
=======
        protected override void OnAppearing()
        {
            RestoreFromSettings();
            base.OnAppearing();
>>>>>>> master
        }

        private void OnSaveClicked(object sender, EventArgs e)
        {
            SaveNotificationSettings();

            NotificationManager.ReScheduleNotificationsBySettings();
            UserDialogs.Instance.Toast("Zapisano");
        }

        private void RestoreFromSettings()
        {
            var notificationsDisabled = GetFromSettings("NotificationsDisabled");
            if (notificationsDisabled is bool check)
                NotificationsCheckbox.IsChecked = check;

            var notificationTime = GetFromSettings("NotificationsTime");
            if (notificationTime is TimeSpan notificationTimeValue)
                NotificationTimePicker.Time = notificationTimeValue;

            var notificationMonday = GetFromSettings("NotificationsMonday");
            if (notificationMonday is bool notificationMondayValue)
                NotificationMonday.IsChecked = notificationMondayValue;

            var notificationTuesday = GetFromSettings("NotificationsTuesday");
            if (notificationTuesday is bool notificationTuesdayValue)
                NotificationTuesday.IsChecked = notificationTuesdayValue;

            var notificationWednesday = GetFromSettings("NotificationsWednesday");
            if (notificationWednesday is bool notificationWednesdayValue)
                NotificationWednesday.IsChecked = notificationWednesdayValue;

            var notificationThursday = GetFromSettings("NotificationsThursday");
            if (notificationThursday is bool notificationThursdayValue)
                NotificationThursday.IsChecked = notificationThursdayValue;

            var notificationFriday = GetFromSettings("NotificationsFriday");
            if (notificationFriday is bool notificationFridayValue)
                NotificationFriday.IsChecked = notificationFridayValue;

            var notificationSaturday = GetFromSettings("NotificationsSaturday");
            if (notificationSaturday is bool notificationSaturdayValue)
                NotificationSaturday.IsChecked = notificationSaturdayValue;

            var notificationSunday = GetFromSettings("NotificationsSunday");
            if (notificationSunday is bool notificationSundayValue)
                NotificationSunday.IsChecked = notificationSundayValue;
        }

        private void DisableNotificationChanged(object sender, Syncfusion.XForms.Buttons.StateChangedEventArgs e)
        {
            NotificationsSetEnabled(e.IsChecked.HasValue ? !e.IsChecked.Value : true);
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

        private async void SaveNotificationSettings()
        {
            SaveSetting("NotificationsDisabled", NotificationsCheckbox.IsChecked.HasValue ? NotificationsCheckbox.IsChecked.Value : false);
            SaveSetting("NotificationsTime", NotificationTimePicker.Time);

            SaveSetting("NotificationsMonday", NotificationMonday.IsChecked.HasValue ? NotificationMonday.IsChecked.Value : false);
            SaveSetting("NotificationsTuesday", NotificationTuesday.IsChecked.HasValue ? NotificationTuesday.IsChecked.Value : false);
            SaveSetting("NotificationsWednesday", NotificationWednesday.IsChecked.HasValue ? NotificationWednesday.IsChecked.Value : false);
            SaveSetting("NotificationsThursday", NotificationThursday.IsChecked.HasValue ? NotificationThursday.IsChecked.Value : false);
            SaveSetting("NotificationsFriday", NotificationFriday.IsChecked.HasValue ? NotificationFriday.IsChecked.Value : false);
            SaveSetting("NotificationsSaturday", NotificationSaturday.IsChecked.HasValue ? NotificationSaturday.IsChecked.Value : false);
            SaveSetting("NotificationsSunday", NotificationSunday.IsChecked.HasValue ? NotificationSunday.IsChecked.Value : false);

            await App.Current.SavePropertiesAsync();
        }

        private void SaveSetting(string settingName, object value)
        {
            if (App.Current.Properties.ContainsKey(settingName))
                App.Current.Properties[settingName] = value;
            else
                App.Current.Properties.Add(settingName, value);
        }

        private object GetFromSettings(string settingName)
        {
            if (App.Current.Properties.ContainsKey(settingName))
            {
                return App.Current.Properties[settingName];
            }
            else
                return null;
        }
    }
}