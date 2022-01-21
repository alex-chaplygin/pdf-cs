using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfCS
{
    public class JBIG2Segment
    {
        /// <summary>
        /// Номер сегмента
        /// </summary>
        public uint number;

        /// <summary>
        /// Флаги и тип сегмента
        /// </summary>
        public byte flags;

	/// <summary>
        /// Тип сегмента
        /// </summary>
        public int type;
	
        /// <summary>
        /// Число ссылок на другие сегменты
        /// </summary>
        public uint refCount;

        /// <summary>
        /// Массив из 4 байт, номера сегментов
        /// </summary>
        public int[] references;

        /// <summary>
        /// Номер страницы
        /// </summary>
        public uint page;

        /// <summary>
        /// Длина сегмента
        /// </summary>
        public uint length;

	/// <summary>
	///   Поток JBIG2
	/// </summary>
	private Stream stream;

	/// <summary>
	///   Читает сегмент из потока stream
	/// </summary>
        public JBIG2Segment(Stream stream)
        {
	    this.stream = stream;
	    number = ReadFourBytes();

            flags = (byte) stream.ReadByte();
            type = (byte) (flags << 2) >> 2;

            var bit = (byte) ((flags >> 6) & 1);
            if (bit == 0)
                refCount = (byte)stream.ReadByte();
            else
                refCount = ReadFourBytes();

            if (refCount >= 4)
                stream.Position = stream.Position + (refCount + 1) / 8 + 1;

            int numByte = 1;
            if (refCount > 256 && refCount <= 65536)
                numByte = 2;
            else if (refCount > 65536)
                numByte = 4;

            references = new int[refCount];
            for (int i = 0; i < references.Length; i++)
                references[i] = ReadValue(numByte);

            if (bit == 0)
                page = (byte)stream.ReadByte();
            else
                page = ReadFourBytes();

            length = ReadFourBytes();
        }

	/// <summary>
        /// Чтение 4 байт из потока и преобразование в число типа UInt
        /// </summary>
        /// <param name="stream"> Заданный поток </param>
        /// <returns></returns>
        private uint ReadFourBytes()
        {
	    byte[] fourbytearray = new byte[4];
            stream.Read(fourbytearray, 0, 4);
            Array.Reverse(fourbytearray);
            return BitConverter.ToUInt32(fourbytearray, 0);
        }

	/// <summary>
        /// Чтение элемента references из потока
        /// </summary>
        /// <param name="stream"> Заданный поток </param>
        /// <param name="numByte"> Количество байт на один элемент массива references </param>
        /// <returns></returns>
        private int ReadValue(int numByte)
        {
            if (numByte == 3 || numByte > 4)
                throw new ArgumentException("Неверное значение numByte");
            byte[] numBytes = new byte[numByte];
            if (numByte == 1)
                return stream.ReadByte();
            else if (numByte == 2)
            {
                stream.Read(numBytes, 0, 2);
                return (numBytes[0] << 8) | numBytes[1];
            }
            else
                return Convert.ToInt32(ReadFourBytes());
        }
    }
}
