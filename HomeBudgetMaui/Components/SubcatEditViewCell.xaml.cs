using HomeBudget.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui;

namespace HomeBudgetStandard.Components
{
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