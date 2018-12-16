using FontAwesome;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeBudgeStandard.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
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
                    new MainPageMenuItem { Id = 3, Title = "O Aplikacji", Icon=FontAwesomeIcons.QuestionCircle, TargetType = typeof(AboutPage) }
                });
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