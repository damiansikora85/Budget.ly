namespace HomeBudget.UWP
{
    public sealed partial class MainPage
    {
		public MainPage()
		{
            new Syncfusion.SfChart.XForms.UWP.SfChartRenderer();
            Syncfusion.SfDataGrid.XForms.UWP.SfDataGridRenderer.Init();

            this.InitializeComponent();

            //ApplicationView.PreferredLaunchViewSize = new Size(800, 600);
            //ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;

            LoadApplication(new HomeBudget.App());
		}
	}
}
