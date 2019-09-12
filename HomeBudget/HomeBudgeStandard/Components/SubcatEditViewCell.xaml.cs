using HomeBudget.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeBudgeStandard.Components
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SubcatEditViewCell : ViewCell
    {
        public event EventHandler<BudgetSubcatEdit> OnRemove;

        public SubcatEditViewCell()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if(BindingContext is BudgetSubcatEdit data && data.IsNew)
            {
                Device.BeginInvokeOnMainThread(() => entry.Focus());
            }
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            if (BindingContext is BudgetSubcatEdit data)
            {
                OnRemove?.Invoke(this, data);
            }
        }
    }
}