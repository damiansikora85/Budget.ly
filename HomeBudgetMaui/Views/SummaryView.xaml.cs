using System.Collections.ObjectModel;
using System.Windows.Input;
using Acr.UserDialogs;
using CommunityToolkit.Mvvm.Messaging;
using HomeBudget.Code;
using HomeBudget.Code.Interfaces;
using HomeBudget.Code.Logic;
using HomeBudget.Pages.Utils;
using HomeBudgetMaui.Messages;
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
        public SummaryView ()
		{
            DeleteTransactionCommand = new Command<TransactionViewModel>(OnDeleteTransaction);
            InitializeComponent();
            _viewModel = new SummaryViewModel();
            BindingContext = _viewModel;
            SelectedCategorySubcats = new ObservableCollection<BaseBudgetSubcat>();

            WeakReferenceMessenger.Default.Register<CategoryClickedMessage>(this, (r, m) =>
            {
                //summaryListView.ScrollTo(m.Element, position: ScrollToPosition.Center, animate: true);
                ExpandCategory(m.Element);
            });

            WeakReferenceMessenger.Default.Register<SubcatClickedMessage>(this, (r, m) =>
            {
                AddExpense(m.Subcat);
            });
        }

        protected override void OnAppearing()
        {
            _viewModel.ViewWillAppear();
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _viewModel.ViewWillDisapear();
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

            await MopupService.Instance.PushAsync(_calcView);        }

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
            {
                element.Collapse();
            }
            else
            {
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
            //transactionsListView.ScrollTo(1, position: ScrollToPosition.End, animate: false);
        }

        private void SummaryTabsView_SelectionChanged(object sender, SummaryTabsChangedEventArgs e)
        {
            summaryListView.IsVisible = e.SelectedMode == SummaryTabsView.Mode.Budget;
            transactionsListView.IsVisible = e.SelectedMode == SummaryTabsView.Mode.Transactions;

            if (summaryListView.IsVisible)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    //summaryListView.Scrolled -= OnListScrolled;
                    summaryListView.ScrollTo(summaryListView.Header, position: ScrollToPosition.Start, animate: false);
                    //summaryListView.Scrolled += OnListScrolled;
                    OnListScrolled(summaryListView, new ItemsViewScrolledEventArgs
                    {
                        VerticalDelta = 0,
                        VerticalOffset = 0
                    });
                });
            }
#if ANDROID
            if (summaryListView.IsVisible)
            {
                var view = summaryListView.Handler?.PlatformView as AndroidX.RecyclerView.Widget.RecyclerView;
                view.ScrollToPosition(0);
                //summaryListView.ScrollTo(summaryListView.Header, position: ScrollToPosition.Start, animate: false);
                OnListScrolled(summaryListView, new ItemsViewScrolledEventArgs
                {
                    VerticalDelta = 0,
                    VerticalOffset = 0
                });
            }
#endif

            //if(summaryListView.IsVisible)
            //{
            //    _viewModel.ReloadBudgetData();
            //}

            //header.TranslationY = 0;
            //SummaryText.TranslationY = 0;
            //_viewModel.HeaderScrollProgress = 1;
        }

        private async void OnDeleteTransaction(TransactionViewModel transactionViewModel)
        {
            if (await UserDialogs.Instance.ConfirmAsync($"Czy na pewno chcesz usunąć transakcje:\n{transactionViewModel.SubcatName}({transactionViewModel.CategoryName})\n{transactionViewModel.Transaction.Amount.ToString("C")}\n{transactionViewModel.Date.ToShortDateString()}?", "Usuń transakcje", "Usuń", "Anuluj"))
            {
                _viewModel.RemoveTransactionAsync(transactionViewModel);
                Task.Run(async () => await MainBudget.Instance.Save().ConfigureAwait(false));
            }
        }

        private void OnListScrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            var transY = Convert.ToSingle(e.VerticalOffset);

            if (transY < 0)
            {
                transY = 0;
            }

            var headerTranslation = Math.Max(-transY, -100);
            header.TranslationY = headerTranslation;
            SummaryText.TranslationY = -headerTranslation/2;

            _viewModel.HeaderScrollProgress = 1.0 - Math.Min(Math.Abs(headerTranslation) / 70.0, 1.0);
            debugScroll.Text = $"Offset: {e.VerticalOffset}, HeaderTransY: {headerTranslation}";
        }
    }
}