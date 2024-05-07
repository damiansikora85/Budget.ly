using Acr.UserDialogs;
using HomeBudgeStandard.Views;
using HomeBudget.Code;
using HomeBudget.Code.Interfaces;
using System;
using Microsoft.Maui;

namespace HomeBudgeStandard.Pages
{
    public partial class MainTabbedPage : TabbedPage
    {
        private bool _isOnline;
        private bool _waitingForSync;
        private ISettings _settings;

        public MainTabbedPage ()
        {
            InitializeComponent();
            MainBudget.Instance.BudgetDataChanged += BudgetDataChanged;
            Connectivity.ConnectivityChanged += NetworkStateChanged;
        }

        public void SetSettings(ISettings settings)
        {
            _settings = settings;
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
            if (!string.IsNullOrEmpty(_settings.CloudRefreshToken) && !_isOnline)
            {
                UserDialogs.Instance.Toast(new ToastConfig("Brak połączenia z Internetem") { MessageTextColor = System.Drawing.Color.Red });
            }
        }
    }
}