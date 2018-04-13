using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace HomeBudgeStandard.Utils
{
    class MasterDetailPageExtended : MasterDetailPage
    {
        public static readonly BindableProperty DrawerWidthProperty = BindableProperty.Create<MasterDetailPageExtended, int>(p => p.DrawerWidth, default(int));

        public int DrawerWidth
        {
            get { return (int)GetValue(DrawerWidthProperty); }
            set { SetValue(DrawerWidthProperty, value); }
        }
    }
}
