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

        private bool show;
        private bool _setupDone;
        private BudgetSummaryDataViewModel _selectedCategory;
        public System.Windows.Input.ICommand GridClicked { get; set; }

        private CalcView _calcView;

        public SummaryView ()
		{
            GridClicked = new Command(OnGridClicked);
			InitializeComponent ();

            BindingContext = this;
            var cultureInfoPL = new CultureInfo("pl-PL");
            var currentDate = DateTime.Now;
            dateText.Text = currentDate.ToString("dd MMMM yyyy", cultureInfoPL);
            show = true;
            //CalcView.OnCancel += HideCalcView;

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
            HideSideBars();
            base.OnDisappearing();
            MainBudget.Instance.BudgetDataChanged -= BudgetDataChanged;
        }

        public bool OnBackPressed()
        {
            if(categories.TranslationX == 0)
            {
                blocker.FadeTo(0);
                categories.TranslateTo(660, 0, easing: Easing.SpringIn);
                return true;
            }
            else if(subcats.TranslationX == 0)
            {
                blocker.FadeTo(0);
                subcats.TranslateTo(660, 0, easing: Easing.SpringIn);
                return true;
            }
            return false;
        }

        private async void OnGridClicked()
        {
            await HideSideBars();
        }

        private async Task HideSideBars()
        {
            var fadeTask = blocker.FadeTo(0);
            var hideSubcatsTask = subcats.TranslateTo(660, 0, easing: Easing.SpringIn);
            var hideCategoriesTask = categories.TranslateTo(660, 0, easing: Easing.SpringIn);

            await Task.WhenAll(fadeTask, hideSubcatsTask, hideCategoriesTask);
        }

        private async void UpdateSummary()
        {
            var summaryData = await GetBudgetSummaryData();
            Device.BeginInvokeOnMainThread(() =>
            {
                SetupBudgetSummary();

                SummaryListViewItems = summaryData;
                listViewCategories.ItemsSource = summaryData;
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
            return await Task.Factory.StartNew<ObservableCollection<BudgetSummaryDataViewModel>>(() =>
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

                    budgetSummaryCollection.Add(budgetSummaryData);
                }

                return budgetSummaryCollection;
            });
        }

        private async void AddButton_Clicked(object sender, EventArgs e)
        {
            SelectedCategorySubcats.Clear();
            var fadeTask = blocker.FadeTo(0.5);
            var showCategoriesTask = categories.TranslateTo(0, 0, easing: Easing.SpringIn);
            await Task.WhenAll(fadeTask, showCategoriesTask);
        }

        private void Summary_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            summaryList.SelectedItem = null;
        }

        private async void listView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if(listViewCategories.SelectedItem is BudgetSummaryDataViewModel selectedCategory)
            {
                _selectedCategory = selectedCategory;
                foreach (var item in selectedCategory.CategoryReal.subcats)
                    SelectedCategorySubcats.Add(item);

                listViewSubcats.ItemsSource = SelectedCategorySubcats;
                if (_calcView == null)
                {
                    _calcView = new CalcView();
                   // AbsoluteLayout.SetLayoutFlags(_calcView, AbsoluteLayoutFlags.All);
                    //AbsoluteLayout.SetLayoutBounds(_calcView, new Rectangle(0.5,0.5,0.9,0.9));
                    
                    _calcView.OnCancel += HideCalcView;
                }
                _calcView.Category = selectedCategory.CategoryName;
            }
            listViewCategories.SelectedItem = null;
            await categories.TranslateTo(660, 0, easing: Easing.SpringIn);

            await subcats.TranslateTo(0, 0, easing: Easing.SpringIn);
        }

        private async void Subcat_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            await subcats.TranslateTo(660, 0, easing: Easing.SpringIn);
            if (listViewSubcats.SelectedItem is RealSubcat selectedSubcat)
            {
                _calcView.Reset();
                await blocker.FadeTo(0);
                _calcView.Subcat = selectedSubcat.Name;
                _calcView.OnSaveValue = (double calculationResult, DateTime date) =>
                {
                    var budgetMonth = MainBudget.Instance.GetMonth(date);
                    var category = budgetMonth.BudgetReal.GetBudgetCategory(_selectedCategory.CategoryReal.Id);
                    if (category != null)
                    {
                        var subcat = category.GetSubcat(selectedSubcat.Id);
                        if(subcat is RealSubcat realSubcat)
                            realSubcat.AddValue(calculationResult, date);
                    }
                    Task.Run(async () =>
                    {
                        await MainBudget.Instance.Save();
                    });

                    SetupBudgetSummary();
                    listViewSubcats.SelectedItem = null;
                    HideCalcView();
                    _selectedCategory.RaisePropertyChanged();
                    _selectedCategory = null;
                    Navigation.PopPopupAsync();
                };

                await Navigation.PushPopupAsync(_calcView);
            }
        }

        private void HideCalcView()
        {
            Navigation.PopPopupAsync();
        }
    }
}