using Controls.UserDialogs.Maui;
using Dropbox.Api;
using Dropbox.Api.Files;
using Firebase.Crashlytics;
using HomeBudget.Code;
using HomeBudget.Code.Interfaces;
using HomeBudget.Standard;
using HomeBudgetStandard.Interfaces.Impl;

namespace HomeBudgetStandard.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DropboxPage : ContentPage
    {
        private const string _redirectUri = "https://localhost/authorize";
        private string _appKey = "p6cayskxetnkx1a";
        private const string LoopbackHost = "http://127.0.0.1:52475/";
        private string _oauth2State;
        private bool _checkDropboxFileExist;
        private bool _waitingForDropboxResponse;

        public string SynchronizationStatus => string.IsNullOrEmpty(_settings.CloudRefreshToken)?"\uf057":"\uf058";
        public string RegularPrice { get; private set; }
        public string PromoPrice { get; private set; }
        public bool DropboxConnected => !string.IsNullOrEmpty(_settings.CloudRefreshToken);
        public bool DropboxNotConnected => string.IsNullOrEmpty(_settings.CloudRefreshToken);

        private PurchaseService _purchaseService;
        private string _iapName;
        private bool _isPromo;
        private ISettings _settings;

        public bool HasAccessToken
        {
            get => !string.IsNullOrEmpty(_settings.CloudAccessToken);
        }

        public DropboxPage(ISettings settings)
        {
            _settings = settings;
            InitializeComponent();
            BindingContext = this;
            _purchaseService = new PurchaseService();
        }

        private async void OnLoginDropbox(object sender, EventArgs e)
        {
            _checkDropboxFileExist = true;
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
            var authorizeUri = DropboxOAuth2Helper.GetAuthorizeUri(OAuthResponseType.Code, _appKey, new Uri(_redirectUri), state: _oauth2State, tokenAccessType: TokenAccessType.Offline);
            //DropboxOAuth2Helper.ProcessCodeFlowAsync
            var webView = new WebView { Source = new UrlWebViewSource { Url = authorizeUri.AbsoluteUri } };
            webView.Navigating += WebViewOnNavigating;
            var contentPage = new ContentPage { Content = webView };
            _waitingForDropboxResponse = true;
            await Navigation.PushModalAsync(contentPage);
        }

        protected async override void OnAppearing()
        {
            UserDialogs.Instance.ShowLoading("");

            try
            {
#if (CUSTOM)
                _iapName = "com.darktower.homebudget.dropboxnormal";
                _isPromo = false;
#else
                var remoteConfig = RemoteConfig.Instance;
                _iapName = remoteConfig.GetActiveInappName();
                _isPromo = remoteConfig.IsPromoActive();
#endif

                if (await IsAnyProductBought())
                {
                    iapLayout.IsVisible = false;
                    connectLayout.IsVisible = true;
                    if (string.IsNullOrEmpty(_settings.CloudRefreshToken))
                    {
                        //if synchro bought but not active
                        resyncButton.IsVisible = true;
                        synchroStatusLabel.Text = "Synchronizacja nieaktywna";
                    }
                    else
                    {
                        //synchro is active
                        resyncButton.IsVisible = false;
                        synchroStatusLabel.Text = "Synchronizacja jest aktywna";
                    }
                }
                else if (_isPromo)
                {
                    iapLayout.IsVisible = true;
                    connectLayout.IsVisible = false;
                    var promoItemInfo = await _purchaseService.GetProductInfo(_iapName);
                    var regularItemInfo = await _purchaseService.GetProductInfo("com.darktower.homebudget.dropboxnormal");
                    if (promoItemInfo != null && regularItemInfo != null)
                    {
                        PromoPrice = $"  {promoItemInfo.LocalizedPrice}";
                        RegularPrice = regularItemInfo.LocalizedPrice;

                        OnPropertyChanged(nameof(PromoPrice));
                        OnPropertyChanged(nameof(RegularPrice));
                    }
                }
                else
                {
                    iapLayout.IsVisible = true;
                    connectLayout.IsVisible = false;

                    var regularItemInfo = await _purchaseService.GetProductInfo("com.darktower.homebudget.dropboxnormal");
                    if (regularItemInfo != null)
                    {
                        PromoPrice = regularItemInfo.LocalizedPrice;
                        OnPropertyChanged(nameof(PromoPrice));
                    }
                }
            }
            catch (Exception exc)
            {
                FirebaseCrashlytics.Instance.RecordException(Java.Lang.Throwable.FromException(exc));
            }
            finally
            {
                UserDialogs.Instance.HideHud();
            }
        }

        private async Task<bool> IsAnyProductBought()
        {
#if CUSTOM
            return await Task.FromResult(true);
#else
            return await _purchaseService.IsProductAlreadyBought("com.darktower.homebudget.dropbox") ||
                           await _purchaseService.IsProductAlreadyBought("com.darktower.homebudget.dropboxnormal");
#endif
        }

        private async void OnIapClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_iapName)) return;

            _checkDropboxFileExist = false;

            var result = await _purchaseService.MakePurchase(_iapName);
            if (result)
            {
                await LoginToDropbox();
            }
        }

        private async void OnConsume(object sender, EventArgs e)
        {
            await _purchaseService.ConsumeProduct("com.darktower.homebudget.dropbox");
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
                var code = GetCodeFromUrl(e.Url);
                var result = await DropboxOAuth2Helper.ProcessCodeFlowAsync(code, DropboxCloudStorage.AppKey, DropboxCloudStorage.AppSecret, _redirectUri);

                _settings.CloudAccessToken = result.AccessToken;
                _settings.CloudRefreshToken = result.RefreshToken;

                await Application.Current.MainPage.Navigation.PopModalAsync();

                if (await HasDropboxData())
                {
                    if (await UserDialogs.Instance.ConfirmAsync("Wykryto zapisane dane aplikacji Budget.ly na Twoim koncie Dropbox. Czy chcesz nadpisać dane lokalne w telefonie?", "Uwaga", "Użyj danych z Dropbox", "Użyj danych z  telefonu"))
                    {
                        UserDialogs.Instance.ShowLoading("");
                        await MainBudget.Instance.OnCloudStorageConnected(true);
                        UserDialogs.Instance.HideHud();
                        var mainPage = Application.Current.MainPage as MainPage;
                        mainPage.AfterCloudLogin();
                    }
                    else
                    {
                        UserDialogs.Instance.HideHud();
                        await MainBudget.Instance.OnCloudStorageConnected(false);
                        var mainPage = Application.Current.MainPage as MainPage;
                        mainPage.AfterCloudLogin();
                    }
                }
                else if(_checkDropboxFileExist)
                {
                    _settings.CloudAccessToken = "";
                    _settings.CloudRefreshToken = "";
                    UserDialogs.Instance.HideHud();
                    if(await UserDialogs.Instance.ConfirmAsync("Podane dane są nieprawidłowe", "Uwaga", "Spróbuj ponownie", "Anuluj"))
                    {
                        await LoginToDropbox();
                    }
                }
                else
                {
                    UserDialogs.Instance.HideHud();
                    await MainBudget.Instance.OnCloudStorageConnected(false);
                    var mainPage = Application.Current.MainPage as MainPage;
                    mainPage.AfterCloudLogin();
                }
            }
            catch (ArgumentException argExc)
            {
                UserDialogs.Instance.HideHud();
                await UserDialogs.Instance.AlertAsync(argExc.Message, "Uwaga", "Dalej");
            }
            catch (Exception exc)
            {
                UserDialogs.Instance.HideHud();
                await UserDialogs.Instance.AlertAsync(exc.Message, "Uwaga", "Dalej");
            }
        }

        private string GetCodeFromUrl(string url)
        {
            var uri = new Uri(url);
            var queryDictionary = System.Web.HttpUtility.ParseQueryString(uri.Query);
            if (queryDictionary["code"] != null)
            {
                return queryDictionary["code"];
            }

            return string.Empty;
        }

        private async Task<bool> HasDropboxData()
        {
            if(string.IsNullOrEmpty(_settings.CloudRefreshToken))
            {
                return false;
            }

            var refreshToken = _settings.CloudRefreshToken;
            var hasData = false;
            using (var dropboxClient = new DropboxClient(refreshToken, DropboxCloudStorage.AppKey, DropboxCloudStorage.AppSecret))
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