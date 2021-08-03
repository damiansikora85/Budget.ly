// Helpers/Settings.cs
using HomeBudget.Code.Interfaces;
using Xamarin.Essentials;

namespace HomeBudget.Helpers
{
    /// <summary>
    /// This is the Settings static class that can be used in your Core solution or in any
    /// of your client applications. All settings are laid out the same exact way with getters
    /// and setters.
    /// </summary>
    public class Settings : ISettings
    {
        private const string DropboxAccessTokenKey = "dropboxAccessToken_key";
        private const string DropboxAccessTokenDefault = "";
        private const string FirstLaunchKey = "first_launch";
        private const string NeedSetupNotificationsKey = "setup_notifications";
        private const string DropboxRefreshTokenKey = "dropboxRefreshToken";

        public string CloudAccessToken
        {
            get => Preferences.Get(DropboxAccessTokenKey, DropboxAccessTokenDefault);
            set => Preferences.Set(DropboxAccessTokenKey, value);
        }

        public bool FirstLaunch
        {
            get => Preferences.Get(FirstLaunchKey, true);
            set => Preferences.Set(FirstLaunchKey, value);
        }
        public bool NeedSetupDefaultNotifications
        {
            get => Preferences.Get(NeedSetupNotificationsKey, true);
            set => Preferences.Set(NeedSetupNotificationsKey, value);
        }
        public string CloudRefreshToken
        {
            get => Preferences.Get(DropboxRefreshTokenKey, string.Empty);
            set => Preferences.Set(DropboxRefreshTokenKey, value);
        }
    }
}