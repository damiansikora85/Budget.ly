using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

using Xamarin.Forms;

namespace HomeBudget
{
	public class WritePage : ContentPage
	{
		public WritePage()
		{
			Content = new StackLayout
			{
				Children = {
					new Label { Text = "Write Page" }
				}
			};
		}
	}
}
