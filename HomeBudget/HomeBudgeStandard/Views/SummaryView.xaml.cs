using Acr.UserDialogs;
using HomeBudgeStandard.Pages;
using HomeBudgeStandard.Utils;
using HomeBudget.Code;
using HomeBudget.Code.Logic;
using HomeBudget.Pages.Utils;
using HomeBudget.Utils;
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
        public System.Windows.Input.ICommand GridClicked { get; set; }

        private CalcView _calcView;
        private bool _isAddingExpenseInProgress;
        private SummaryViewModel _viewModel;
        private double _currentScrollPos;
        private BudgetSummaryDataViewModel _selectedCategory;
        private BudgetSummaryDataViewModel _lastClickedElem;
        private BudgetPopupManager _popupManager;
        private double _baseHeaderHeight;

        public SummaryView ()
		{
            InitializeComponent();
            _viewModel = new SummaryViewModel();
            BindingContext = _viewModel;
            summaryList.OnScroll += SummaryList_Scrolled;
            _baseHeaderHeight = -1;

            _popupManager = new BudgetPopupManager(Parent as Page, Navigation);
            SelectedCategorySubcats = new ObservableCollection<BaseBudgetSubcat>();
        }

        protected override async void OnAppearing()
        {
            await _viewModel.ViewWillAppear().ConfigureAwait(false);
            MainBudget.Instance.BudgetDataChanged += BudgetDataChanged;
            MessagingCenter.Subscribe<SummaryGroupHeaderViewCell, BudgetSummaryDataViewModel>(this, "CategoryClicked", (sender, element) => ExpandCategory(element));
            MessagingCenter.Subscribe<AnimatedViewCell, SummaryListSubcat>(this, "SubcatClicked", (sender, subcat) => AddExpense(subcat));

            if (MainBudget.Instance.IsDataLoaded)
            {
                _popupManager.TryDisplayPopup();
            }
        }

        private void BudgetDataChanged(bool obj)
        {
            _popupManager.TryDisplayPopup();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _viewModel.ViewWillDisapear();

            MessagingCenter.Unsubscribe<SummaryGroupHeaderViewCell, BudgetSummaryDataViewModel>(this, "CategoryClicked");
            MessagingCenter.Unsubscribe<AnimatedViewCell, SummaryListSubcat>(this, "SubcatClicked");
        }

        private void SummaryList_Scrolled(object sender, EventArgs e)
        {
            if(_baseHeaderHeight < 0)
            {
                _baseHeaderHeight = header.Height;
            }
            var newHeight = _baseHeaderHeight - summaryList.ScrollPosition/3;

            newHeight = Math.Max(newHeight, header.MinimumHeightRequest);
            if (newHeight > header.MinimumHeightRequest)
            {
                header.HeightRequest = newHeight;
            }
            else
            {
                header.HeightRequest = header.MinimumHeightRequest;
            }
            _viewModel.ScrollProgress = 1-summaryList.FirstElementVisibiltyPerc;
            debugScroll.Text = $"{summaryList.ScrollPosition} {summaryList.Test}";
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
            _calcView.OnSaveValue = (double result, DateTime date) =>
            {
                _viewModel.AddExpenseAsync(result, date, _selectedCategory.CategoryReal, selectedSubcat.Id);

                Task.Run(async () => await MainBudget.Instance.Save().ConfigureAwait(false));

                _selectedCategory.Collapse();

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
                {
                    _lastClickedElem.Collapse();
                }

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
            if(await UserDialogs.Instance.ConfirmAsync("Ułóż swój plan wydatków i zarobków - kontroluj swoje finanse", "Planowanie budżetu", "Planuj teraz", "Może później").ConfigureAwait(false))
            {
                if(Parent is TabbedPage tabbedPage)
                {
                    tabbedPage.CurrentPage = tabbedPage.Children[tabbedPage.Children.Count - 1];
                }
            }
        }
    }
}