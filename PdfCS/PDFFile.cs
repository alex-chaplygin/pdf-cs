using System.Collections.Generic;
using System.IO;

namespace PdfCS
{
    /// <summary>
    /// Класс чтения PDF файла
    /// </summary>
    public class PDFFile
    {
        /// <summary>
        /// Cтруктура для элемента таблицы ссылок
        /// </summary>
        public struct XRefEntry
        {
            /// <summary>
            /// смещение в файле
            /// </summary>
            public int offset;

            /// <summary>
            /// номер поколения (0 для нового объекта, далее увеличивается при каждом обновлении)
            /// </summary>
            public int generation;

            /// <summary>
            /// true - если свободный элемент
            /// </summary>
            public bool free;

            /// <summary>
            /// true - если сжатый объект
            /// </summary>
            public bool compressed;

            /// <summary>
            /// для сжатого объекта - номер потока объектов
            /// </summary>
            public int streamNum;

            /// <summary>
            /// индекс внутри потока
            /// </summary>
            public int streamIndex;
        }
	
        /// <summary>
        ///   Поток файла
        /// </summary>
        private static Stream stream;

        /// <summary>
        /// Кэш объектов - ключ - номер, значение - объект
        /// </summary>
        private static Dictionary<int, object> objectCache;

        /// <summary>
        /// Таблица ссылок
        /// </summary>
        private static XRefEntry[] xrefTable;

        /// <summary>
        /// Объект класса Parser - синтаксического разбора объектов PDF
        /// </summary>
        private static Parser parser;

        /// <summary>
        /// Метод чтения объекта кэша -
        /// если объект есть в кеше, то берем объект из кеша, если нет, то используем смещение из таблицы ссылок, 
        /// перемещаем указатель потока на это смещение и считываем объект методом ReadIndirectObject затем помещаем объект в кеш
        /// если в таблице ссылок объект сжатый, то его нужно прочитать из потока с помощью метода ReadObjectFromStream
        /// </summary>
        /// <param name="num"> номер объекта </param>
        /// <param name="dic"> словарь потока (если есть) </param>
        /// <returns> возвращает прочитанный объект </returns>
        public static object GetObject(int num, out Dictionary<string, object> dict)
        {
            object obj;
	    dict = null;
            if (objectCache.ContainsKey(num))
                return objectCache[num];
            if (xrefTable[num].compressed)
                obj = ReadObjectFromStream(xrefTable[num].streamNum, xrefTable[num].streamIndex);
            else {
		stream.Seek(xrefTable[num].offset, SeekOrigin.Begin);
                obj = parser.ReadIndirectObject(out dict);
	    }
            objectCache.Add(num, obj);
            return obj;
        }

	public static object ReadObjectFromStream(int streamNum, int index)
	{
	    return null;
	}
    }
}
