using System.Threading.Tasks;
using Firebase.RemoteConfig;
using HomeBudget.Standard;
using Xamarin.Forms;

[assembly: Dependency(typeof(HomeBudget.Droid.Native.CustomFirebaseRemoteConfig))]
namespace HomeBudget.Droid.Native
{
    public class CustomFirebaseRemoteConfig : IRemoteConfig
    {
        private FirebaseRemoteConfig _firebaseRemoteConfig;

        public string GetActiveInappName()
        {
            return _firebaseRemoteConfig.GetString("active_inapp_product_name");
        }

        public void Init()
        {
            _firebaseRemoteConfig = FirebaseRemoteConfig.Instance;
            var configSettings = new FirebaseRemoteConfigSettings.Builder().SetMinimumFetchIntervalInSeconds(3600).Build();
            _firebaseRemoteConfig.SetConfigSettingsAsync(configSettings);

            _firebaseRemoteConfig.SetDefaultsAsync(Resource.Xml.firebase_default_settings);
            Task.Factory.StartNew(() =>
            {
                _firebaseRemoteConfig.FetchAndActivate();
            });
        }

        public bool IsFeatureEnabled(string featureName)
        {
            return _firebaseRemoteConfig.GetBoolean(featureName);
        }

        public bool IsPromoActive()
        {
            return _firebaseRemoteConfig.GetBoolean("is_promo_active");
        }
    }
}