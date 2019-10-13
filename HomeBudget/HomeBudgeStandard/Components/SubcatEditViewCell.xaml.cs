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
                Device.BeginInvokeOnMainThread(async() =>
                {
                    await Task.Delay(100);
                    entry.Focus();
                });
                data.IsNew = false;
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