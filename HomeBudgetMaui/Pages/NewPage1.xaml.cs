using HomeBudget.Code;
using HomeBudget.Code.Interfaces;
using HomeBudgetShared.Code.Synchronize;
using HomeBudgetStandard.Interfaces.Impl;
using HomeBudgetStandard.Pages;
using HomeBudgetStandard.Utils;

namespace HomeBudgetMaui.Pages;

public partial class NewPage1 : FlyoutPage
{
    private readonly ISettings _settings;

    public NewPage1()
	{
		InitializeComponent();
	}

    public bool OnBackPressed()
    {
        if (Detail is NavigationPage navigationPage && navigationPage.RootPage is MainTabbedPage mainTabbedPage)
        {
            return mainTabbedPage.OnBackPressed();
        }
        else
        {
            return false;
        }
    }
    
    private async Task InitBudget()
    {
        try
        {
            var crashReporter = new XamarinCrashReporter();
            var featureSwitch = TinyIoC.TinyIoCContainer.Current.Resolve<IFeatureSwitch>();

            await MainBudget.Instance.Init(new FileManagerXamarin(), new BudgetSynchronizer(new DropboxCloudStorage(crashReporter, _settings)), crashReporter, _settings, featureSwitch);
            if (_settings.FirstLaunch || _settings.NeedSetupDefaultNotifications)
            {
                NotificationManager.ScheduleDefaultNotifications();
                _settings.FirstLaunch = false;
                _settings.NeedSetupDefaultNotifications = false;
            }
        }
        catch (Exception exc)
        {
            var msg = exc.Message;
        }
    }
    /*
    public void AfterCloudLogin()
    {
        Device.BeginInvokeOnMainThread(() =>
        {
            var mainTabbedPage = new MainTabbedPage();
            mainTabbedPage.SetSettings(_settings);
            Detail = new NavigationPage(mainTabbedPage);
            IsPresented = false;
        });
    }

    private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (!(e.SelectedItem is MainPageMenuItem item))
            return;

        try
        {
            if (item.OnClick != null)
            {
                item.OnClick.Invoke();
            }
            else if (item.TargetType != typeof(MainTabbedPage))
            {
                Page page;
                if (item.TargetType == typeof(DropboxPage))
                {
                    page = (Page)Activator.CreateInstance(item.TargetType, new object[] { _settings });
                }
                else
                {
                    page = (Page)Activator.CreateInstance(item.TargetType);
                }
                Detail.Navigation.PushAsync(page);
            }
            IsPresented = false;
        }
        catch (Exception exc)
        {
            var msg = exc.Message;
        }
        MasterPage.ListView.SelectedItem = null;
    }*/

    protected async override void OnAppearing()
    {
        //if (Detail is NavigationPage navigationPage && navigationPage.CurrentPage is MainTabbedPage mainTabbedPage)
        //{
        //    mainTabbedPage.SetSettings(_settings);
        //}
        //await Crashes.SetEnabledAsync(true);
        //var didAppCrash = await Crashes.HasCrashedInLastSessionAsync();
    }

}