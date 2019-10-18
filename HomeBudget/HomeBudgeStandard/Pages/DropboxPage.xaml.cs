using Acr.UserDialogs;
using Dropbox.Api;
using Dropbox.Api.Files;
using HomeBudgeStandard.Interfaces.Impl;
using HomeBudget.Code;
using HomeBudget.Standard;
using Microsoft.AppCenter.Crashes;
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

        private PurchaseService _purchaseService;
        private string _iapName;
        private bool _isPromo;

        public bool HasAccessToken
        {
            get => !string.IsNullOrEmpty(HomeBudget.Helpers.Settings.DropboxAccessToken);
        }

        public DropboxPage()
        {
            InitializeComponent();
            BindingContext = this;
            _purchaseService = new PurchaseService();
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
            UserDialogs.Instance.ShowLoading("");

            try
            {
#if (CUSTOM)
                _iapName = "com.darktower.homebudget.dropboxnormal";
                _isPromo = false;
#else
                _iapName = DependencyService.Get<IRemoteConfig>().GetActiveInappName();
                _isPromo = DependencyService.Get<IRemoteConfig>().IsPromoActive();
#endif

                if (await IsAnyProductBought())
                {
                    iapLayout.IsVisible = false;
                    connectLayout.IsVisible = true;
                    if (string.IsNullOrEmpty(HomeBudget.Helpers.Settings.DropboxAccessToken))
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
                Crashes.TrackError(exc);
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
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
                var result = DropboxOAuth2Helper.ParseTokenFragment(new Uri(e.Url));

                if (result.State != _oauth2State)
                {
                    return;
                }
                HomeBudget.Helpers.Settings.DropboxAccessToken = result.AccessToken;

                await Application.Current.MainPage.Navigation.PopModalAsync();

                if (await HasDropboxData() && _checkDropboxFileExist)
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
                else if(_checkDropboxFileExist)
                {
                    HomeBudget.Helpers.Settings.DropboxAccessToken = "";
                    UserDialogs.Instance.HideLoading();
                    if(await UserDialogs.Instance.ConfirmAsync("Podane dane są nieprawidłowe", "Uwaga", "Spróbuj ponownie", "Anuluj"))
                    {
                        await LoginToDropbox();
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
            if(string.IsNullOrEmpty(HomeBudget.Helpers.Settings.DropboxAccessToken))
            {
                return false;
            }

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