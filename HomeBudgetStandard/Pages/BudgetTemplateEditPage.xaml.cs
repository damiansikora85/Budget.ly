using HomeBudget.Code;

namespace HomeBudgetStandard.Pages
{
    public partial class BudgetTemplateEditPage : ContentPage
    {
        public List<BudgetCategoryForEdit> BudgetTemplate { get; private set; }
        public Command<BudgetCategoryForEdit> AddSubcatCommand { get; set; }

        public BudgetTemplateEditPage()
        {
            AddSubcatCommand = new Command<BudgetCategoryForEdit>(category =>
            {
                var foundCategory = BudgetTemplate.FirstOrDefault(element => category.Id == element.Id);
                if (foundCategory != null)
                {
                    var newElement = new BudgetSubcatEdit { Id = foundCategory.Count, IsNew = true };
                    foundCategory.Add(newElement);

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        listView.ScrollTo(newElement, ScrollToPosition.Center, false);
                        OnPropertyChanged(nameof(BudgetTemplate));
                    });
                }
            });

            BudgetTemplate = MainBudget.Instance.BudgetDescription.GetBudgetTemplateEdit();

            InitializeComponent();
            BindingContext = this;
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            MainBudget.Instance.UpdateBudgetCategories(BudgetTemplate);
            await Navigation.PopAsync();
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            listView.SelectedItem = null;
        }

        private void SubcatEditViewCell_OnRemove(object sender, BudgetSubcatEdit subcat)
        {
            var name = subcat.Name;
            foreach (var category in BudgetTemplate)
            {
                var foundSubcat = category.FirstOrDefault(elem => elem.Id == subcat.Id && elem.Name == subcat.Name);
                if (foundSubcat != null)
                {
                    category.Remove(foundSubcat);
                }
            }
            Device.BeginInvokeOnMainThread(() =>
            {
                OnPropertyChanged(nameof(BudgetTemplate));
            });
        }
    }
}