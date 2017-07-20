using Syncfusion.SfDataGrid.XForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeBudget.Pages.PC
{
    public class CustomStyle : DataGridStyle
    {
        public CustomStyle()
        {
        }
        public override GridLinesVisibility GetGridLinesVisibility()
        {
            return GridLinesVisibility.None;
        }
    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlanningPage : ContentPage
    {
        private int selectedCategory;
        private int selectedSubcat;
        private PlanningPageViewModel viewModel;

        public PlanningPage()
        {
            InitializeComponent();
            viewModel = new PlanningPageViewModel();
            BindingContext = viewModel;
            Calculator.IsVisible = false;

            
            listView.ItemsSource = Code.MainBudget.Instance.GetPlanningData();
            listView.GridStyle = new CustomStyle();
        }

        private async void OnHomeClick(object sender, EventArgs args)
        {
            NavigationPage mainPage = new NavigationPage(new MainPagePC());
            await Navigation.PushModalAsync(mainPage);
        }

        private async void OnAnalizeClick(object sender, EventArgs e)
        {
            NavigationPage analizePage = new NavigationPage(new AnalyticsPagePC());
            await Navigation.PushModalAsync(analizePage);
        }

        private async void OnElementClick(object sender, EventArgs e)
        {
            Calculator.IsVisible = true;
        }

        private async void OnOk(object sender, EventArgs e)
        {
            Calculator.IsVisible = false;
            bool isIncome = selectedCategory == Code.MainBudget.INCOME_CATEGORY_ID;
            if (isIncome)
                await Code.MainBudget.Instance.SetPlanedIncome(float.Parse(viewModel.CalculationText), selectedSubcat);
            else
                await Code.MainBudget.Instance.AddPlanedExpense(float.Parse(viewModel.CalculationText), selectedCategory, selectedSubcat);
        }

        private async void OnCancel(object sender, EventArgs e)
        {
            Calculator.IsVisible = false;
        }

        void OnElementSelected(object sender, ItemTappedEventArgs e)
        {
            Calculator.IsVisible = true;

            Code.GroupedCategory group = e.Group as Code.GroupedCategory;
            Code.SimpleCategory element = e.Item as Code.SimpleCategory;
            Header.Text = group.Name;
            Description.Text = element.Name;

            selectedCategory = group.Id;
            selectedSubcat = element.Id;

            ((ListView)sender).SelectedItem = null;
        }
    }

    public class PlanningPageViewModel : INotifyPropertyChanged
    {
        public enum CalculatorKey
        {
            Zero = 0,
            One = 1,
            Two = 2,
            Three = 3,
            Four = 4,
            Five = 5,
            Six = 6,
            Seven = 7,
            Eight = 8,
            Nine = 9,
            Backspace = 20,
            Clear,
            PlusMinus,
            Divide,
            Multiply,
            Minus,
            Plus,
            Equal,
            Point,
            Ok,
            Cancel,
            Calendar
        }

        public ICommand KeyPressed { get; private set; }
        private String calculationText;
        private string categoryText;
        private string dateText;

        public event PropertyChangedEventHandler PropertyChanged;

        public string CategoryText
        {
            get { return categoryText; }
            set
            {
                categoryText = value;
                if (string.IsNullOrEmpty(categoryText))
                {
                    categoryText = " ";
                }
                OnPropertyChanged("CategoryText");
            }
        }

        public PlanningPageViewModel()
        {
            KeyPressed = new Command<string>(HandleKeyPressed);
            CalculationText = "";
        }

        public string CalculationText
        {
            get { return calculationText; }
            set
            {
                calculationText = value;
                if (string.IsNullOrEmpty(calculationText))
                {
                    calculationText = " "; // HACK to force grid view to allocate space.
                }
                OnPropertyChanged("CalculationText");
            }
        }

        public string DateText
        {
            get { return dateText; }
            set
            {
                dateText = value;
                if (string.IsNullOrEmpty(dateText))
                {
                    dateText = " ";
                }
            }
        }

        void HandleKeyPressed(string value)
        {
            var calculatorKey = (CalculatorKey)Enum.Parse(typeof(CalculatorKey), value, true);

            switch (calculatorKey)
            {
                case CalculatorKey.One:
                case CalculatorKey.Two:
                case CalculatorKey.Three:
                case CalculatorKey.Four:
                case CalculatorKey.Five:
                case CalculatorKey.Six:
                case CalculatorKey.Seven:
                case CalculatorKey.Eight:
                case CalculatorKey.Nine:
                case CalculatorKey.Zero:
                    CalculationText += ((int)calculatorKey).ToString();
                    break;
                case CalculatorKey.Equal:
                    break;
                case CalculatorKey.Point:
                    CalculationText += '.';
                    break;
            }
            return;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}