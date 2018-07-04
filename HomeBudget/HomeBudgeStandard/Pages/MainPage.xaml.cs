using HomeBudgeStandard.Utils;
using HomeBudget.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeBudgeStandard.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : MasterDetailPage
    {
        public MainPage()
        {
            InitializeComponent();
            InitBudget();
            MasterPage.ListView.ItemSelected += ListView_ItemSelected;
            Detail = new NavigationPage(new MainTabbedPage());
        }

        private void InitBudget()
        {
            MainBudget.Instance.onBudgetLoaded += OnBudgetLoaded;
            MainBudget.Instance.Init(new FileManagerXamarin(), new DropboxManager());
        }

        private void OnBudgetLoaded()
        {
            //Device.BeginInvokeOnMainThread(() => Detail = new NavigationPage(new MainTabbedPage()));
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