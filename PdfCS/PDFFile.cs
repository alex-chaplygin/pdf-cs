using System;
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
        public static Parser parser;

	/// <summary>
        ///   Строка версии
        /// </summary>
        private static string version;

	/// <summary>
        /// Ссылка на каталог
        /// </summary>
        public static Tuple<int, int> root;

        /// <summary>
        /// Ссылка на информацию о документе (словарь)
        /// </summary>
        public static Tuple<int, int> info;

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
        /// <summary>
        /// Читает сжатый объект из потока, содержащего объекты
        /// </summary>
        /// <param name="streamNum"> номер объекта, содержащего поток объектов </param>
        /// <param name="index"> индекc объекта в потоке </param>
        /// <returns> возвращает прочитанный объект </returns>
        public static object ReadObjectFromStream(int streamNum, int index)
	{
            /*Stream rstream = (Stream)GetObject(streamNum, out Dictionary<string, object> dict);
            if (dict.ContainsKey("Type") && (string)dict["Type"] != "ObjStm")
                throw new System.Exception("Поле Type не является ObjStm");
            rstream.Seek((int)dict["First"], SeekOrigin.Begin);
            parser = new Parser((MemoryStream)rstream);
            object o = parser.ReadToken();
            objectCache.Add(index, o);
            if (dict.ContainsKey("Extends"))
                ReadObjectFromStream(Tuple<int, int>);
            return o;*/
	    return null;
        }

	/// <summary>
        ///  Метод для чтения заголовка
        /// </summary>
        static void ReadHeader()
        {
            StreamReader reader = new StreamReader(stream);
            version = reader.ReadLine().Substring(7);
        }

	/// <summary>
        /// Читает данные в конце PDF файла
        /// указатель файла уже перемещен на начало trailer
        /// перед startxref находится словарь trailer(его читаем с помощью #11 ):
        /// trailer
        /// << key 1 value 1
        /// key 2 value 2
        /// ...
        /// key n value n
        /// startxref
        /// Поля словаря:
        /// Prev - (необязательный) смещение в файле предыдущей таблицы ссылок
        /// Root - ссылка на каталог
        /// Encrypt - словарь шифрования(если есть)
        /// Info(необязательный) - ссылка на словарь информации о документе
        /// ID(если есть шифрование или необязательный) - массив из двух байтовых строк - уникальный идентификатор файла(используется в шифровании файла)
        /// Если есть Encrypt, то вызывается инициализация шифрования #37
        /// Если было поле Prev, то нужно переместиться на данное смещение, вызвать загрузку таблицы ссылок #32
	///
        /// Пример:
        /// trailer
        /// << /Size 22
        /// /Root 2 0 R
        /// /Info 1 0 R
        /// /ID[ <81b14aafa313db63dbd6f981e49f94f4>
        /// <81b14aafa313db63dbd6f981e49f94f4> ]
        /// >>
        /// startxref
        /// 18799
        /// %%EOF
        /// </summary>
        /// 
        private static void ReadTrailer()
        {
            Dictionary<string, object> trailer = (Dictionary<string, object>)parser.ReadToken();
            root = (Tuple<int, int>)trailer["Root"];
            if (trailer.ContainsKey("Info"))
                info = (Tuple<int, int>)trailer["Info"];
            if (trailer.ContainsKey("ID") && trailer.ContainsKey("Encrypt"))
                Encryption.Init((Dictionary<string, object>)trailer["Encrypt"], (object[])trailer["ID"]);
            if (trailer.ContainsKey("Prev"))
            {
                stream.Seek((long)trailer["Prev"], SeekOrigin.Begin);
                ReadCrossReferenceTable();
            }
        }

        private static void ReadCrossReferenceTable()
        {

        }
    }
}
