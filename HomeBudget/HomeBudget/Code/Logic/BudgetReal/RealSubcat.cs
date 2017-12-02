using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Code.Logic
{
    public class RealSubcat : BaseBudgetSubcat
    {
        private double[] values = new double[31];

        public static RealSubcat Create(string subcatName, int id)
        {
            RealSubcat subcat = new RealSubcat()
            {
                Name = subcatName,
                Id = id
            };

            return subcat;
        }

        public double[] Values
        {
            get
            {
                return values;
            }
            set
            {
                values = value;
            }
        }

        public override double Value
        {
            get
            {
                return values.Sum();
            }
            set
            {
                values[0] = value;
            }
        }

        public override byte[] Serialize()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(base.Serialize());
            var byteArray = new byte[31 * sizeof(double)];
            Buffer.BlockCopy(values, 0, byteArray, 0, byteArray.Length);
            bytes.AddRange(byteArray);

            return bytes.ToArray();
        }

        public override void Deserialize(BinaryData binaryData)
        {
            base.Deserialize(binaryData);
            for (int i = 0; i < 31; i++)
            {
                values[i] = binaryData.GetDouble();
            }
        }

        public void AddValue(double value, DateTime date)
        {
            values[date.Day] += value;
        }
    }
}
