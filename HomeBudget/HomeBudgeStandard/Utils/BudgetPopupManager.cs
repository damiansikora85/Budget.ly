using HomeBudgeStandard.Pages;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

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
            var lastRatePopupDisplayedDate = Xamarin.Essentials.Preferences.Get("ratePopupDisplayDate", DateTime.MinValue);
            if ((DateTime.Now - lastRatePopupDisplayedDate).TotalDays >= 5 && Xamarin.Essentials.Preferences.Get("shouldShowRatePopup", true))
            {
                _navigation.PushPopupAsync(new RatePage());
                Xamarin.Essentials.Preferences.Set("ratePopupDisplayDate", DateTime.Now);
            }
        }

        private void TryNewFeatureInfo()
        {
            if (Xamarin.Essentials.Preferences.Get("categoryEdit", true))
            {
                Xamarin.Essentials.Preferences.Set("categoryEdit", false);

                _navigation.PushPopupAsync(new NewFeaturePopup("Edycja kategorii", "Zarządzaj swoimi wydatkami i dochodami tak jak chcesz. Teraz możesz dostosować szablon kategorii do Twoich potrzeb. Stwórz prawdziwy budżet osobisty!",
                async () =>
                {
                    if (_parentPage is MainTabbedPage tabbedPage)
                    {
                        await tabbedPage.Navigation.PushAsync(new BudgetTemplateEditPage()).ConfigureAwait(false);
                    }
                })
                { CloseWhenBackgroundIsClicked = false });
            }
        }

        private void TryFirstLaunchInfo()
        {
            if (Xamarin.Essentials.Preferences.Get("firstLaunch", true))
            {
                Xamarin.Essentials.Preferences.Set("firstLaunch", false);
                Xamarin.Essentials.Preferences.Set("categoryEdit", false);
                _navigation.PushPopupAsync(new WelcomePopup());
            }
        }
    }
}
