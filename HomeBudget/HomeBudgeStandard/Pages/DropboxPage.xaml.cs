using Acr.UserDialogs;
using Dropbox.Api;
using Dropbox.Api.Files;
using HomeBudgeStandard.Interfaces.Impl;
using HomeBudget.Code;
using System;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeBudgeStandard.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DropboxPage : ContentPage
    {
        private const string _redirectUri = "https://localhost/authorize";
        private string _appKey = "p6cayskxetnkx1a";
        private string _oauth2State;
        private bool _checkDropboxFileExist;
        private bool _waitingForDropboxResponse;

        public string SynchronizationStatus => string.IsNullOrEmpty(HomeBudget.Helpers.Settings.DropboxAccessToken)?"\uf057":"\uf058";
        public string RegularPrice { get; private set; }
        public string PromoPrice { get; private set; }
        public bool DropboxConnected => !string.IsNullOrEmpty(HomeBudget.Helpers.Settings.DropboxAccessToken);
        public bool DropboxNotConnected => string.IsNullOrEmpty(HomeBudget.Helpers.Settings.DropboxAccessToken);

        public bool HasAccessToken
        {
            get => !string.IsNullOrEmpty(HomeBudget.Helpers.Settings.DropboxAccessToken);
        }

        public DropboxPage()
        {
            InitializeComponent();
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
            _waitingForDropboxResponse = true;
            await Navigation.PushModalAsync(contentPage);
        }

        protected async override void OnAppearing()
        {
            /*var purchaseService = new PurchaseService();
            var info = await purchaseService.GetProductInfo("com.darktower.homebudget.dropbox");
            if (info != null)
            {
                PromoPrice = info.LocalizedPrice;
                OnPropertyChanged(nameof(PromoPrice));

                //temp
                RegularPrice = info.LocalizedPrice;
                OnPropertyChanged(nameof(RegularPrice));
            }
            info = null;
            info = await purchaseService.GetProductInfo("com.darktower.homebudget.dropboxnormal ");
            if (info != null)
            {
                RegularPrice = info.LocalizedPrice;
                OnPropertyChanged(nameof(RegularPrice));
            }*/

            //DropboxSynchroBought = await purchaseService.IsProductAlreadyBought("com.darktower.homebudget.dropbox");
            //OnPropertyChanged(nameof(IsNotBoughtYet));

            base.OnAppearing();
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
                UserDialogs.Instance.ShowLoading("");
                var result = DropboxOAuth2Helper.ParseTokenFragment(new Uri(e.Url));

                if (result.State != _oauth2State)
                {
                    return;
                }
                HomeBudget.Helpers.Settings.DropboxAccessToken = result.AccessToken;

                await Application.Current.MainPage.Navigation.PopModalAsync();

                if (await HasDropboxData())
                {
                    if (await UserDialogs.Instance.ConfirmAsync("Wykryto zapisane dane aplikacji Budget.ly na Twoim koncie Dropbox. Czy chcesz nadpisać dane lokalne w telefonie?", "Uwaga", "Użyj danych z Dropbox", "Użyj danych z  telefonu"))
                    {
                        UserDialogs.Instance.HideLoading();
                        MainBudget.Instance.OnCloudStorageConnected(true);
                        var mainPage = Application.Current.MainPage as MainPage;
                        mainPage.AfterCloudLogin();
                    }
                    else
                    {
                        UserDialogs.Instance.HideLoading();
                        MainBudget.Instance.OnCloudStorageConnected(false);
                        var mainPage = Application.Current.MainPage as MainPage;
                        mainPage.AfterCloudLogin();
                    }
                }
                else
                {
                    UserDialogs.Instance.HideLoading();
                    MainBudget.Instance.OnCloudStorageConnected(false);
                    var mainPage = Application.Current.MainPage as MainPage;
                    mainPage.AfterCloudLogin();
                }
            }
            catch (ArgumentException argExc)
            {
                UserDialogs.Instance.HideLoading();
                await UserDialogs.Instance.AlertAsync(argExc.Message, "Uwaga", "Dalej");
            }
            catch (Exception exc)
            {
                UserDialogs.Instance.HideLoading();
                await UserDialogs.Instance.AlertAsync(exc.Message, "Uwaga", "Dalej");
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