using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Code
{
	public class BudgetSubcategory
	{
		float[] expense = new float[31];

		public void AddExpense(float value, int dayOfMonth)
		{
			expense[dayOfMonth] += value;
		}

		public float GetSum()
		{
			float result = 0;
			for(int i=0; i<31; i++)
			{
				result += expense[i];
			}

            //Random rand = new Random();
            //result = (float)rand.NextDouble() * 100;

			return result;
		}
	}
}
