using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Code
{
    public class BinaryData
    {
        private byte[] dataArray;
        private int currentIndex;

        public BinaryData(byte[] data)
        {
            dataArray = data;
            currentIndex = 0;
        }

        public bool GetBool()
        {
            bool value = BitConverter.ToBoolean(dataArray, currentIndex);
            currentIndex += sizeof(bool);
            return value;
        }

        public int GetInt()
        {
            int value = BitConverter.ToInt32(dataArray, currentIndex);
            currentIndex += sizeof(int);

            return value;
        }

        public double GetDouble()
        {
            double value = BitConverter.ToDouble(dataArray, currentIndex);
            currentIndex += sizeof(double);

            return value;
        }

        public string GetString()
        {
            int stringLength = GetInt();
            string value = "";
            for(int i=0; i<stringLength; i++)
            {
                value += BitConverter.ToChar(dataArray, currentIndex);
                currentIndex += sizeof(char);
            }
            return value;
        }

        public static byte[] GetBytes(string value)
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(value.Length));
            for (int i = 0; i < value.Length; i++)
                bytes.AddRange(BitConverter.GetBytes(value[i]));

            return bytes.ToArray();
        }
    }
}
