using HomeBudgeStandard.Views;
using HomeBudget.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeBudgeStandard.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainTabbedPage : TabbedPage
    {
        public MainTabbedPage ()
        {
            InitializeComponent();

            CurrentPageChanged += OnTabChanged;
            MainBudget.Instance.BudgetDataChanged += BudgetDataChanged;
        }

        private void BudgetDataChanged()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (CurrentPage != Children[0])
                    CurrentPage = Children[0];
            }
            );
        }

        private void OnTabChanged(object sender, EventArgs e)
        {
            
        }

        public bool OnBackPressed()
        {
            if (CurrentPage is SummaryView summaryView)
                return summaryView.OnBackPressed();

            return false;
        }
    }
}