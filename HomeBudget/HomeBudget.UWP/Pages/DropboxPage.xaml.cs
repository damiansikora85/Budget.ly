using Dropbox.Api;
using Dropbox.Api.Files;
using HomeBudget.Code;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Windows.Services.Store;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace HomeBudget.UWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DropboxPage : Page
    {
        private const string RedirectUri = "https://localhost/authorize/";
        private string appKey = "p6cayskxetnkx1a";
        private string oauth2State;
        private bool _checkDropboxFileExist;

        private StoreContext _storeContext;

        public DropboxPage()
        {
            this.InitializeComponent();
            ProgressRing.IsActive = false;
            _storeContext = StoreContext.GetDefault();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var response = await _storeContext.GetStoreProductsAsync(new List<string> { "Durable" }, new List<string> { "9NKFZRCQRZ3H" });
            if(response.Products.Count > 0)
            {
                //foreach(var product in response.Products)
                {

                }
                //var product = response.Products[0];
            }
        }

        private async void ConnectWithDropbox(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = await _storeContext.RequestPurchaseAsync("com.darktower.homebudget.dropbox");

                switch (result.Status)
                {
                    case StorePurchaseStatus.Succeeded:
                        _checkDropboxFileExist = false;
                        await LoginToDropbox();
                        break;
                    case StorePurchaseStatus.AlreadyPurchased:
                        _checkDropboxFileExist = false;
                        await LoginToDropbox();
                        break;
                    case StorePurchaseStatus.NotPurchased:
                        ShowToastNotification("Wystąpił błąd", result.ExtendedError != null ? result.ExtendedError.Message : "Not Purchased");
                        break;
                    case StorePurchaseStatus.NetworkError:
                        ShowToastNotification("Wystąpił błąd", result.ExtendedError != null ? result.ExtendedError.Message : "Network Error");
                        break;
                    case StorePurchaseStatus.ServerError:
                        ShowToastNotification("Wystąpił błąd", result.ExtendedError != null ? result.ExtendedError.Message : "Server Error");
                        break;
                }
            }
            catch (Exception exc)
            {
                var msg = exc.Message;
                ShowToastNotification("Wystąpił błąd", msg);
                // The in-app purchase was not completed because
                // an error occurred.
            }
        }

        private async void ConnectWithDataCheck(object sender, RoutedEventArgs e)
        {
            _checkDropboxFileExist = true;
            await LoginToDropbox();
        }

        private async Task LoginToDropbox()
        {
            oauth2State = Guid.NewGuid().ToString("N");
            var authorizeUri = DropboxOAuth2Helper.GetAuthorizeUri(OAuthResponseType.Token, appKey, new Uri(RedirectUri), state: oauth2State);

            var result = await WebAuthenticationBroker.AuthenticateAsync(
                WebAuthenticationOptions.None,
                authorizeUri,
                new Uri(RedirectUri));

            await ProcessResult(result);
        }

        private async Task ProcessResult(WebAuthenticationResult result)
        {
            switch (result.ResponseStatus)
            {
                case WebAuthenticationStatus.Success:
                    {
                        try
                        {
                            ProgressRing.IsActive = true;
                            var response = DropboxOAuth2Helper.ParseTokenFragment(new Uri(result.ResponseData));
                            Helpers.Settings.DropboxAccessToken = response.AccessToken;
                            if (!_checkDropboxFileExist)
                                MainBudget.Instance.OnCloudStorageConnected();
                            else if (await HasDropboxData())
                                MainBudget.Instance.OnCloudStorageConnected();
                            else
                            {
                                ProgressRing.IsActive = false;
                                ShowToastNotification("HomeBudget", "Dropbox data not found");
                            }
                        }
                        catch(Exception exc)
                        {
                            var msg = exc.Message;
                        }
                    }
                    break;

                    /*case WebAuthenticationStatus.ErrorHttp:
                        throw new OAuthException(result.ResponseErrorDetail);

                    case WebAuthenticationStatus.UserCancel:
                    default:
                        throw new OAuthUserCancelledException();*/
            }
        }

        private async Task<bool> HasDropboxData()
        {
            var accessToken = Helpers.Settings.DropboxAccessToken;
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

        private void ShowToastNotification(string title, string stringContent)
        {
            ToastNotifier ToastNotifier = ToastNotificationManager.CreateToastNotifier();
            Windows.Data.Xml.Dom.XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
            Windows.Data.Xml.Dom.XmlNodeList toastNodeList = toastXml.GetElementsByTagName("text");
            toastNodeList.Item(0).AppendChild(toastXml.CreateTextNode(title));
            toastNodeList.Item(1).AppendChild(toastXml.CreateTextNode(stringContent));
            Windows.Data.Xml.Dom.IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
            Windows.Data.Xml.Dom.XmlElement audio = toastXml.CreateElement("audio");
            audio.SetAttribute("src", "ms-winsoundevent:Notification.SMS");

            ToastNotification toast = new ToastNotification(toastXml);
            toast.ExpirationTime = DateTime.Now.AddSeconds(4);
            ToastNotifier.Show(toast);
        }
    }
}
