using Syncfusion.Calculate;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeBudgeStandard.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CalcView : Frame
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
        public Action OnCancel;
        public Action<double, DateTime> OnSaveValue;

        private String formulaText;
        public String FormulaText
        {
            get
            {
                return formulaText;
            }

            set
            {
                formulaText = value;
                OnPropertyChanged(nameof(FormulaText));
            }
        }


        private String calculationResultText;

        public string CalculationResultText
        {
            get
            {
                var cultureInfoPL = new CultureInfo("pl-PL");
                return string.Format(cultureInfoPL, "{0:C}", float.Parse(calculationResultText, CultureInfo.InvariantCulture.NumberFormat));
            }
            set
            {
                try
                {
                    var style = NumberStyles.Float | NumberStyles.AllowCurrencySymbol;
                    if (float.TryParse(value, style, CultureInfo.InvariantCulture.NumberFormat, out var result))
                        calculationResultText = value;
                    else
                        calculationResultText = "0";
                }
                catch (Exception)
                {
                    calculationResultText = "0";
                }

                if (string.IsNullOrEmpty(calculationResultText))
                {
                    calculationResultText = " "; // HACK to force grid view to allocate space.
                }
            }
        }

        public CalcView ()
		{
            KeyPressed = new Command<string>(HandleKeyPressed);
            calculationResultText = "0";
            InitializeComponent ();
            BindingContext = this;
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
                    {
                        FormulaText += ((int)calculatorKey).ToString();
                        OnPropertyChanged(nameof(CalculationResultText));
                        break;
                    }
                case CalculatorKey.Equal:
                    {
                        using (var calcQuick = new CalcQuickBase())
                        {
                            CalculationResultText = calcQuick.ParseAndCompute(formulaText);
                            OnPropertyChanged(nameof(CalculationResultText));
                            break;
                        }
                    }
                case CalculatorKey.Point:
                    {
                        FormulaText += '.';
                        break;
                    }
                case CalculatorKey.Minus:
                    {
                        FormulaText += '-';
                        break;
                    }
                case CalculatorKey.Multiply:
                    {
                        FormulaText += '*';
                        break;
                    }
                case CalculatorKey.Divide:
                    {
                        FormulaText += '/';
                        break;
                    }
                case CalculatorKey.Plus:
                    {
                        FormulaText += '+';
                        break;
                    }
                case CalculatorKey.Backspace:
                    if (!string.IsNullOrEmpty(FormulaText))
                        FormulaText = FormulaText.Substring(0, FormulaText.Length - 1);
                    break;
                default:
                    throw new Exception("Unexpected Case");
            }
            return;
        }

        void OnSave(object sender, EventArgs e)
        {
            using (var calcQuick = new CalcQuickBase())
            {
                CalculationResultText = calcQuick.ParseAndCompute(formulaText);
                OnPropertyChanged("CategoryReal.TotalValues");
                OnSaveValue?.Invoke(double.Parse(calculationResultText), Calendar.Date);
            }
        }

        public void Reset()
        {
            FormulaText = string.Empty;
            CalculationResultText = "0";
        }

        private void OnCancelClicked(object sender, EventArgs e)
        {
            OnCancel?.Invoke();
        }
    }
}