using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace HomeBudget.Code
{
	public class BudgetView
	{ 
		private Grid currentSubCategory;

		public void OnClickCategory(Grid subcategoryGrid)
		{
			if (currentSubCategory != null)
				currentSubCategory.IsVisible = false;

			currentSubCategory = subcategoryGrid;
		}
	}
}
