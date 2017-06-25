using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace HomeBudget
{
    public partial class AddIncomePopup : PopupPage
    {

        public AddIncomePopup(string categoryName, string subcatName, DateTime date, Action<DateTime> callback)
        {
            InitializeComponent();
            var viewModel = new AddIncomeViewModel(callback)
            {
                Navigation = Navigation
            };
            BindingContext = viewModel;

            category.Text = categoryName;
            subcategory.Text = subcatName;
            DateButton.Text = date.ToString("d");
        }
    }

    public class AddIncomeViewModel : INotifyPropertyChanged
    {
        Action<DateTime> calendarCallback;

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

        public INavigation Navigation;

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

        public AddIncomeViewModel(Action<DateTime> callback)
        {
            KeyPressed = new Command<string>(HandleKeyPressed);
            CalculationText = "";
            calendarCallback = callback;
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
            //OnPropertyChanged("DateText");
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
                case CalculatorKey.Ok:
                    SaveValue();
                    break;
                case CalculatorKey.Cancel:
                    Navigation.PopPopupAsync();
                    break;
                case CalculatorKey.Calendar:
                    OpenCalendar();
                    break;
            }
            return;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void SaveValue()
        {
           // await Code.MainBudget.Instance.AddIncome(float.Parse(calculationText));
            await Navigation.PopPopupAsync();
        }

        private async void OpenCalendar()
        {
            var page = new CalendarPopup(calendarCallback);
            await Navigation.PushModalAsync(page);
            await Navigation.PopPopupAsync();
        }
    }
}
