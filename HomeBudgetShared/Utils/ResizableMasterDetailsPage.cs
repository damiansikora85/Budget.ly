using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HomeBudget.Utils
{
    class ResizableMasterDetailsPage : MasterDetailPage
    {
        public static readonly BindableProperty DrawerWidthProperty = BindableProperty.Create<ResizableMasterDetailsPage, int>(p => p.DrawerWidth, default(int));

        public int DrawerWidth
        {
            get { return (int)GetValue(DrawerWidthProperty); }
            set { SetValue(DrawerWidthProperty, value); }
        }
    }
}
