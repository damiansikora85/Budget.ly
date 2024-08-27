using System.Globalization;
using System.Windows.Input;
using CommunityToolkit.Maui.Views;
using Syncfusion.Calculate;

namespace HomeBudgetStandard.Views
{
	public partial class CalcView : Popup
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
        public Action<double, string, DateTime> OnSaveValue;

        private CultureInfo _cultureInfoPL = new CultureInfo("pl-PL");

        public string Note { get; set; }
        public string Category { get; set; }
        private string _subcat;
        public string Subcat
        {
            get => _subcat;
            set
            {
                _subcat = value;
                OnPropertyChanged(nameof(Subcat));
            }
        }

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
                return string.Format(_cultureInfoPL, "{0:C}", float.Parse(calculationResultText, CultureInfo.CurrentCulture.NumberFormat));
            }
            set
            {
                try
                {
                    var style = NumberStyles.Float | NumberStyles.AllowCurrencySymbol;
                    if (float.TryParse(value, style, CultureInfo.CurrentCulture.NumberFormat, out var result))
                    {
                        calculationResultText = result < float.MaxValue ? value : "0";
                        calculationResultText = result > 0 ? calculationResultText : "0";
                    }
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

        public String DecimalSeparator => _cultureInfoPL.NumberFormat.NumberDecimalSeparator;

        private bool _dateSelected;
        private bool _saveAfterDateSelected;

        public CalcView ()
		{
            KeyPressed = new Command<string>(HandleKeyPressed);
            calculationResultText = "0";
            InitializeComponent ();
            BindingContext = this;
            dateLabel.Text = Calendar.Date.ToString("dd.MM.yyyy");
            Calendar.SelectedDateConfirmed += Calendar_DateSelected;
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
                        using (var calcQuick = new CalcQuickBase() { ThrowCircularException = true })
                        {

                            CalculationResultText = calcQuick.ParseAndCompute(formulaText);
                            OnPropertyChanged(nameof(CalculationResultText));
                            break;
                        }
                    }
                case CalculatorKey.Point:
                    {
                        if (!string.IsNullOrEmpty(FormulaText) && Char.IsDigit(FormulaText.Last()))
                            FormulaText += ',';
                        break;
                    }
                case CalculatorKey.Minus:
                    {
                        if (!string.IsNullOrEmpty(FormulaText) && Char.IsDigit(FormulaText.Last()))
                            FormulaText += '-';
                        break;
                    }
                case CalculatorKey.Multiply:
                    {
                        if (!string.IsNullOrEmpty(FormulaText) && Char.IsDigit(FormulaText.Last()))
                            FormulaText += '*';
                        break;
                    }
                case CalculatorKey.Divide:
                    {
                        if (!string.IsNullOrEmpty(FormulaText) && Char.IsDigit(FormulaText.Last()))
                            FormulaText += '/';
                        break;
                    }
                case CalculatorKey.Plus:
                    {
                        if (!string.IsNullOrEmpty(FormulaText) && Char.IsDigit(FormulaText.Last()))
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

        private void OnSave(object sender, EventArgs e)
        {
            if (_dateSelected)
            {
                using (var calcQuick = new CalcQuickBase { ThrowCircularException = true })
                {
                    CalculationResultText = calcQuick.ParseAndCompute(formulaText);
                    OnPropertyChanged("CategoryReal.TotalValues");
                    OnSaveValue?.Invoke(double.Parse(calculationResultText), Note, Calendar.Date);
                    Close();
                }
            }
            else
            {
                try
                {
                    _saveAfterDateSelected = true;
                    Calendar.Unfocus();
                    Calendar.Focus();
                }
                catch(Exception exc)
                {
                    Microsoft.AppCenter.Crashes.Crashes.TrackError(exc);
                }
            }
        }

        public void Reset()
        {
            //don't chanage order
            _saveAfterDateSelected = false;
            Calendar.Date = DateTime.Now;
            dateLabel.Text = Calendar.Date.ToString("dd.MM.yyyy");
            _dateSelected = false;

            FormulaText = string.Empty;
            CalculationResultText = "0";
            Note = "";
            OnPropertyChanged(nameof(Category));
            OnPropertyChanged(nameof(Subcat));
            OnPropertyChanged(nameof(Note));
        }

        private void OnCancelClicked(object sender, EventArgs e)
        {
            Close();
            OnCancel?.Invoke();
        }

        private void Calendar_DateSelected()
        {
            dateLabel.Text = Calendar.Date.ToString("dd.MM.yyyy");
            _dateSelected = true;

            if (_saveAfterDateSelected)
            {
                using (var calcQuick = new CalcQuickBase { ThrowCircularException = true })
                {
                    CalculationResultText = calcQuick.ParseAndCompute(formulaText);
                    OnPropertyChanged("CategoryReal.TotalValues");
                    OnSaveValue?.Invoke(double.Parse(calculationResultText), Note, Calendar.Date);
                }
            }
        }

        //protected override bool OnBackButtonPressed()
        //{
        //    OnCancel?.Invoke();
        //    return base.OnBackButtonPressed();
        //}
    }
}