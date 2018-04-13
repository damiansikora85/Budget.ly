using HomeBudget.Code;
using HomeBudget.UWP.Pages;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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

            InitializeComponent();

            //ApplicationView.PreferredLaunchViewSize = new Size(800, 600);
            //ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;

            //LoadApplication(new HomeBudget.App());

            ContentFrame.Content = new ProgressRing { IsActive = true };


            if (!MainBudget.Instance.IsInitialized)
                InitBudget();
            else
                OnBudgetLoaded();
        }

        private void InitBudget()
        {
            MainBudget.Instance.Init();
            MainBudget.Instance.onBudgetLoaded += OnBudgetLoaded;

            if (!string.IsNullOrEmpty(Helpers.Settings.DropboxAccessToken))
            {
                Task.Run(() => DropboxManager.Instance.DownloadData());
            }
            else
            {
                Task.Run(() => MainBudget.Instance.Load());
            }
        }

        private void OnBudgetLoaded()
        {
            Task.Run(async () =>
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => 
                {
                    foreach (NavigationViewItemBase item in NavView.MenuItems)
                    {
                        if (item is NavigationViewItem && item.Tag.ToString() == "home")
                        {
                            NavView.SelectedItem = item;
                            break;
                        }
                    }
                }));
        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {

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
