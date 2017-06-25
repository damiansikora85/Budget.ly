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
        double expensePlanned;

		public void AddExpense(double value, int dayOfMonth)
		{
			expense[dayOfMonth] += value;
		}

        public void SetPlannedExpense(float value)
        {
            expensePlanned = value;
        }

        public double GetSum()
		{
			double result = 0;
			for(int i=0; i<31; i++)
			{
				result += expense[i];
			}

			return result;
		}

        public double GetExpenseDay(int dayNum)
        {
            return expense[dayNum];
        }

        public byte[] Serialize()
        {
            var byteArray = new byte[31 * sizeof(double)];
            Buffer.BlockCopy(expense, 0, byteArray, 0, byteArray.Length);
            List<byte> bytes = new List<byte>(byteArray);
            bytes.AddRange(BitConverter.GetBytes(expensePlanned));

            return bytes.ToArray();
        }

        public void Deserialize(BinaryData binaryData)
        {
            for (int i = 0; i < 31; i++)
            {
                expense[i] = binaryData.GetDouble();
            }
            expensePlanned = binaryData.GetDouble();
        }
    }
}
