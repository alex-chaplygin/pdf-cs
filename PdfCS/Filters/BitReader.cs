using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfCS
{
    public class BitReader
    {
        private byte[] array;
        private int bytePos;
        private int bitPos;

        public BitReader(byte[] arr)
        {
            array = arr;
            bytePos = 0;
            bitPos = 0;
        }

        public int GetBit()
        {
            if (bytePos >= array.Length)
                throw new Exception("Выход за пределы массива -> BitReader, GetBit");
            else
            {
                int bit = array[bytePos] >> bitPos++ & 1;
                if (bitPos == 8)
                {
                    bitPos = 0;
                    bytePos++;
                }
                return bit;
            }
        }

        public int GetBits(int num)
        {
            int res = 0;

            for (int i = 0; i < num; i++)
	    {
                res += GetBit();
		res <<= 1;
	    }
            return res;
        }
    }
}
