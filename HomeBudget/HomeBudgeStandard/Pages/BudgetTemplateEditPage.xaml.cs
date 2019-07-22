using HomeBudgeStandard.Views;
using HomeBudget.Code;
using HomeBudget.Converters;
using HomeBudget.Utils;
using Syncfusion.Data;
using Syncfusion.SfDataGrid.XForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeBudgeStandard.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BudgetTemplateEditPage : ContentPage
    {
        public List<BudgetCategoryForEdit> BudgetTemplate { get; private set; }
        public Command<BudgetSubcatEdit> RemoveElementCommand { get; set; }

        public BudgetTemplateEditPage()
        {
            RemoveElementCommand = new Command<BudgetSubcatEdit>( (subcat)=>
            {
                var name = subcat.Name;
                var toRemove = BudgetTemplate.Where(elem => elem.)
                BudgetTemplate.Remove()
            });
            InitializeComponent();
            BindingContext = this;

            BudgetTemplate = MainBudget.Instance.GetCurrentMonthData().GetBudgetTemplateEdit();
            listView.ItemsSource = BudgetTemplate;
        }

        private void OnSaveClicked(object sender, EventArgs e)
        {
            MainBudget.Instance.UpdateBudgetCategories(BudgetTemplate);
        }

        private void RemoveElement(object sender, EventArgs e)
        {

        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            listView.SelectedItem = null;
        }
    }
}