using Acr.UserDialogs;
using HomeBudgeStandard.Views;
using HomeBudget.Code;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeBudgeStandard.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainTabbedPage : TabbedPage
    {
        public MainTabbedPage ()
        {
            InitializeComponent();

            CurrentPageChanged += OnTabChanged;
            MainBudget.Instance.BudgetDataChanged += BudgetDataChanged;
        }

        private void BudgetDataChanged(bool isLoadedFromCloud)
        {
            if (isLoadedFromCloud)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (CurrentPage != Children[0])
                        CurrentPage = Children[0];
                    UserDialogs.Instance.Toast("Zaktualizowano dane z Dropbox");
                }
                );
            }
        }

        private void OnTabChanged(object sender, EventArgs e)
        {
            
        }

        public bool OnBackPressed()
        {
            //if (CurrentPage is SummaryView summaryView)
                //return summaryView.OnBackPressed();

            return false;
        }
    }
}