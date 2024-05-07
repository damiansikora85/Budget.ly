using CommunityToolkit.Maui.Views;

namespace HomeBudgeStandard.Pages
{
    public partial class NewFeaturePopup : Popup
    {
        public string FeatureName { get; set; }
        public string FeatureDescription { get; set; }

        private Action _onTryNow;
        public NewFeaturePopup(string featureName, string featureDesc, Action onTryNow)
        {
            FeatureName = featureName;
            FeatureDescription = featureDesc;
            _onTryNow = onTryNow;

            InitializeComponent();
            BindingContext = this;
        }

        private async void OnTryNowClicked(object sender, EventArgs e)
        {
            await CloseAsync();
            _onTryNow?.Invoke();
        }

        private async void OnCheckLaterClicked(object sender, EventArgs e)
        {
            await CloseAsync();
        }
    }
}