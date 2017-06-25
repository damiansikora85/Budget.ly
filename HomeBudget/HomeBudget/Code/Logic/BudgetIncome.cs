using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Code
{
    public class BudgetIncome
    {
        public string Name;
        public int Id;

        private double[] incomes;
        private double incomesPlanned;

        public static BudgetIncome Create(BudgetIncomeTemplate incomeTemplate)
        {
            BudgetIncome income = new BudgetIncome()
            {
                Name = incomeTemplate.Name,
                Id = incomeTemplate.Id
            };

            return income;
        }

        public static BudgetIncome CreateWithBinaryData(BinaryData binaryData)
        {
            BudgetIncome income = new BudgetIncome();
            income.Deserialize(binaryData);

            return income;
        }

        private BudgetIncome()
        {
            incomes = new double[31];
        }

        public byte[] Serialize()
        {
            List<byte> bytes = new List<byte>();

            bytes.AddRange(BinaryData.GetBytes(Name));
            bytes.AddRange(BitConverter.GetBytes(Id));

            var byteArray = new byte[31 * sizeof(double)];
            Buffer.BlockCopy(incomes, 0, byteArray, 0, byteArray.Length);
            bytes.AddRange(byteArray);
            bytes.AddRange(BitConverter.GetBytes(incomesPlanned));

            return bytes.ToArray();
        }

        public void Deserialize(BinaryData binaryData)
        {
            Name = binaryData.GetString();
            Id = binaryData.GetInt();

            for (int i = 0; i < 31; i++)
            {
                incomes[i] = binaryData.GetDouble();
            }
            incomesPlanned = binaryData.GetDouble();
        }

        public double GetTotal()
        {
            double incomeSum = 0;
            for (int i = 0; i < 31; i++)
            {
                incomeSum += incomes[i];
            }

            return incomeSum;
        }

        public void SetPlannedIncome(float value)
        {
            incomesPlanned = value;
        }

        public void AddIncome(double value, DateTime date)
        {
            incomes[date.Day] += value;
        }
    }
}
