using Acr.UserDialogs;
using HomeBudgeStandard.Views;
using HomeBudget.Code;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeBudgeStandard.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainTabbedPage : TabbedPage
    {
        private bool _isOnline;
        private bool _waitingForSync;

        public MainTabbedPage ()
        {
            InitializeComponent();

            
            CurrentPageChanged += OnTabChanged;
            MainBudget.Instance.BudgetDataChanged += BudgetDataChanged;
            Connectivity.ConnectivityChanged += NetworkStateChanged;
        }

        private void NetworkStateChanged(object sender, ConnectivityChangedEventArgs e)
        {
            HandleNetworkState();
        }

        private void BudgetDataChanged(bool isLoadedFromCloud)
        {
            if (isLoadedFromCloud)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (CurrentPage != Children[0])
                    {
                        CurrentPage = Children[0];
                    }
                    UserDialogs.Instance.Toast("Zaktualizowano dane z Dropbox");
                    if(_waitingForSync)
                    {
                        UserDialogs.Instance.HideLoading();
                    }
                    _waitingForSync = false;
                });
            }
        }

        private void OnTabChanged(object sender, EventArgs e)
        {
            
        }

        public bool OnBackPressed()
        {
            return false;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            HandleNetworkState();
        }

        private void HandleNetworkState()
        {
            var wasOnline = _isOnline;
            _isOnline = Connectivity.NetworkAccess == NetworkAccess.Internet;
            if (!string.IsNullOrEmpty(HomeBudget.Helpers.Settings.DropboxAccessToken) && !_isOnline)
            {
                UserDialogs.Instance.Toast(new ToastConfig("Brak połączenia z Internetem") { MessageTextColor = Color.Red });
            }
            /*if (!wasOnline && _isOnline)
            {
                UserDialogs.Instance.ShowLoading("");
                _waitingForSync = true;
            }
            if(!_isOnline)
            {
                UserDialogs.Instance.HideLoading();
            }*/
        }
    }
}