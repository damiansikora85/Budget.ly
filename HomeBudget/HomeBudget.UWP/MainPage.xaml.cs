using HomeBudget.Code;
using HomeBudget.UWP.Pages;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HomeBudget.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            //new Syncfusion.SfChart.XForms.UWP.SfChartRenderer();
            //Syncfusion.SfDataGrid.XForms.UWP.SfDataGridRenderer.Init();

            this.InitializeComponent();

            //ApplicationView.PreferredLaunchViewSize = new Size(800, 600);
            //ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;

            //LoadApplication(new HomeBudget.App());


            if (!MainBudget.Instance.IsInitialized)
                InitBudget();
            else
                OnBudgetLoaded();
        }

        private void InitBudget()
        {
            MainBudget.Instance.Init();
            MainBudget.Instance.onBudgetLoaded += OnBudgetLoaded;

            /*if (!string.IsNullOrEmpty(Helpers.Settings.DropboxAccessToken))
            {
                //DropboxButtonChangeVisibility(false);
                //DropboxLoginElement.IsVisible = false;
                DropboxManager.Instance.DownloadData();
            }
            else*/
            {
                //DropboxButtonChangeVisibility(true);
                MainBudget.Instance.Load();
            }
        }

        private void OnBudgetLoaded()
        {
            //SetupBudgetSummary();
            /*CreateCategoriesBar();
            CreateIncomesBar();
            SetupBudgetSummary();
            budgetSummaryElement = new BudgetSummaryListView();
            budgetSummaryElement.Setup(listView);
            DropboxButtonChangeVisibility(false);

            UserDialogs.Instance.HideLoading();*/

            foreach (NavigationViewItemBase item in NavView.MenuItems)
            {
                if (item is NavigationViewItem && item.Tag.ToString() == "home")
                {
                    NavView.SelectedItem = item;
                    break;
                }
            }
        }

        

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {

            // set the initial SelectedItem 
            

            //ContentFrame.Navigate(typeof(HomePage));

        }

        private void NavView_SelectionChanged(NavigationView view, NavigationViewSelectionChangedEventArgs args)
        {
            if(args.IsSettingsSelected)
            {
                ContentFrame.Navigate(typeof(SettingsPage));
            }
            else if (args.SelectedItem is NavigationViewItem item)
            {
                switch (item.Tag)
                {
                    case "home":
                        ContentFrame.Navigate(typeof(HomePage));
                        break;
                    case "analytics":
                        ContentFrame.Navigate(typeof(AnalyticsPage));
                        break;
                    case "plan":
                        ContentFrame.Navigate(typeof(PlanPage));
                        break;
                }
            }
            
        }

        private void HamburgerButton_Click(object obj, RoutedEventArgs args)
        {
            
        }

        private void OnDropboxClick(object obj, RoutedEventArgs args)
        {

        }

        
    }
}
