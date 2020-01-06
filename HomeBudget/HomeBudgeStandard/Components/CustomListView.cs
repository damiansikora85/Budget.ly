using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace HomeBudget.Components
{
    public class CustomListView : ListView
    {
        public static BindableProperty FirstElementVisibiltyPercProperty = BindableProperty.Create(nameof(FirstElementVisibiltyPerc), typeof(float), typeof(CustomListView));
        public static BindableProperty ScrollPositionProperty = BindableProperty.Create(nameof(ScrollPosition), typeof(int), typeof(CustomListView));
        public float FirstElementVisibiltyPerc
        {
            get => (float)GetValue(FirstElementVisibiltyPercProperty);
            set => SetValue(FirstElementVisibiltyPercProperty, value);
        }

        public int ScrollPosition
        {
            get => (int)GetValue(ScrollPositionProperty);
            set => SetValue(ScrollPositionProperty, value);
        }

        public event EventHandler OnScroll;

        public void WasScrolled()
        {
            OnScroll?.Invoke(this, null);
        }
    }
}
