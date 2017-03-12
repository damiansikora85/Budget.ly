using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace HomeBudget
{
	public partial class AddExpensePopup : PopupPage
	{
		public AddExpensePopup()
		{
			InitializeComponent();
		}

		private async void OnOk(object sender, EventArgs e)
		{
			Code.MainBudget.Instance.AddExpense(DateTime.Now, float.Parse(Result.Text));
			await Navigation.PopPopupAsync();
		}
	}
}
