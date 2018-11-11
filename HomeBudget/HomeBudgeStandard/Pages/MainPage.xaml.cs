using HomeBudgeStandard.Utils;
using HomeBudget.Code;
using HomeBudgetShared.Code.Synchronize;
using System;
using System.Globalization;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeBudgeStandard.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : MasterDetailPage
    {
        public MainPage()
        {
            CultureInfo.CurrentCulture = new CultureInfo("pl-PL");
            InitializeComponent();
            InitBudget();
            MasterPage.ListView.ItemSelected += ListView_ItemSelected;
        }

        public bool OnBackPressed()
        {
            if(Detail is NavigationPage navigationPage && navigationPage.RootPage is MainTabbedPage mainTabbedPage)
            {
                return mainTabbedPage.OnBackPressed();
            }
            return false;
        }

        private void InitBudget()
        {
            MainBudget.Instance.Init(new FileManagerXamarin(), new BudgetSynchronizer(new DropboxCloudStorage()));
        }

        public void AfterCloudLogin()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Detail = new NavigationPage(new MainTabbedPage());
                IsPresented = false;
                });
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as MainPageMenuItem;
            if (item == null)
                return;

            var page = (Page)Activator.CreateInstance(item.TargetType);
            page.Title = item.Title;

            Detail = new NavigationPage(page);
            IsPresented = false;

            MasterPage.ListView.SelectedItem = null;
        }
    }
}