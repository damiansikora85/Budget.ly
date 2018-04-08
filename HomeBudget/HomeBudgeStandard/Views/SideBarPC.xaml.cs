using HomeBudget.Pages.PC;
using System;

using Xamarin.Forms;

namespace HomeBudget.Views
{
    //[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SideBarPC : ContentView
	{
        public enum EMode
        {
            Home,
            Analize,
            Planning
        }
        private EMode currentMode;

		public SideBarPC ()
		{
			InitializeComponent ();
            currentMode = EMode.Home;
		}

        

        private async void OnHomeClick(object sender, EventArgs e)
        {
            //App.Current.s
            //await Navigation.PushModalAsync(new MainPagePC());
        }

        private async void OnPlanClick(object sender, EventArgs args)
        {
            //Application.Current.MainPage = PlanningPage;
            //await Navigation.PushModalAsync(new PlanningPage());
        }

        private async void OnAnalyticsClick(object sender, EventArgs e)
        {
            //Application.Current.MainPage = AnalyticsPage;
            //await Navigation.PushModalAsync(new AnalyticsPagePC());
        }

        public void SetMode(EMode newMode)
        {
            currentMode = newMode;
            switch(currentMode)
            {
                case EMode.Home:
                    SetupIcon(homeIcon, "Assets/home.png", 0, homeButton, false);
                    SetupIcon(analizeIcon, "Assets/analyze_inactive.png", 5, analiticsButton, true);
                    SetupIcon(planIcon, "Assets/planning_inactive.png", 5, planButton, true);
                    break;
                case EMode.Analize:
                    SetupIcon(homeIcon, "Assets/home_inactive.png", 5, homeButton, true);
                    SetupIcon(analizeIcon, "Assets/analyze_active.png", 0, analiticsButton, false);
                    SetupIcon(planIcon, "Assets/planning_inactive.png", 5, planButton, true);
                    break;
                case EMode.Planning:
                    SetupIcon(homeIcon, "Assets/home_inactive.png", 5, homeButton, true);
                    SetupIcon(analizeIcon, "Assets/analyze_inactive.png", 5, analiticsButton, true);
                    SetupIcon(planIcon, "Assets/planning_active.png", 0, planButton, false);
                    break;
            }
        }

        private void SetupIcon(Image icon, string imageName, int margin, Button button, bool isEnabled)
        {
            icon.Source = imageName;
            icon.Margin = new Thickness(margin);
            button.IsVisible = isEnabled;
        }
    }
}