using HomeBudget.Code;
using HomeBudget.Code.Logic;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeBudget.UseCases
{
    public class AddExpenseUseCase
    {
        public static void AddExpense(double value, DateTime date, BaseBudgetCategory category, int subcatId, string note)
        {
            var budgetMonth = MainBudget.Instance.GetMonth(date);
            if (category.IsIncome)
            {
                budgetMonth.AddIncome(value, date, subcatId, note);
            }
            else
            {
                budgetMonth.AddExpense(value, date, category.Id, subcatId, note);
            }
        }
    }
}
