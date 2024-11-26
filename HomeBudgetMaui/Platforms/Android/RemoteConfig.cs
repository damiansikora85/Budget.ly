using System.Threading.Tasks;
using Firebase.RemoteConfig;

namespace HomeBudget.Standard
{
    public partial class RemoteConfig : IRemoteConfig
    {
        private FirebaseRemoteConfig _firebaseRemoteConfig;

        public partial string GetActiveInappName()
        {
            return _firebaseRemoteConfig.GetString("active_inapp_product_name");
        }

        public partial void Init()
        {
            _firebaseRemoteConfig = FirebaseRemoteConfig.Instance;
            var configSettings = new FirebaseRemoteConfigSettings.Builder().SetMinimumFetchIntervalInSeconds(3600).Build();
            _firebaseRemoteConfig.SetConfigSettingsAsync(configSettings);

            _firebaseRemoteConfig.SetDefaultsAsync(HomeBudgetMaui.Resource.Xml.firebase_default_settings);
            Task.Factory.StartNew(() =>
            {
                _firebaseRemoteConfig.FetchAndActivate();
            });
        }

        public partial bool IsFeatureEnabled(string featureName)
        {
            return _firebaseRemoteConfig.GetBoolean(featureName);
        }

        public partial bool IsPromoActive()
        {
            return _firebaseRemoteConfig.GetBoolean("is_promo_active");
        }
    }
}