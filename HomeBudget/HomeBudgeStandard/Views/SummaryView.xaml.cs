using Acr.UserDialogs;
using HomeBudget.Code;
using HomeBudget.Code.Logic;
using HomeBudget.Pages.Utils;
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

        public SummaryView ()
		{
			InitializeComponent ();

            BindingContext = this;
            var cultureInfoPL = new CultureInfo("pl-PL");
            var currentDate = DateTime.Now;
            dateText.Text = currentDate.ToString("dd MMMM yyyy", cultureInfoPL);
            show = true;
            CalcView.OnCancel += HideCalcView;

            SelectedCategorySubcats = new ObservableCollection<BaseBudgetSubcat>();
            
            MainBudget.Instance.onBudgetLoaded += UpdateSummary;
        }

        protected override void OnAppearing()
        {
            if (MainBudget.Instance.IsInitialized && !_setupDone)
                UpdateSummary();
            else if(SummaryListViewItems == null)
                UserDialogs.Instance.ShowLoading();

            _setupDone = true;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MainBudget.Instance.onBudgetLoaded -= UpdateSummary;
        }

        private void UpdateSummary()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                SetupBudgetSummary();

                SummaryListViewItems = GetBudgetSummaryData();
                listViewCategories.ItemsSource = SummaryListViewItems;
                summaryList.ItemsSource = SummaryListViewItems;

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

            OnPropertyChanged(nameof(RealExpenses));
            OnPropertyChanged(nameof(RealIncomes));
            OnPropertyChanged(nameof(DiffReal));

            OnPropertyChanged(nameof(ExpectedExpenses));
            OnPropertyChanged(nameof(ExpectedIncomes));
            OnPropertyChanged(nameof(DiffExpected));
        }

        private ObservableCollection<BudgetSummaryDataViewModel> GetBudgetSummaryData()
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
        }

        private void AddButton_Clicked(object sender, EventArgs e)
        {
            SelectedCategorySubcats.Clear();
            categories.TranslateTo(0, 0, easing: Easing.SpringIn);
        }

        private void Summary_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            summaryList.SelectedItem = null;
        }

        private async void listView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if(listViewCategories.SelectedItem is BudgetSummaryDataViewModel selectedCategory)
            {
                foreach (var item in selectedCategory.CategoryReal.subcats)
                    SelectedCategorySubcats.Add(item);

                listViewSubcats.ItemsSource = SelectedCategorySubcats;
                CalcView.Category = selectedCategory.CategoryName;
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
                CalcLayout.IsVisible = true;
                CalcView.Reset();
                CalcView.Subcat = selectedSubcat.Name;
                CalcView.OnSaveValue = (double calculationResult, DateTime date) =>
                {
                    selectedSubcat.AddValue(calculationResult, date);

                    Task.Run(async () =>
                    {
                        await MainBudget.Instance.Save();
                    });

                    SetupBudgetSummary();
                    listViewSubcats.SelectedItem = null;
                    HideCalcView();
                };
            }
        }

        private void HideCalcView()
        {
            CalcLayout.IsVisible = false;
        }
    }
}