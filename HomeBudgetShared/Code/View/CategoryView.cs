using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HomeBudget.Code
{
	public class CategoryView
	{
		public Grid subGrid;

		public Action<Grid> OnClickBtn;

		public void OnClick(object sender, EventArgs e)
		{
			subGrid.IsVisible = !subGrid.IsVisible;
			OnClickBtn(subGrid);
		}
	}
}
