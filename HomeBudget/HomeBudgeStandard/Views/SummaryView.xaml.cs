using Acr.UserDialogs;
using HomeBudget.Code;
using HomeBudget.Code.Logic;
using HomeBudget.Pages.Utils;
using Microsoft.AppCenter.Crashes;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeBudgeStandard.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SummaryView : ContentPage
	{
        public double ExpectedIncomes { get; set; }
        public double ExpectedExpenses { get; set; }
        public double RealIncomes { get; set; }
        public double RealExpenses { get; set; }
        public double DiffReal { get; set; }
        public double DiffExpected { get; set; }

        public ObservableCollection<BudgetSummaryDataViewModel> SummaryListViewItems { get; set; }
        public ObservableCollection<BaseBudgetSubcat> SelectedCategorySubcats { get; private set; }
        public Command ExpandCategoryCommand;

        private bool _setupDone;
        private BudgetSummaryDataViewModel _selectedCategory;
        private BudgetSummaryDataViewModel _lastClickedElem;

        public System.Windows.Input.ICommand GridClicked { get; set; }

        private CalcView _calcView;

        public SummaryView ()
		{
            MessagingCenter.Subscribe<SummaryGroupHeaderViewCell, BudgetSummaryDataViewModel>(this, "CategoryClicked", (sender, element) => ExpandCategory(element));
            MessagingCenter.Subscribe<AnimatedViewCell, SummaryListSubcat>(this, "SubcatClicked", (sender, subcat) => AddExpense(subcat));

            InitializeComponent();

            BindingContext = this;
            var cultureInfoPL = new CultureInfo("pl-PL");
            var currentDate = DateTime.Now;

            MainBudget.Instance.BudgetDataChanged += BudgetDataChanged;

            SelectedCategorySubcats = new ObservableCollection<BaseBudgetSubcat>();
        }

        private void BudgetDataChanged(bool isLoadedFromCloud)
        {
            UpdateSummary();
        }

        protected override void OnAppearing()
        {
            if (MainBudget.Instance.IsDataLoaded && !_setupDone)
                UpdateSummary();
            else if (SummaryListViewItems == null)
            {
                loader.IsRunning = true;
                UserDialogs.Instance.ShowLoading("");
            }
            else
            {
                foreach (var summaryItem in SummaryListViewItems)
                    summaryItem.RaisePropertyChanged();

                SetupBudgetSummary();
            }

            _setupDone = true;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MainBudget.Instance.BudgetDataChanged -= BudgetDataChanged;
        }

        private async void UpdateSummary()
        {
            var summaryData = await GetBudgetSummaryData();
            Device.BeginInvokeOnMainThread(() =>
            {
                SetupBudgetSummary();

                SummaryListViewItems = summaryData;
                summaryList.ItemsSource = summaryData;

                loader.IsRunning = false;
                UserDialogs.Instance.HideLoading();
            });
        }

        private void SetupBudgetSummary()
        {
            var budgetMonth = MainBudget.Instance.GetCurrentMonthData();
            RealExpenses = budgetMonth.GetTotalExpenseReal();
            RealIncomes = budgetMonth.GetTotalIncomeReal();
            DiffReal = RealIncomes - RealExpenses;

            ExpectedExpenses = budgetMonth.GetTotalExpensesPlanned();
            ExpectedIncomes = budgetMonth.GetTotalIncomePlanned();
            DiffExpected = ExpectedIncomes - ExpectedExpenses;

            OnPropertyChanged("");
        }

        private async Task<ObservableCollection<BudgetSummaryDataViewModel>> GetBudgetSummaryData()
        {
            return await Task.Factory.StartNew(() =>
            {
                var budgetSummaryCollection = new ObservableCollection<BudgetSummaryDataViewModel>();
                var budgetReal = MainBudget.Instance.GetCurrentMonthData().BudgetReal;
                var categoriesDesc = MainBudget.Instance.BudgetDescription.Categories;
                var budgetPlanned = MainBudget.Instance.GetCurrentMonthData().BudgetPlanned;
                for (int i = 0; i < budgetReal.Categories.Count; i++)
                {
                    var budgetSummaryData = new BudgetSummaryDataViewModel
                    {
                        CategoryReal = budgetReal.Categories[i],
                        CategoryPlanned = budgetPlanned.Categories[i],
                        IconFile = categoriesDesc[i].IconFileName
                    };

                    budgetSummaryData.Init();
                    budgetSummaryCollection.Add(budgetSummaryData);
                }

                return budgetSummaryCollection;
            });
        }

        private async void AddExpense(SummaryListSubcat selectedSubcat)
        {
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

                SetupBudgetSummary();

                HideCalcView();
                _selectedCategory.RaisePropertyChanged();
                _selectedCategory = null;
                _lastClickedElem = null;
                Navigation.PopPopupAsync();
            };
            _selectedCategory.Collapse();

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
    }
}