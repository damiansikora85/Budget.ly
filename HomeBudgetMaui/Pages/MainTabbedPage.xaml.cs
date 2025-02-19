using Controls.UserDialogs.Maui;
using HomeBudget.Code;
using HomeBudget.Code.Interfaces;

namespace HomeBudgetStandard.Pages
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
                    UserDialogs.Instance.ShowToast("Zaktualizowano dane z Dropbox");
                    if(_waitingForSync)
                    {
                        UserDialogs.Instance.HideHud();
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
            //HandleNetworkState();
        }

        private void HandleNetworkState()
        {
             var wasOnline = _isOnline;
            _isOnline = Connectivity.NetworkAccess == NetworkAccess.Internet;
            if (!string.IsNullOrEmpty(_settings.CloudRefreshToken) && !_isOnline)
            {
                UserDialogs.Instance.ShowToast(new ToastConfig() { Message = "Brak połączenia z Internetem", MessageColor = Colors.Red });
            }
        }
    }
}