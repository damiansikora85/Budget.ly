using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Code
{
	public class BudgetDailyExpense
	{
		private float dailyExpense;
		private DateTime date;
		public DateTime Date
		{
			get { return date; }
		}

		public BudgetDailyExpense(DateTime date)
		{
			this.date = date;
			dailyExpense = 0;
		}

		public void AddExpense(float value)
		{
			dailyExpense += value;
		}
	}
}
