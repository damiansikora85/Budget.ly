using System.Collections.ObjectModel;
using System.Diagnostics;
using HomeBudgetStandard.Interfaces;
using HomeBudget.Code;
using HomeBudget.Pages;
using HomeBudget.Utils;
using HomeBudgetStandard.Views.ViewModels;

namespace HomeBudgetStandard.Views
{
    public partial class BudgetRealView : ContentPage, IActiveAware
    {
        public ObservableCollection<BudgetViewModelData> Budget { get; set; }
        public ObservableCollection<BudgetViewModelData> IncomesData { get; private set; }
        public ObservableCollection<BudgetViewModelData> ExpensesData { get; set; }

        private bool _setupDone;
        private BudgetRealViewModel _viewModel;

        public event EventHandler IsActiveChanged;

        bool _isActive;
        public virtual bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                }
            }
        }

        public BudgetRealView()
        {
            Budget = new ObservableCollection<BudgetViewModelData>();
            _viewModel = new BudgetRealViewModel();
            BindingContext = _viewModel;
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            MainBudget.Instance.BudgetDataChanged -= MarkDataChanged;

            if (!IsActive) return;
            try
            {
                if (MainBudget.Instance.IsDataLoaded && !_setupDone)
                {
                    Setup();
                }
                else
                {
                    OnPropertyChanged(nameof(Budget));
                    _viewModel.UpdateCharts();
                    _viewModel.ForceSwitchChart();
                }

                _setupDone = true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        protected override void OnDisappearing()
        {
            MainBudget.Instance.BudgetDataChanged += MarkDataChanged;
        }

        private void MarkDataChanged(bool obj)
        {
            _setupDone = false;
        }

        public async Task Activate()
        {
#if ANDROID
            UserDialogs.Instance.ShowLoading("");
#endif

            var dataTemplate = (DataTemplate)Resources["ContentTemplate"];
            View view = null;
            await Task.Factory.StartNew(() =>
            {
                view = (View)dataTemplate.CreateContent();
            });
            Content = view;

            if (MainBudget.Instance.IsDataLoaded && !_setupDone)
            {
                Setup();
            }
            else
            {
                OnPropertyChanged(nameof(Budget));
                _viewModel.UpdateCharts();
                _viewModel.ForceSwitchChart();
            }

#if ANDROID
            UserDialogs.Instance.HideLoading();
#endif
            _setupDone = true;
        }

        private async void OnDetailsClick(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new BudgetDataGridPage(_viewModel.CurrentMonth));
        }

        private void Setup()
        {
            _viewModel.Setup();
            _viewModel.UpdateCharts();
            //_viewModel.ForceSwitchChart();
        }

        private void SwitchChart(object sender, EventArgs e)
        {
            if (sender is View view && view.Effects.Count == 0)
            {
                _viewModel.ForceSwitchChart();
            }
        }
    }
}