using System;
using System.Collections.Generic;
using System.Linq;
using System.IO.Compression;
using System.IO;

namespace PdfCS
{
    /// <summary>
    ///  Класс декодирования Deflate
    /// </summary>
    class Flate
    {
        /// <summary>
        ///   код алгоритма предсказания
        /// </summary>
        private static int predictor;

        /// <summary>
        ///   число цветовых компонент
        /// </summary>
        private static int colors;

        /// <summary>
        ///   число бит на один цветовой компонент
        /// </summary>
        private static int bpp;

        /// <summary>
        ///   число столбцов в изображении
        /// </summary>
        private static int columns;

        private static void InitParams(Dictionary<string, object> p)
        {
            predictor = p.ContainsKey("Predictor") ? (int)p["Predictor"] : 1;
            colors = p.ContainsKey("Colors") ? (int)p["Colors"] : 1;
            bpp = p.ContainsKey("BitsPerComponent") ? (int)p["BitsPerComponent"] : 8;
            if (p.ContainsKey("Columns"))
                columns = (int)p["Columns"];
        }

        /// <summary>
        ///   Метод, декодирующий поток байт, сжатых методом Deflate
        /// </summary>
        /// <param name="stream">массив закодированных байт</param>
        /// <param name="predictParams">словарь параметров алгоритма предсказания</param>
        /// <returns>
        /// массив декодированных байт
        /// </returns>
        public static byte[] Decode(byte[] stream, Dictionary<string, object> predictParams = null) 
        {
            InitParams(predictParams);
            Stream compStream = new MemoryStream(stream);
            DeflateStream decompStream =  new DeflateStream(compStream, CompressionMode.Decompress);
            byte[] decompressed = new byte[decompStream.Length];
            decompStream.Read(decompressed, 0, (int)decompStream.Length);
            return Predictor.Decode(decompressed, predictor, colors, bpp, columns);
        }
    }
}
