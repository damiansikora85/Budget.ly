using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeBudgeStandard.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SummaryTabsView : Grid
    {
        public event EventHandler<SummaryTabsChangedEventArgs> SelectionChanged;
        public enum Mode
        {
            Budget,
            Transactions
        }
        public bool IsBudgetSelected => _selectedMode == Mode.Budget;
        public bool IsTransactionsSelected => _selectedMode == Mode.Transactions;

        public Color BudgetTabColor => IsBudgetSelected ? Color.White : Color.LightGray;
        public Color TransactionsTabColor => IsTransactionsSelected ? Color.White : Color.LightGray;

        private Mode _selectedMode;

        public SummaryTabsView()
        {
            _selectedMode = Mode.Budget;
            BindingContext = this;
            InitializeComponent();

            VisualStateManager.GoToState(budgetTabLabel, "Selected");
            VisualStateManager.GoToState(transactionsTabLabel, "UnSelected");
        }

        private void OnBudgetTabClicked(object sender, EventArgs e)
        {
            if (_selectedMode != Mode.Budget)
            {
                VisualStateManager.GoToState((Label)sender, "Selected");
                VisualStateManager.GoToState(transactionsTabLabel, "UnSelected");
                _selectedMode = Mode.Budget;
                OnPropertyChanged(nameof(IsBudgetSelected));
                OnPropertyChanged(nameof(IsTransactionsSelected));
                OnPropertyChanged(nameof(BudgetTabColor));
                OnPropertyChanged(nameof(TransactionsTabColor));
                SelectionChanged?.Invoke(this, new SummaryTabsChangedEventArgs(Mode.Budget));
            }
        }

        private void OnTransationTabClicked(object sender, EventArgs e)
        {
            if (_selectedMode != Mode.Transactions)
            {
                VisualStateManager.GoToState((Label)sender, "Selected");
                VisualStateManager.GoToState(budgetTabLabel, "UnSelected");
                _selectedMode = Mode.Transactions;
                OnPropertyChanged(nameof(IsBudgetSelected));
                OnPropertyChanged(nameof(IsTransactionsSelected));
                OnPropertyChanged(nameof(BudgetTabColor));
                OnPropertyChanged(nameof(TransactionsTabColor));
                SelectionChanged?.Invoke(this, new SummaryTabsChangedEventArgs(Mode.Transactions));
            }
        }
    }

    public class SummaryTabsChangedEventArgs : EventArgs
    {
        public SummaryTabsView.Mode SelectedMode { get; }
        public SummaryTabsChangedEventArgs(SummaryTabsView.Mode mode)
        {
            SelectedMode = mode;
        }
    }
}