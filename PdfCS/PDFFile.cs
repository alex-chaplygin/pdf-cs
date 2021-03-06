using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
	    public XRefEntry(int o, int g, bool f)
            {
                offset = o;
                generation = g;
                free = f;
                compressed = false;
                streamNum = 0;
                streamIndex = 0;
            }

	    public XRefEntry(bool c, int sn, int si)
            {
                offset = 0;
                generation = 0;
                free = false;
                compressed = c;
                streamNum = sn;
                streamIndex = si;
            }
	    
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
        public static Stream stream;

        /// <summary>
        /// Кэш объектов - ключ - номер, значение - объект
        /// </summary>
        public static Dictionary<int, object> objectCache;

        /// <summary>
        /// Таблица ссылок
        /// </summary>
        public static XRefEntry[] xrefTable;

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
        public static Tuple<int, int> root = null;

        /// <summary>
        /// Ссылка на информацию о документе (словарь)
        /// </summary>
        public static Tuple<int, int> info = null;

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
		parser.NextChar();
                obj = parser.ReadIndirectObject(out dict);
	    }
            objectCache.Add(num, obj);
            return obj;
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
        private static void ReadTrailer(Dictionary<string, object> trailer)
        {
//            Dictionary<string, object> trailer = (Dictionary<string, object>)parser.ReadToken();
	    if (root == null)
		root = (Tuple<int, int>)trailer["Root"];
            if (trailer.ContainsKey("Info")  && info == null)
                info = (Tuple<int, int>)trailer["Info"];
            if (trailer.ContainsKey("ID") && trailer.ContainsKey("Encrypt"))
	    {
		Dictionary<string, object> dict;
		Encryption.Init((Dictionary<string, object>)LoadLink(trailer["Encrypt"], out dict), (object[])trailer["ID"]);
		if (!Encryption.UserAuthentificate(""))
                    throw new Exception("Требуется пароль");
	    }
            if (trailer.ContainsKey("Prev"))
            {
                stream.Seek(Convert.ToInt64(trailer["Prev"]), SeekOrigin.Begin);
		parser.NextChar();
                ReadCrossReferenceTable();
            }
        }

        /// <summary>
        ///   Загружает объект по ссылке если объект - ссылка
        /// </summary>
        public static object LoadLink(object o, out Dictionary<string, object> dict)
        {
            dict = null;
            if (o is Tuple<int, int>)
            {
                var tuple = (Tuple<int, int>)o;
                return GetObject(tuple.Item1, out dict);
            }
            return o;
        }

        /// <summary>
        /// загружает поток ссылок, позиция в файле уже установлена
        /// поток загружается с помощью #25
        /// словарь содержит те же поля, что и обычная таблица ссылок, их нужно считать из словаря #32
        /// Пример:
        /// 0 obj % Cross-reference stream
        /// << /Type /XRef % Cross-reference stream dictionary
        /// /Size...
        /// /Root...
        /// 
        /// stream
        /// ...Stream data containing cross-reference information ...
        /// endstream
        /// endobj
        /// также словарь содержит поля из хвоста #33
        /// их нужно сохранить в соответствующие поля класса
        /// все следующие значения являются прямыми объектами
        /// Дополнительные поля словаря:
        /// Type - XRef
        /// Size - максимальный номер объекта + 1
        /// Index(необязательный) - массив из пар целых чисел: номер первого объекта и число объектов в каждой секции #32
        /// Prev(необязательно) - смещение на предыдущую таблицу ссылок, если есть смещаемся в файле и вызываем загрузку таблицы ссылок #32
        /// W - массив из 3х целых чисел: длина полей для записей в байтах, например[1 2 1]
        /// если какой-то элемент равен 0, значит этого поля нет, нужно использовать значение по умолчанию
        /// если первого поля нет, то по умолчанию берется тип 1
        /// Каждая запись в потоке имеет 1 или более полей,
        /// первое поле - тип записи(0, 1, 2) любое другое значение означает, что объект - null
        /// длина каждого поля определяется через W
        /// Тип 0 - свободный объект
        /// поле 2 - номер следующего свободного объекта(игнорируем)
        /// поле 3 - номер поколения
        /// Тип 1 - не сжатый объект
        ///  поле2 - смещение
        ///  поле3 - номер поколения(по умолчанию 0)
        ///  Тип 2 - сжатый объект
        /// поле 2 - номер потокового объекта, где хранится объект
        /// поле 3 - индекс объекта внутри потока
        /// </summary>
        public static void LoadXRefStream() //метод #36
        {
            Dictionary<string, object> dict;
            MemoryStream m = new MemoryStream((byte[])parser.ReadIndirectObject(out dict));
            int size = (int)dict["Size"];
            object[] W = (object[])dict["W"];
	    Console.WriteLine($"XRef size = {size}");

            int[] val = new int[3];

            for (int i = 0; i < size; i++)
            {
                for (int w = 0; w < 3; w++)
                {
                    val[w] = 0;
                    for (int k = 0; k < (int)W[w]; k++) //чтение в соответствии с длиной поля
		    {
			val[w] <<= 8;
			val[w] += m.ReadByte();
		    }
                }

                if (xrefTable == null)
                    xrefTable = new XRefEntry[1];
                else
                    Array.Resize(ref xrefTable, xrefTable.Length + 1);

                 if ((int)W[0] == 0) // если первого поля нет, то по умолчанию берется тип 1
                    val[0] = 1;

                if (val[0] == 0)
                {
                    xrefTable[i].free = true; // свободный объект
                    xrefTable[i].generation = val[2]; // номер поколения
                }
                if (val[0] == 1)
                {
                    xrefTable[i].compressed = false; // не сжатый
                    xrefTable[i].offset = val[1]; // смещение
                    xrefTable[i].generation = val[2]; // номер поколения
                }
                if (val[0] == 2)
                {
                    xrefTable[i].compressed = true; //сжатый
                    xrefTable[i].streamNum = val[1]; // номер потокового объекта, где хранится объект
                    xrefTable[i].streamIndex = val[2]; //индекс объекта внутри потока
                    xrefTable[i].generation = 0; // номер поколения по умолчанию 0
                }
		Console.WriteLine($"num = {i} free = {xrefTable[i].free} comp = {xrefTable[i].compressed} ofs = {xrefTable[i].offset} gen = {xrefTable[i].generation} stream = {xrefTable[i].streamNum} index = {xrefTable[i].streamIndex}");
            }
	    ReadTrailer(dict);
        }

	/// <summary>
        /// Метод чтения таблицы ссылок
        /// читает таблицу ссылок, позиция в файле уже установлена в ее начало, массив таблицы нужно создать
        /// используется объект Parser для чтения данных #11
        /// запоминаем позицию потока и читаем лексему.
        /// если это число, то возвращаем позицию потока назад и вызываем чтение потока ссылок #36
        /// таблица ссылок содержит по одной записи для каждого косвенного объекта #25
        /// часть или все из этих записей могут содержаться в потоках ссылок #31
        /// таблица состоит из одной или более секций.
        /// таблица начинается с ключевого слова xref, после чего идут секции
        /// каждая секция состоит из последовательного диапазона объектов
        /// секция начинается с двух чисел: номер первого объекта в секции и количество объектов в секции.
        /// Например: 28 5
        /// значит в секции будут объекты: от 28 до 32
        /// объекты в секциях не пересекаются
        /// далее идут записи, каждая на новой строке:
        /// число1 число2 (n или f)(символ)
        /// число1 - смещение объекта в файле
        /// число2 - номер поколения
        /// n - значит запись используется,
        /// f - запись свободная(ее запоминать в таблицу не нужно)
        /// если в таблице ссылок запись не пустая(смещение не равно 0), то пропускаем этот объект!
        /// (в файле могут быть несколько таблиц ссылок, сначала читаются новые таблицы)
        /// после таблицы ссылок должен идти trailer, читаем его #33
        /// общий пример таблицы(содержит 4 занятых и два свободных объекта):
        /// xref
        /// 0 6
        /// 0000000003 65535 f
        /// 0000000017 00000 n
        /// 0000000081 00000 n
        /// 0000000000 00007 f
        /// 0000000331 00000 n
        /// 0000000409 00000 n
        /// пример из нескольких секций
        /// xref
        /// 0 1
        /// 0000000000 65535 f
        /// 3 1
        /// 0000025325 00000 n
        /// 23 2
        /// 0000025518 00002 n
        /// 0000025635 00000 n
        /// 30 1
        /// 0000025777 00000 n
        /// </summary>
        public static void ReadCrossReferenceTable()
        {
            long position;
            position = stream.Position; //запоминаем позицию потока
            object o = parser.ReadToken();
            //Console.WriteLine(o.ToString());
            if (o is int) 
            {
                stream.Seek(position, SeekOrigin.Begin); //возвращаем позицию потока назад
		parser.NextChar();
                LoadXRefStream(); //вызываем чтение потока ссылок #36
                return;
            }
            while (true)
            {
                o = parser.ReadToken();
                if (o is char && (char)o == '\uffff')
                    break;
//                Console.WriteLine(o.ToString());
                if (o is string && (string)o == "trailer")
                    break;
                int first = (int)o; //читаем номер первого объекта
  //              Console.WriteLine($"first = {first}");
                int count = (int)parser.ReadToken(); //читаем количество объектов
    //            Console.WriteLine($"count = {count}");
                if (xrefTable == null)
                    xrefTable = new XRefEntry[first + count];
                else
                    Array.Resize(ref xrefTable, first + count);
                for (int i = 0; i < count; i++)
                {
                    int index = first + i;
                    int ofs = (int)parser.ReadToken(); // читаем номер смещения объекта в файле
                  //  Console.WriteLine($"ofs = {ofs}");
                    int gen = (int)parser.ReadToken(); // читаем номер поколения
                //    Console.WriteLine($"gen = {gen}");
                    string s = (string)parser.ReadToken(); //читаем n или f
              //      Console.WriteLine($"n or f: {s}");
                    if (s == "n" && xrefTable[index].offset == 0)
                    {
                        xrefTable[index].offset = ofs;
                        xrefTable[index].generation = gen;
                        xrefTable[index].free = false;
                    }
                    else if (s == "f")
                        xrefTable[index].free = true;
                }
            }
            ReadTrailer((Dictionary<string, object>)parser.ReadToken());
        }

	/// <summary>
        /// Читает структуру PDF файла
        /// </summary>
        /// <param name="s"> Поток файла </param>
        public static void Open(Stream s)
        {
            stream = s;
            parser = new Parser(stream);
            objectCache = new Dictionary<int, object>();
	    root = null;
	    info = null;
            ReadHeader();

            string line = "";
            int count = -8;

            while (true)
            {
                s.Seek(count, SeekOrigin.End);
                int bt = s.ReadByte();
                string str = System.Text.Encoding.Default.GetString(BitConverter.GetBytes(bt));
                if (int.TryParse(str, out bt))
                    line += bt.ToString();
                else
                    break;
                count--;
            }
            int xrefOffset = int.Parse(new String(line.Reverse().ToArray()));
	    //Console.WriteLine($"xref offset = {xrefOffset}");
            stream.Seek(xrefOffset, SeekOrigin.Begin);
	    parser.NextChar();
            ReadCrossReferenceTable();
        }

	/// <summary>
        /// Читает сжатый объект из потока, содержащего объекты
        /// читаем потоковый объект #35
        /// в считанном словаре могут быть дополнительные поля
        /// Type - обязательно должно равняться ObjStm
        /// N - число объектов в этом потоке
        /// First - смещение в байтах внутри потока, где находится первый объект
        /// Extends(необязательно) - ссылка на продолжение потока
        /// считанный поток открываем как MemoryStream и читаем с помощью парсера (#11)
        /// в начале идут N пар целых чисел, первое число в паре - номер объекта,
	/// второе - смещение в этом потоке, относительно поля First.все смещения идут по возрастанию, номера объектов - в произвольном порядке.
        /// сами объекты записаны последовательно в виде данных, без ключевых слов obj и endobj
        /// переходим по смещению, считываем объект #11 и записываем в кеш #35
        /// если есть Extends, то вызываем рекурсивно чтение объекта из потока объектов по заданной ссылке (Tuple<int, int>)
        /// 15 0 obj % The object stream
        //<< /Type /ObjStm
        ///Length 1856
        ///N 3 % The number of objects in the stream
        ///First 19 % The byte offset in the decoded stream of the first object
        //% The object numbers and offsets of the objects, relative to the first are shown on the first line of
        //% the stream(i.e., 11 0 12 547 13 665).
        //>>
        //stream
        //11 0 12 547 13 665
        //<< /Type /Font
        ///Subtype /TrueType
        ///FontDescriptor 12 0 R
        //>>
        //<< /Type /FontDescriptor
        ///Ascent 891
        ///FontFile2 22 0 R
        //>>
        //<< /Type /Font
        ///Subtype /Type0
        ///ToUnicode 10 0 R
        //>>
        //endstream
        //endobj
        /// </summary>
        /// <param name="streamNum"> номер объекта, содержащего поток объектов </param>
        /// <param name="index"> индекc объекта в потоке </param>
        /// <returns> возвращает прочитанный объект </returns>
        public static object ReadObjectFromStream(int streamNum,int index)
	{
            Dictionary<string, object> dict;
            byte[] bstream = (byte[])GetObject(streamNum, out  dict);
            if (dict.ContainsKey("Type") && (string)dict["Type"] != "ObjStm")
                throw new Exception("Поле Type не является ObjStm");
            if((long)dict["N"] > index && !dict.ContainsKey("Extends"))
                throw new Exception("Избыточные элементы потока не содержатся в поле Extends");
            if ((long)dict["N"] > index)
            {
                Stream stream = new MemoryStream(bstream);
                parser = new Parser(stream);
                parser.NextChar();
                int num = 0;
                int ofs = 0;
                for (int i = 0; i <= index; i++)
                {
                    num = (int)parser.ReadToken();
                    ofs = (int)parser.ReadToken();
                }
                stream.Seek(ofs + (long)dict["First"], SeekOrigin.Begin);
		parser.NextChar();
                object obj = parser.ReadToken();
                objectCache.Add(num, obj);
                return obj;
            }
            else
            {
                Tuple<int, int> t = (Tuple<int, int>)dict["Extends"];
                return ReadObjectFromStream(t.Item1, (int)((long)dict["N"] - index));
            }
        }

	/// <summary>
        /// загружает объект страницы
        /// загрузить каталог по ссылке root, все объекты загружаем с помощью #35
        /// проверяем тип объекта Type - Catalog
        /// загружаем объект по полю Pages(по ссылке)
        /// каждый узел начиная с Pages содержит поля
        /// Kids - массив ссылок на дочерние элементы
        /// Count - количество дочерних элементов
        /// нужно обойти массив чтобы отсчитать объект по номеру страницы
        /// если дочерний объект - узел(Type = Pages) тогда обходим узел
        /// конечные узлы - Type = Page
        /// нужен вспомогательный метод
        /// </summary>
        /// <param name="num">номер страницы, начиная с 1</param>
        /// <returns>страницу указанного номера</returns>
        public static Dictionary<string, object> GetPage(int num)
        {
            Dictionary<string, object> page;
            int count;
            object[] kids;
            Dictionary<string, object> catalog;
            Dictionary<string, object> pages;
            GetObject(root.Item1, out catalog);

            if (catalog["Type"].ToString() != "Catalog")
                throw new Exception("тип каталога не равен Catalog");

            Tuple<int, int> tupPages = (Tuple<int, int>)catalog["Pages"];
            
            GetObject(tupPages.Item1, out pages);
            kids = (object[])pages["Kids"];
            count = (int)pages["Count"];
            if (count < num)
                throw new Exception("Номер страницы превышает количество страниц документа");
            GetObject(((Tuple<int, int>)kids[num - 1]).Item1, out page);       
            
            return page;
        }        

        /// <summary>
        /// вспомогательный метод для GetPage
        /// </summary>
        /// <param name="num">номер страницы</param>
        /// <param name="node">словарь узла</param>
        /// <returns></returns>
        private static object GetNode(int num, Dictionary<string, object> node)
        {
            
            object[] kids = (object[])node["Kids"];
            object page = null;
            Dictionary<string, object> tempPages;
            
                if ((int)node["Count"] >= num)
                {
                    for (int i = 0; i < (int)node["Count"]; i++)
                    {
                        GetObject((int)kids[i], out tempPages);
                        if (tempPages["Type"].ToString() == "Pages")
                        {
                            page = GetNode(num, tempPages);
                        }
                        else if (tempPages["Type"].ToString() == "Page")
                        {
                            if (i + 1 == num)
                                page = tempPages;
                        }
                    }
                }
            
            return page;        
        }	
    }
}
