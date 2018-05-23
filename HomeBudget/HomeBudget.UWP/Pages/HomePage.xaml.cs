using HomeBudget.Code;
using HomeBudget.Code.Logic;
using HomeBudget.Pages.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace HomeBudget.UWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private CultureInfo _cultureInfoPL = new CultureInfo("pl-PL");

        public string Today
        {
            get => DateTime.Now.ToString("dd MMMM yyyy", _cultureInfoPL);
        }

        public double ExpectedIncomes { get; private set; }
        public double ExpectedExpenses { get; private set; }
        public double RealIncomes { get; set; }
        public double RealExpenses { get; set; }
        public double DiffReal { get; set; }
        public double DiffExpected { get; set; }
        public ObservableCollection<BudgetSummaryDataViewModel> SummaryListViewItems { get; private set; }
        public ObservableCollection<BaseBudgetSubcat> SelectedCategorySubcats { get; private set; }

        public HomePage()
        {
            InitializeComponent();
            DataContext = this;

            var flyout = FlyoutBase.GetAttachedFlyout(MainPanel);

            flyout.Closed += ShowSubcats;

            SummaryListViewItems = GetBudgetSummaryData();
            SelectedCategorySubcats = new ObservableCollection<BaseBudgetSubcat>();

            SetupBudgetSummary();   
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

            Bindings.Update();
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
                    IconFile = "Assets/Categories/" + categoriesDesc[i].IconFileName
                };

                budgetSummaryCollection.Add(budgetSummaryData);
            }

            return budgetSummaryCollection;
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedCategorySubcats.Clear();
            FlyoutBase.ShowAttachedFlyout(MainPanel);
        }

        private void OnFlyoutCategoryClicked(object sender, SelectionChangedEventArgs e)
        {
            var flyout = FlyoutBase.GetAttachedFlyout(MainPanel);
            var listView = (ListView)sender;
            var selectedCategory = (BudgetSummaryDataViewModel)listView.SelectedItem;
            
            foreach(var item in selectedCategory.CategoryReal.subcats)
                SelectedCategorySubcats.Add(item);

            flyout.Hide();
        }

        private void OnFlyoutSubcatClicked(object sender, SelectionChangedEventArgs e)
        {
            var flyout = FlyoutBase.GetAttachedFlyout(MainGrid);
            flyout.Hide();
             
            var listView = (ListView)sender;
            if (listView.SelectedItem == null) return;

            
            var selectedSubcat = (RealSubcat)listView.SelectedItem;
            var calc = new CalcDialog(selectedSubcat.Name);

            calc.OnSaveValue = (double calculationResult, DateTime date) =>
            {
                selectedSubcat.AddValue(calculationResult, date);

                Task.Run(async () =>
                {
                    await MainBudget.Instance.Save(); 
                });

                SetupBudgetSummary();
                calc.Hide();
            };
            
            calc.ShowAsync();
        }

        private void ShowSubcats(object sender, object e)
        {
            if(SelectedCategorySubcats.Count() > 0)
                FlyoutBase.ShowAttachedFlyout(MainGrid);
        }
    }
}
