﻿using System.Collections.ObjectModel;
using System.Windows.Input;
using Acr.UserDialogs;
using HomeBudget.Code;
using HomeBudget.Code.Interfaces;
using HomeBudget.Code.Logic;
using HomeBudget.Pages.Utils;
using HomeBudget.Utils;
using HomeBudgetStandard.Views.ViewModels;
using Mopups.Services;

namespace HomeBudgetStandard.Views
{
    public partial class SummaryView : ContentPage
	{
        public ICommand DeleteTransactionCommand { get; set; }

        public ObservableCollection<BaseBudgetSubcat> SelectedCategorySubcats { get; private set; }
        public ICommand GridClicked { get; set; }

        private CalcView _calcView;
        private bool _isAddingExpenseInProgress;
        private SummaryViewModel _viewModel;
        private BudgetSummaryDataViewModel _selectedCategory;
        private BudgetSummaryDataViewModel _lastClickedElem;
        private BudgetPopupManager _popupManager;
        private double _baseHeaderHeight;

        public SummaryView ()
		{
            DeleteTransactionCommand = new Command<TransactionViewModel>(OnDeleteTransaction);
            InitializeComponent();
            _viewModel = new SummaryViewModel();
            BindingContext = _viewModel;
            summaryListView.OnScroll += SummaryList_Scrolled;
            transactionsListView.OnScroll += TransactionsList_Scrolled;
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
            if(!summaryListView.IsVisible)
            {
                return;
            }
            if(_baseHeaderHeight < 0)
            {
                _baseHeaderHeight = header.Height;
            }
            var newHeight = _baseHeaderHeight - summaryListView.ScrollPosition/3;

            newHeight = Math.Max(newHeight, header.MinimumHeightRequest);
            if (newHeight > header.MinimumHeightRequest)
            {
                header.HeightRequest = newHeight;
            }
            else
            {
                header.HeightRequest = header.MinimumHeightRequest;
            }
            _viewModel.HeaderScrollProgress = (newHeight - header.MinimumHeightRequest) / (_baseHeaderHeight - header.MinimumHeightRequest);
            debugScroll.Text = $"{summaryListView.FirstElementVisibiltyPerc}";
        }

        private void TransactionsList_Scrolled(object sender, EventArgs e)
        {
            if(!transactionsListView.IsVisible)
            {
                return;
            }
            if (_baseHeaderHeight < 0)
            {
                _baseHeaderHeight = header.Height;
            }
            var newHeight = _baseHeaderHeight - transactionsListView.ScrollPosition / 3;

            newHeight = Math.Max(newHeight, header.MinimumHeightRequest);
            if (newHeight > header.MinimumHeightRequest)
            {
                header.HeightRequest = newHeight;
            }
            else
            {
                header.HeightRequest = header.MinimumHeightRequest;
            }
            _viewModel.HeaderScrollProgress = 1 - transactionsListView.FirstElementVisibiltyPerc;
            debugScroll.Text = $"{transactionsListView.ScrollPosition}";
        }

        private async void AddExpense(SummaryListSubcat selectedSubcat)
        {
            if(_isAddingExpenseInProgress)
            {
                return;
            }

            var featureSwitch = TinyIoC.TinyIoCContainer.Current.Resolve<IFeatureSwitch>();

            _isAddingExpenseInProgress = true;
            _calcView.Reset();
            _calcView.Subcat = selectedSubcat.Name;
            _calcView.OnSaveValue = (double result, string note, DateTime date) =>
            {
                _viewModel.AddExpenseAsync(result, date, _selectedCategory.CategoryReal, selectedSubcat.Id, note);
                _selectedCategory.RaisePropertyChanged();

                Task.Run(async () => await MainBudget.Instance.Save().ConfigureAwait(false));

                _selectedCategory.Collapse();

                _selectedCategory = null;
                _lastClickedElem = null;
                _isAddingExpenseInProgress = false;
            };

            await MopupService.Instance.PushAsync(_calcView);
            //this.ShowPopup(_calcView);
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
                summaryListView.ScrollTo(element, ScrollToPosition.Start, false);
                element.Expand();
            }
        }

        private void Summary_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            summaryListView.SelectedItem = null;
        }

        private void HideCalcView()
        {
            _isAddingExpenseInProgress = false;
            //Navigation.PopPopupAsync();
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
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (Parent is TabbedPage tabbedPage)
                    {
                        tabbedPage.CurrentPage = tabbedPage.Children[tabbedPage.Children.Count - 1];
                    }
                });
            }
        }

        private void Transaction_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            transactionsListView.SelectedItem = null;
        }

        private void SummaryTabsView_SelectionChanged(object sender, SummaryTabsChangedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                summaryListView.IsVisible = e.SelectedMode == SummaryTabsView.Mode.Budget;
                transactionsListView.IsVisible = e.SelectedMode == SummaryTabsView.Mode.Transactions;
                summaryListView.ScrollToTop?.Invoke();
                transactionsListView.ScrollToTop?.Invoke();
            });
        }

        private async void OnDeleteTransaction(TransactionViewModel transactionViewModel)
        {
            if (await UserDialogs.Instance.ConfirmAsync($"Czy na pewno chcesz usunąć transakcje:\n{transactionViewModel.SubcatName}({transactionViewModel.CategoryName})\n{transactionViewModel.Transaction.Amount.ToString("C")}\n{transactionViewModel.Date.ToShortDateString()}?", "Usuń transakcje", "Usuń", "Anuluj"))
            {
                await _viewModel.RemoveTransactionAsync(transactionViewModel);
                Task.Run(async () => await MainBudget.Instance.Save().ConfigureAwait(false));
            }
        }
    }
}