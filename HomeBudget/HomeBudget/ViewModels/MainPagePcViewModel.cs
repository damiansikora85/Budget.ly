using Syncfusion.Calculate;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace HomeBudget.ViewModels
{
    public class MainPagePcViewModel : INotifyPropertyChanged
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
        private String calculationResultText;
        private String formulaText;
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
            }
        }

        public String FormulaText
        {
            get
            {
                return formulaText;
            }

            set
            {
                formulaText = value;
                OnPropertyChanged("FormulaText");
            }
        }
        
        public string CalculationResultText
        {
            get
            {
                CultureInfo cultureInfoPL = new CultureInfo("pl-PL");
                return string.Format(cultureInfoPL, "{0:C}", float.Parse(calculationResultText, CultureInfo.InvariantCulture.NumberFormat));
            }
            set
            {
                try
                {
                    float.Parse(value, CultureInfo.InvariantCulture.NumberFormat);
                    calculationResultText = value;
                }
                catch
                {
                    calculationResultText = "0";
                }
                
                if (string.IsNullOrEmpty(calculationResultText))
                {
                    calculationResultText = " "; // HACK to force grid view to allocate space.
                }
                OnPropertyChanged("CalculationResultText");
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

        public MainPagePcViewModel()
        {
            KeyPressed = new Command<string>(HandleKeyPressed);
            calculationResultText = "0";
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
                    FormulaText += ((int)calculatorKey).ToString();
                    OnPropertyChanged("CalculationResultText");
                    break;
                case CalculatorKey.Equal:
                    CalcQuickBase calcQuick = new CalcQuickBase();
                    CalculationResultText = calcQuick.ParseAndCompute(formulaText);
                    break;
                case CalculatorKey.Point:
                    FormulaText += '.';
                    break;
                case CalculatorKey.Minus:
                    FormulaText += '-';
                    break;
                case CalculatorKey.Multiply:
                    FormulaText += '*';
                    break;
                case CalculatorKey.Divide:
                    FormulaText += '/';
                    break;
                case CalculatorKey.Plus:
                    FormulaText += '+';
                    break;
            }
            return;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void ForceCalculateFormula()
        {
            CalcQuickBase calcQuick = new CalcQuickBase();
            CalculationResultText = calcQuick.ParseAndCompute(formulaText);
        }
    }
}
