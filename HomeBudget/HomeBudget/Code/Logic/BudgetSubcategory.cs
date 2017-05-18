using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Code
{
	public class BudgetSubcategory
	{
		double[] expense = new double[31];

		public void AddExpense(float value, int dayOfMonth)
		{
			expense[dayOfMonth] += value;
		}

		public double GetSum()
		{
			double result = 0;
			for(int i=0; i<31; i++)
			{
				result += expense[i];
			}

            //Random rand = new Random();
            //result = (float)rand.NextDouble() * 100;

			return result;
		}

        public byte[] Serialize()
        {
            var byteArray = new byte[31 * sizeof(double)];
            Buffer.BlockCopy(expense, 0, byteArray, 0, byteArray.Length);

            return byteArray;
        }

        public void Deserialize(BinaryData binaryData)
        {
            for (int i = 0; i < 31; i++)
            {
                expense[i] = binaryData.GetDouble();
            }
        }
	}
}
