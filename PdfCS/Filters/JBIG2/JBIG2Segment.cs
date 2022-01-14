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
        /// Число ссылок на другие сегменты
        /// </summary>
        public byte refCount;

        /// <summary>
        /// Массив из 4 байт, номера сегментов
        /// </summary>
        public uint[] references;

        /// <summary>
        /// Номер страницы
        /// </summary>
        public byte page;

        /// <summary>
        /// Длина сегмента
        /// </summary>
        public uint length;

        public JBIG2Segment(Stream stream)
        {
            byte[] fourbytearray = new byte[4];

            stream.Read(fourbytearray, 0, 4);
            Array.Reverse(fourbytearray);
            number = BitConverter.ToUInt32(fourbytearray, 0);

            flags = (byte) stream.ReadByte();
            refCount = (byte) stream.ReadByte();

            references = new uint[refCount];
            for (int i = 0; i < references.Length; i++)
            {
                stream.Read(fourbytearray, 0, 4);
		Array.Reverse(fourbytearray);
                uint n = BitConverter.ToUInt32(fourbytearray, 0);
                references[i] = n;
            }

            page = (byte)stream.ReadByte();
            stream.Read(fourbytearray, 0, 4);
            Array.Reverse(fourbytearray);
            length = BitConverter.ToUInt32(fourbytearray, 0);
        }
    }
}
