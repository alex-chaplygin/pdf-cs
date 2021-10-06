using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfCS
{
    /// <summary>
    ///   Класс для чтения бит из массива
    /// </summary>
    public class BitReader
    {
	/// <summary>
	///   Массив, откуда считываются биты
	/// </summary>
        private byte[] array;

	/// <summary>
	///   Номер текущего байта
	/// </summary>
        private int bytePos;

	/// <summary>
	///   Позиция младшего бита
	/// </summary>
        private int bitPosLS;

	/// <summary>
	///   Позиция старшего бита
	/// </summary>
        private int bitPosMS;

	/// <summary>
	///   Инициализация класса
	/// </summary>
        /// <param name="arr">Массив откуда будут считываться биты</param>
        public BitReader(byte[] arr)
        {
            array = arr;
            bytePos = 0;
            bitPosLS = 0;
            bitPosMS = 7;
        }

	/// <summary>
	///   Читает бит, начиная с младшего
	/// </summary>
        /// <returns>
        /// прочитанный бит
        /// </returns>
        public int GetBitLS()
        {
            if (bytePos >= array.Length)
                throw new Exception("Выход за пределы массива -> BitReader, GetBit");
            else
            {
                int bit = array[bytePos] >> bitPosLS++ & 1;
                if (bitPosLS == 8)
                {
                    bitPosLS = 0;
                    bytePos++;
                }
                return bit;
            }
        }

	/// <summary>
	///   Читает бит, начиная со старшего
	/// </summary>
        /// <returns>
        /// прочитанный бит
        /// </returns>
        public int GetBitMS()
        {
            if (bytePos >= array.Length)
                throw new Exception("Выход за пределы массива -> BitReader, GetBit");
            else
            {
                int bit = array[bytePos] >> bitPosMS-- & 1;
                if (bitPosMS < 0)
                {
                    bitPosMS = 7;
                    bytePos++;
                }
                return bit;
            }
        }

	/// <summary>
	///   Читает последовательность бит, старшие биты впереди
	/// </summary>
        /// <param name="num">Число бит, сколько будет прочитано</param>
        /// <returns>
        /// последовательность бит
        /// </returns>
        public int GetBitsMS(int num)
        {
            int res = 0;

            for (int i = 0; i < num; i++)
	    {
		res <<= 1;
                res += GetBitMS();
	    }
            return res;
        }

	/// <summary>
	///   Читает последовательность бит, младшие биты ставятся на свои места
	/// </summary>
        /// <param name="num">Число бит, сколько будет прочитано</param>
        /// <returns>
        /// последовательность бит
        /// </returns>
	public int GetBitsLS(int num)
        {
            int res = 0;

            for (int i = 0; i < num; i++)
                res += GetBitLS() << i;

            return res;
        }
    }
}
