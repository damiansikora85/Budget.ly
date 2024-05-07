using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using FontAwesome;

namespace HomeBudgeStandard.Pages
{
    public partial class MainPageMaster : ContentPage
    {
        public ListView ListView;

        public MainPageMaster()
        {
            InitializeComponent();

            BindingContext = new MainPageMasterViewModel();
            ListView = MenuItemsListView;
        }

        class MainPageMasterViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<MainPageMenuItem> MenuItems { get; set; }

            public MainPageMasterViewModel()
            {
                MenuItems = new ObservableCollection<MainPageMenuItem>(new[]
                {
                    new MainPageMenuItem { Id = 0, Title = "Budżet", Icon=FontAwesomeIcons.FileInvoiceDollar, TargetType = typeof(MainTabbedPage) },
                    new MainPageMenuItem { Id = 1, Title = "Synchronizacja", Icon=FontAwesomeIcons.Sync, TargetType = typeof(DropboxPage) },
                    new MainPageMenuItem { Id = 2, Title = "Ustawienia", Icon=FontAwesomeIcons.Cog, TargetType = typeof(SettingsPage) },
                    new MainPageMenuItem { Id = 3, Title = "Edycja kategorii", Icon = FontAwesomeIcons.Edit, TargetType = typeof(BudgetTemplateEditPage)},
                    new MainPageMenuItem { Id = 4, Title = "Kontakt", Icon = FontAwesomeIcons.Envelope, OnClick = OpenContact },
                    new MainPageMenuItem { Id = 5, Title = "O Aplikacji", Icon=FontAwesomeIcons.QuestionCircle, TargetType = typeof(AboutPage) }
                });
            }

            private async void OpenContact()
            {
                try
                {
                    var message = new EmailMessage
                    {
                        Subject = "Budget.ly",
                        To = new List<string> { "darktowerlab@gmail.com" },
                    };
                    await Email.ComposeAsync(message);
                }
                catch (FeatureNotSupportedException fbsEx)
                {
                    // Email is not supported on this device
                }
                catch (Exception ex)
                {
                    // Some other exception occurred
                }
            }

            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }
    }

    public class CellTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NormalTemplate { get; set; }
        public DataTemplate BrandsTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return ((MainPageMenuItem)item).UseBrandsIcon ? BrandsTemplate : NormalTemplate;
        }
    }
}