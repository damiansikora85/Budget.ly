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
	public class AddExpenseViewModel : INotifyPropertyChanged
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
			Ok
		}

		public ICommand KeyPressed { get; private set; }
		private String calculationText;

		public event PropertyChangedEventHandler PropertyChanged;

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

		public AddExpenseViewModel()
		{
			KeyPressed = new Command<string>(HandleKeyPressed);
			CalculationText = "";
		}

		void HandleKeyPressed(string value)
		{
			var calculatorKey = (CalculatorKey)Enum.Parse(typeof(CalculatorKey), value, true);

			switch(calculatorKey)
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
				case CalculatorKey.Ok:
					break;
			}
			return;
		}

		protected virtual void OnPropertyChanged(string propertyName)
		{
			var handler = PropertyChanged;
			if (handler != null)
				handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
