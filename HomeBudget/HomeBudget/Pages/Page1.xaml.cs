using Dropbox.Api;
using Dropbox.Api.Files;
using Syncfusion.SfCalendar.XForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeBudget
{

    //[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Page1 : ContentPage
    {
        private const string RedirectUri = "https://localhost/authorize";
        private string appKey = "p6cayskxetnkx1a";
        private string oauth2State;
        public string AccessToken { get; private set; }
        DropboxClient dropboxClient;

        public Page1()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            if(Helpers.Settings.DropboxAccessToken != string.Empty)
            {
                this.AccessToken = Helpers.Settings.DropboxAccessToken;
                ConnectButton.IsEnabled = false;
                dropboxClient = new DropboxClient(AccessToken);
            }
        }

        private async void Connect(object sender, EventArgs e)
        {
            this.oauth2State = Guid.NewGuid().ToString("N");
            var authorizeUri = DropboxOAuth2Helper.GetAuthorizeUri(OAuthResponseType.Token, appKey, new Uri(RedirectUri), state: oauth2State);

            var webView = new WebView { Source = new UrlWebViewSource { Url = authorizeUri.AbsoluteUri } };
            webView.Navigating += this.WebViewOnNavigating;
            var contentPage = new ContentPage { Content = webView };
            await Navigation.PushModalAsync(contentPage);
        }

        private async void Upload(object sender, EventArgs e)
        {
            //MemoryStream data = new MemoryStream(Code.MainBudget.Instance.GetData());
            //await dropboxClient.Files.UploadAsync("/test22.dat", WriteMode.Overwrite.Instance, false, null, false, data);
        }

        private async void Download(object sender, EventArgs e)
        {
            using (var response = await dropboxClient.Files.DownloadAsync("/test22.dat"))
            {
                byte[] data = await response.GetContentAsByteArrayAsync();
                //Code.MainBudget.Instance.UpdateData(data);
            }
        }

        private async void Back(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        async Task Upload()
        {
            //MemoryStream data = new MemoryStream(Code.MainBudget.Instance.GetData());
            //await dropboxClient.Files.UploadAsync("/budget/test.dat", null, false, null, false, data);
        }

        async Task Download(DropboxClient dbx, string folder, string file)
        {
            using (var response = await dbx.Files.DownloadAsync(folder + "/" + file))
            {
                //Console.WriteLine(await response.GetContentAsStringAsync());
            }
        }

        private async void WebViewOnNavigating(object sender, WebNavigatingEventArgs e)
        {
            if (!e.Url.StartsWith(RedirectUri, StringComparison.OrdinalIgnoreCase))
            {
                // we need to ignore all navigation that isn't to the redirect uri.
                return;
            }

            try
            {
                var result = DropboxOAuth2Helper.ParseTokenFragment(new Uri(e.Url));

                if (result.State != this.oauth2State)
                {
                    return;
                }

                this.AccessToken = result.AccessToken;
                Helpers.Settings.DropboxAccessToken = this.AccessToken;

                dropboxClient = new DropboxClient(AccessToken);

                //await SaveDropboxToken(this.AccessToken);
                //this.OnAuthenticated?.Invoke();
            }
            catch (ArgumentException argExc)
            {
                string msg = argExc.Message;
                msg += "error";
                // There was an error in the URI passed to ParseTokenFragment
            }
            finally
            {
                e.Cancel = true;
                await Application.Current.MainPage.Navigation.PopModalAsync();
            }
        }
    }
}
