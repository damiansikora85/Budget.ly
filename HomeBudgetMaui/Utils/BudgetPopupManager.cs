namespace HomeBudget.Utils
{
    public class BudgetPopupManager
    {
        private bool _isPopupDisplaying;
        private readonly INavigation _navigation;
        private Page _parentPage;
        private object _displayLock = new object();

        public BudgetPopupManager(Page parentPage, INavigation navigation)
        {
            _parentPage = parentPage;
            _navigation = navigation;
        }

        public void TryDisplayPopup()
        {
            lock (_displayLock)
            {
                TryFirstLaunchInfo();
                if (!_isPopupDisplaying)
                {
                    TryNewFeatureInfo();
                    TryShowRatePopup();
                }
            }
        }

        private void TryShowRatePopup()
        {
            _isPopupDisplaying = true;
            var lastRatePopupDisplayedDate = Preferences.Get("ratePopupDisplayDate", DateTime.MinValue);
            if ((DateTime.Now - lastRatePopupDisplayedDate).TotalDays >= 5 && Preferences.Get("shouldShowRatePopup", true))
            {
                //_navigation.PushPopupAsync(new RatePage());
                Preferences.Set("ratePopupDisplayDate", DateTime.Now);
            }
        }

        private void TryNewFeatureInfo()
        {
            if (Preferences.Get("categoryEdit", true))
            {
                Preferences.Set("categoryEdit", false);

                //_navigation.PushPopupAsync(new NewFeaturePopup("Edycja kategorii", "Zarządzaj swoimi wydatkami i dochodami tak jak chcesz. Teraz możesz dostosować szablon kategorii do Twoich potrzeb. Stwórz prawdziwy budżet osobisty!",
                //async () =>
                //{
                //    if (_parentPage is MainTabbedPage tabbedPage)
                //    {
                //        await tabbedPage.Navigation.PushAsync(new BudgetTemplateEditPage()).ConfigureAwait(false);
                //    }
                //})
                //{ CloseWhenBackgroundIsClicked = false });
            }
        }

        private void TryFirstLaunchInfo()
        {
            if (Preferences.Get("firstLaunch", true))
            {
                Preferences.Set("firstLaunch", false);
                Preferences.Set("categoryEdit", false);
                //_navigation.PushPopupAsync(new WelcomePopup());
            }
        }
    }
}
