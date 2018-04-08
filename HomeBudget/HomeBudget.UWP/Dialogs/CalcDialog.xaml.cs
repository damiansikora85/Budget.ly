using HomeBudget.UWP.Utils;
using System;
using System.Globalization;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using HomeBudget.ViewModels;
using static HomeBudget.ViewModels.MainPagePcViewModel;
using System.ComponentModel;
using Syncfusion.Calculate;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace HomeBudget.UWP
{
    public sealed partial class CalcDialog : ContentDialog, INotifyPropertyChanged
    {
        public DelegateCommand KeyPressed { get; private set; }
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

        CultureInfo cultureInfoPL = new CultureInfo("pl-PL");

        public string Date => DateTime.Now.ToString("dd.MM.yyyy", cultureInfoPL);

        public CalcDialog()
        {
            InitializeComponent();
            DataContext = this;

            KeyPressed = new DelegateCommand(HandleKeyPressed);
            calculationResultText = "0";
        }

        public CalcDialog(string title)
        {
            KeyPressed = new DelegateCommand(HandleKeyPressed);

            InitializeComponent();
            DataContext = this;

            calculationResultText = "0";
            Title = title;
            Calendar.Date = DateTime.Now;
        }

        private String calculationResultText;

        public event PropertyChangedEventHandler PropertyChanged;

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
                    var style = NumberStyles.Float|NumberStyles.AllowCurrencySymbol;
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

        private void OnCancel()
        {
            Hide();
        }

        void OnSave()
        {
            using (var calcQuick = new CalcQuickBase())
            {
                CalculationResultText = calcQuick.ParseAndCompute(formulaText);
                OnPropertyChanged("CategoryReal.TotalValues");
                OnSaveValue?.Invoke(double.Parse(calculationResultText), Calendar.Date.Value.DateTime);
            }
        }

        void HandleKeyPressed(object param)
        {
            var value = (string)param;
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
                    OnPropertyChanged(nameof(CalculationResultText));
                    break;
                case CalculatorKey.Equal:
                    if (!string.IsNullOrEmpty(formulaText))
                    {
                        using (var calcQuick = new CalcQuickBase())
                        {
                            CalculationResultText = calcQuick.ParseAndCompute(formulaText);
                            OnPropertyChanged(nameof(CalculationResultText));
                        }
                    }
                    break;
                case CalculatorKey.Point:
                    FormulaText += cultureInfoPL.NumberFormat.NumberDecimalSeparator;
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
                case CalculatorKey.Backspace:
                    if(!string.IsNullOrEmpty(FormulaText))
                        FormulaText = FormulaText.Substring(0, FormulaText.Length - 1);
                    break;
            }
            return;
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if(sender is TextBox textbox)
                FormulaText = textbox.Text;
        }

        private void TextBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (sender is TextBox textbox && e.Key == Windows.System.VirtualKey.Enter)
            {
                FormulaText = textbox.Text;
                using (var calcQuick = new CalcQuickBase())
                {
                    CalculationResultText = calcQuick.ParseAndCompute(formulaText);
                    OnPropertyChanged(nameof(CalculationResultText));
                }
            }
        }
    }
}
