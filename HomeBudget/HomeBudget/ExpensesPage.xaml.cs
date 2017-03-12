using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace HomeBudget
{
	public partial class ExpensesPage : ContentPage
	{
		public ExpensesPage()
		{
			InitializeComponent();

			CreateSubCategory();
		}

		private void CreateSubCategory()
		{
			Button btn1 = new Button();
			btn1.Text = "btn1";

			Button btn2 = new Button();
			btn2.Text = "btn2";

			Button btn3 = new Button();
			btn3.Text = "btn3";

			FoodDetailsGrid.Children.Add(btn1, 0, 0);
			FoodDetailsGrid.Children.Add(btn2, 1, 0);
			FoodDetailsGrid.Children.Add(btn3, 0, 1);
		}

		private void OnFoodClick(object sender, EventArgs e)
		{
			HideAllDetailsGridsExceptOne(FoodDetailsGrid);
			FoodDetailsGrid.IsVisible = !FoodDetailsGrid.IsVisible;

			//ShowExpensePopup();
		}

		private void OnHomeClick(object sender, EventArgs e)
		{
			HideAllDetailsGridsExceptOne(HomeDetailsGrid);
			HomeDetailsGrid.IsVisible = !HomeDetailsGrid.IsVisible;
		}

		private void OnTransportClick(object sender, EventArgs e)
		{
			HideAllDetailsGridsExceptOne(TransportDetailsGrid);
			TransportDetailsGrid.IsVisible = !TransportDetailsGrid.IsVisible;
		}


		private void OnTelekomClick(object sender, EventArgs e)
		{
			HideAllDetailsGridsExceptOne(TelekomDetailsGrid);
			TelekomDetailsGrid.IsVisible = !TelekomDetailsGrid.IsVisible;
		}

		private void OnHealthClick(object sender, EventArgs e)
		{
			HideAllDetailsGridsExceptOne(HealthDetailsGrid);
			HealthDetailsGrid.IsVisible = !HealthDetailsGrid.IsVisible;
		}

		private void OnClothesClick(object sender, EventArgs e)
		{
			HideAllDetailsGridsExceptOne(ClothesDetailsGrid);
			ClothesDetailsGrid.IsVisible = !ClothesDetailsGrid.IsVisible;
		}

		private void OnHygenClick(object sender, EventArgs e)
		{
			HideAllDetailsGridsExceptOne(HygenDetailsGrid);
			HygenDetailsGrid.IsVisible = !HygenDetailsGrid.IsVisible;
		}

		private void OnKidsClick(object sender, EventArgs e)
		{
			HideAllDetailsGridsExceptOne(KidsDetailsGrid);
			KidsDetailsGrid.IsVisible = !KidsDetailsGrid.IsVisible;
		}

		private void OnEntertainmentClick(object sender, EventArgs e)
		{
			HideAllDetailsGridsExceptOne(EntertainmentDetailsGrid);
			EntertainmentDetailsGrid.IsVisible = !EntertainmentDetailsGrid.IsVisible;
		}

		private void OnOthersClick(object sender, EventArgs e)
		{
			HideAllDetailsGridsExceptOne(OthersDetailsGrid);
			OthersDetailsGrid.IsVisible = !OthersDetailsGrid.IsVisible;
		}

		private void OnDebtClick(object sender, EventArgs e)
		{
			HideAllDetailsGridsExceptOne(DebtsDetailsGrid);
			DebtsDetailsGrid.IsVisible = !DebtsDetailsGrid.IsVisible;
		}

		private void OnSavingsClick(object sender, EventArgs e)
		{
			HideAllDetailsGridsExceptOne(SavingsDetailsGrid);
			SavingsDetailsGrid.IsVisible = !SavingsDetailsGrid.IsVisible;
		}

		

		private void HideAllDetailsGridsExceptOne(VisualElement element)
		{
			bool visible = element.IsVisible;

			FoodDetailsGrid.IsVisible = false;
			HomeDetailsGrid.IsVisible = false;
			TransportDetailsGrid.IsVisible = false;
			TelekomDetailsGrid.IsVisible = false;
			HealthDetailsGrid.IsVisible = false;
			ClothesDetailsGrid.IsVisible = false;
			HygenDetailsGrid.IsVisible = false;
			KidsDetailsGrid.IsVisible = false;
			EntertainmentDetailsGrid.IsVisible = false;
			OthersDetailsGrid.IsVisible = false;
			DebtsDetailsGrid.IsVisible = false;
			SavingsDetailsGrid.IsVisible = false;

			element.IsVisible = visible;
		}

		private async void OnAddExpense(object sender, EventArgs e)
		{
			var page = new AddExpensePopup();
			var viewModel = new AddExpenseViewModel();
			page.BindingContext = viewModel;

			await Navigation.PushPopupAsync(page);
		}
		
	}
}
