using Acr.UserDialogs;
using Dropbox.Api;
using Dropbox.Api.Files;
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
    public partial class SettingsPage : ContentPage
    {
        private const string _redirectUri = "https://localhost/authorize";
        private string _appKey = "p6cayskxetnkx1a";
        private string _oauth2State;
        private bool _checkDropboxFileExist;

        public bool HasAccessToken
        {
            get => !string.IsNullOrEmpty(HomeBudget.Helpers.Settings.DropboxAccessToken);
        }

        public SettingsPage ()
		{
			InitializeComponent ();
            BindingContext = this;
		}

        private async void OnLoginDropbox(object sender, EventArgs e)
        {
            _checkDropboxFileExist = false;
            await LoginToDropbox();
        }

        private async void OnLoginDropboxWithDataCheck(object sender, EventArgs args)
        {
            _checkDropboxFileExist = true;
            await LoginToDropbox();
        }

        private async Task LoginToDropbox()
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
                

            }
            catch (ArgumentException argExc)
            {
                var msg = argExc.Message;
                msg += "error";
                // There was an error in the URI passed to ParseTokenFragment
            }
            finally
            {
                await Application.Current.MainPage.Navigation.PopModalAsync();

                if (!_checkDropboxFileExist)
                {
                    MainBudget.Instance.OnCloudStorageConnected();
                    var mainPage = Application.Current.MainPage as MainPage;
                    mainPage.AfterCloudLogin();
                }
                else if(await HasDropboxData())
                {
                    MainBudget.Instance.OnCloudStorageConnected();   
                    var mainPage = Application.Current.MainPage as MainPage;
                    mainPage.AfterCloudLogin();
                }
                else UserDialogs.Instance.Toast("Nie znaleziono pliku");  
            }
        }

        private async Task<bool> HasDropboxData()
        {
            var accessToken = HomeBudget.Helpers.Settings.DropboxAccessToken;
            var hasData = false;
            using (var dropboxClient = new DropboxClient(accessToken))
            {
                try
                {
                    var metadata = await dropboxClient.Files.GetMetadataAsync(DropboxCloudStorage.DROPBOX_DATA_FILE_PATH);
                    hasData = metadata.IsFile;
                }
                catch (ApiException<GetMetadataError> apiExc)
                {
                    hasData = false;
                }
                catch (Exception e)
                {
                    hasData = false;
                }
            }

            return hasData;
        }
    }
}