using Dropbox.Api;
using HomeBudget.Code;
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
    public partial class MainPageDetail : ContentPage
    {
        private const string _redirectUri = "https://localhost/authorize";
        private string _appKey = "p6cayskxetnkx1a";
        private string _oauth2State;

        public MainPageDetail()
        {
            InitializeComponent();
        }

        private async void LoginDropbox(object sender, EventArgs e)
        {
            _oauth2State = Guid.NewGuid().ToString("N");
            var authorizeUri = DropboxOAuth2Helper.GetAuthorizeUri(OAuthResponseType.Token, _appKey, new Uri(_redirectUri), state: _oauth2State);

            var webView = new WebView { Source = new UrlWebViewSource { Url = authorizeUri.AbsoluteUri } };
            webView.Navigating += WebViewOnNavigating;
            var contentPage = new ContentPage { Content = webView };
            await Navigation.PushModalAsync(contentPage);
        }

        private async void WebViewOnNavigating(object sender, WebNavigatingEventArgs e)
        {
            if (!e.Url.StartsWith(_redirectUri.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                // we need to ignore all navigation that isn't to the redirect uri.  
                return;
            }

            try
            {
                var result = DropboxOAuth2Helper.ParseTokenFragment(new Uri(e.Url));

                if (result.State != _oauth2State)
                {
                    return;
                }
                HomeBudget.Helpers.Settings.DropboxAccessToken = result.AccessToken;
                MainBudget.Instance.OnCloudStorageConnected();                
            }
            catch (ArgumentException argExc)
            {
                var msg = argExc.Message;
                msg += "error";
                // There was an error in the URI passed to ParseTokenFragment
            }
            finally
            {
                e.Cancel = true;
                
                await Application.Current.MainPage.Navigation.PopModalAsync();
                var mainPage = Application.Current.MainPage as MainPage;
                mainPage.AfterCloudLogin();

            }
        }
    }
}