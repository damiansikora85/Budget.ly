using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeBudgeStandard.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewFeaturePopup : PopupPage
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
            await Navigation.PopPopupAsync();
            _onTryNow?.Invoke();
        }

        private async void OnCheckLaterClicked(object sender, EventArgs e)
        {
            await Navigation.PopPopupAsync();
        }
    }
}