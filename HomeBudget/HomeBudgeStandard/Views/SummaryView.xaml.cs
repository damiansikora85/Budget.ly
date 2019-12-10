using Acr.UserDialogs;
using HomeBudgeStandard.Pages;
using HomeBudget.Code;
using HomeBudget.Code.Logic;
using HomeBudget.Pages.Utils;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeBudgeStandard.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SummaryView : ContentPage
	{

        public ObservableCollection<BaseBudgetSubcat> SelectedCategorySubcats { get; private set; }
        public Command ExpandCategoryCommand;

        //private bool _setupDone;
        private BudgetSummaryDataViewModel _selectedCategory;
        private BudgetSummaryDataViewModel _lastClickedElem;

        private bool _isPopupDisplaying;

        public System.Windows.Input.ICommand GridClicked { get; set; }

        private CalcView _calcView;
        private bool _isAddingExpenseInProgress;
        private SummaryViewModel _viewModel;

        public SummaryView ()
		{
            InitializeComponent();
            _viewModel = new SummaryViewModel();
            BindingContext = _viewModel;
            summaryList.Scrolled += SummaryList_Scrolled;

            SelectedCategorySubcats = new ObservableCollection<BaseBudgetSubcat>();
        }

        protected override async void OnAppearing()
        {
            await _viewModel.ViewWillAppear().ConfigureAwait(false);
            MessagingCenter.Subscribe<SummaryGroupHeaderViewCell, BudgetSummaryDataViewModel>(this, "CategoryClicked", (sender, element) => ExpandCategory(element));
            MessagingCenter.Subscribe<AnimatedViewCell, SummaryListSubcat>(this, "SubcatClicked", (sender, subcat) => AddExpense(subcat));

            _isPopupDisplaying = false;
            if (MainBudget.Instance.IsDataLoaded)
            {
                TryFirstLaunchInfo();
                if (!_isPopupDisplaying)
                {
                    TryNewFeatureInfo();
                    TryShowRatePopup();
                }
            }
            //else if (SummaryListViewItems == null)
            //{
            //    loader.IsRunning = true;
            //    UserDialogs.Instance.ShowLoading("");
            //}
           // else
            {
                //foreach (var summaryItem in SummaryListViewItems)
                //{
                //    summaryItem.RaisePropertyChanged();
                //}

                //await _viewModel.RefreshAsync().ConfigureAwait(false);
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _viewModel.ViewWillDisapear();

            MessagingCenter.Unsubscribe<SummaryGroupHeaderViewCell, BudgetSummaryDataViewModel>(this, "CategoryClicked");
            MessagingCenter.Unsubscribe<AnimatedViewCell, SummaryListSubcat>(this, "SubcatClicked");
        }

        private void TryShowRatePopup()
        {
            _isPopupDisplaying = true;
            var lastRatePopupDisplayedDate = Xamarin.Essentials.Preferences.Get("ratePopupDisplayDate", DateTime.MinValue);
            if ((DateTime.Now - lastRatePopupDisplayedDate).TotalDays >= 5 && Xamarin.Essentials.Preferences.Get("shouldShowRatePopup", true))
            {
                Navigation.PushPopupAsync(new RatePage());
                Xamarin.Essentials.Preferences.Set("ratePopupDisplayDate", DateTime.Now);
            }
        }

        private void TryNewFeatureInfo()
        {
            if (Xamarin.Essentials.Preferences.Get("categoryEdit", true))
            {
                Xamarin.Essentials.Preferences.Set("categoryEdit", false);

                Navigation.PushPopupAsync(new NewFeaturePopup("Edycja kategorii", "Zarządzaj swoimi wydatkami i dochodami tak jak chcesz. Teraz możesz dostosować szablon kategorii do Twoich potrzeb. Stwórz prawdziwy budżet osobisty!",
                async () =>
                {
                    if (Parent is MainTabbedPage tabbedPage)
                    {
                        await tabbedPage.Navigation.PushAsync(new BudgetTemplateEditPage());
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
                Navigation.PushPopupAsync(new WelcomePopup());
            }
        }

        private double _currentScrollPos;
        private void SummaryList_Scrolled(object sender, ScrolledEventArgs e)
        {
            var newHeight = header.HeightRequest - (e.ScrollY - _currentScrollPos);

            newHeight = Math.Max(newHeight, header.MinimumHeightRequest);
            if (newHeight > header.MinimumHeightRequest)
            {
                header.HeightRequest = newHeight;
                _currentScrollPos = e.ScrollY;
            }
            else
            {
                _currentScrollPos += (header.HeightRequest - header.MinimumHeightRequest);
                header.HeightRequest = header.MinimumHeightRequest;
            }
            _viewModel.ScrollProgress = _currentScrollPos;
            debugScroll.Text = _currentScrollPos.ToString();
        }

        private async void AddExpense(SummaryListSubcat selectedSubcat)
        {
            if(_isAddingExpenseInProgress)
            {
                return;
            }

            _isAddingExpenseInProgress = true;
            _calcView.Reset();
            _calcView.Subcat = selectedSubcat.Name;
            _calcView.OnSaveValue = (double calculationResult, DateTime date) =>
            {
                var budgetMonth = MainBudget.Instance.GetMonth(date);
                var category = budgetMonth.BudgetReal.GetBudgetCategory(_selectedCategory.CategoryReal.Id);
                if (category != null)
                {
                    var subcat = category.GetSubcat(selectedSubcat.Id);
                    if (subcat is RealSubcat realSubcat)
                        realSubcat.AddValue(calculationResult, date);
                }
                Task.Run(async () =>
                {
                    await MainBudget.Instance.Save();
                });

                _viewModel.RefreshAsync();

                HideCalcView();
                summaryList.ScrollTo(selectedSubcat, _selectedCategory, ScrollToPosition.Center, false);
                _selectedCategory.Collapse();
                _selectedCategory.RaisePropertyChanged();
                _selectedCategory = null;
                _lastClickedElem = null;
                _isAddingExpenseInProgress = false;
                Navigation.PopPopupAsync();
            };

            await Navigation.PushPopupAsync(_calcView);
        }

        private void ExpandCategory(BudgetSummaryDataViewModel element)
        {
            if (_lastClickedElem != null && _lastClickedElem.IsExpanding) return;

            if (element != _lastClickedElem)
            {
                if (_lastClickedElem != null)
                    _lastClickedElem.Collapse();

                element.Expand();
                //summaryList.ScrollTo(element[0], element, ScrollToPosition.MakeVisible, false);

                _lastClickedElem = element;

                if (_calcView == null)
                {
                    _calcView = new CalcView();
                    _calcView.OnCancel += HideCalcView;
                }
                _calcView.Category = element.CategoryName;
                _selectedCategory = element;
            }
            else if (element.IsExpanded)
                element.Collapse();
            else
            {
                summaryList.ScrollTo(element, ScrollToPosition.Start, false);
                element.Expand();
            }
        }

        private void Summary_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            summaryList.SelectedItem = null;
        }

        private void HideCalcView()
        {
            Navigation.PopPopupAsync();
        }

        private void OnPrevMonth(object sender, EventArgs e)
        {
            _viewModel.DecreaseMonth();
        }

        private void OnNextMonth(object sender, EventArgs e)
        {
            _viewModel.IncreaseMonth();
        }

        private async void OnNoPlanClick(object sender, EventArgs e)
        {
            if(await UserDialogs.Instance.ConfirmAsync("Ułóż swój plan wydatków i zarobków - kontroluj swoje finanse", "Planowanie budżetu", "Planuj teraz", "Może później"))
            {
                if(Parent is TabbedPage tabbedPage)
                {
                    tabbedPage.CurrentPage = tabbedPage.Children[tabbedPage.Children.Count - 1];
                }
            }
        }
    }
}