using Firebase.RemoteConfig;
using HomeBudget.Code.Interfaces;
using HomeBudget.Standard;
using System.Threading.Tasks;
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
#if DEBUG
            var configSettings = new FirebaseRemoteConfigSettings.Builder().SetDeveloperModeEnabled(true).Build();
            _firebaseRemoteConfig.SetConfigSettings(configSettings);
            long cacheExpiration = 0;
#else
            var configSettings = new FirebaseRemoteConfigSettings.Builder().SetDeveloperModeEnabled(false).Build();
            _firebaseRemoteConfig.SetConfigSettings(configSettings);
            long cacheExpiration = 3600; // 1 hour in seconds.
#endif

            _firebaseRemoteConfig.SetDefaults(Resource.Xml.firebase_default_settings);
            Task.Factory.StartNew(async () =>
            {
                await _firebaseRemoteConfig.FetchAsync(cacheExpiration);
                _firebaseRemoteConfig.ActivateFetched();
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