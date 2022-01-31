using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace PdfCS
{
    /// <summary>
    ///   Класс синтаксического разбора объектов PDF
    /// </summary>
    public class Parser : Lexer
    {

	/// <summary>
	///   Конструктор класса
	/// </summary>
        /// <param name="s">Поток, откуда будут считываться символы</param>
        public Parser(Stream s) : base(s)
        { }


        /// <summary>
        /// Считывает очередную лексему из потока.
        /// вызывает base.ReadToken
        /// Он должен уметь распознавать "null" - возвращает null
        ///"true", "false" - true, false
        ///'[' - вызываем ReadArray
        ///"<<" - вызываем ReadDictionary
        /// </summary>
        /// <returns>
        /// текущий объект
        /// </returns>
        public object ReadToken()
        {

            object read = base.ReadToken();
            string readstring;
            char lastChar;
            if (read is int)
            {
                lastChar = this.lastChar;//(char)read;
                                         //read = ReadNumber();                   
                long pos = stream.Position;
                object o2 = base.ReadToken();
                object o3 = base.ReadToken();
                if (o2 is int && o3 is string && (string)o3 == "R")
                    return Tuple.Create((int)read, (int)o2);
                else
                {
                    stream.Seek(pos, SeekOrigin.Begin);
                    this.lastChar = lastChar;//NextChar();
                    return read;//lastChar;
                }
            }
            else if (read is char && (char)read == '[')
                return ReadArray();            
            else if (read is string)
            {
                readstring = (string)read;
                if (readstring == "true")
                    return true;
                else if (readstring == "false")
                    return false;
                else if (readstring == "null")
                    return null;
                else if (readstring == "<<")
                    return ReadDictionary();
                else
                    return readstring;

            }
            else return read;
        }

	/// <summary>
        /// Читает словарь, где присутствуют пары объектов: ключ-значение.
        /// Ключ - объект-имя (читается с помощью ReadNameObject), ключи все различные.
        /// Значение может быть любым объектом(читается с помощью ReadToken) в том числе словарем.
        /// Если null, то эта запись в значение не существует (не нужно добавлять).
        /// Словарь может иметь 0 записей.
        /// Открывающие скобки "<<" уже прочитаны.
	/// << /Type /Example
	/// /Subtype /DictionaryExample
	/// /Version 0 . 01
	/// /IntegerItem 12
	/// /StringItem ( a string )
	/// /Subdictionary << /Item1 0 . 4
	/// /Item2 true
	/// /LastItem ( not ! )
	/// /VeryLastItem ( OK )
	/// >>
	/// >>
        /// </summary>
        /// <returns>
        /// словарь объектов
        /// </returns>
	public object ReadDictionary() 
        {
	    Dictionary<string, object> dictionary = new Dictionary<string, object>();
	    while (true)
            {
                object key = ReadToken();
                if (key is char && (char)key == '\uffff')
                    break;
                if (key is string && key.ToString() == ">>")
                    return dictionary;
                object value = ReadToken();
                if (value != null)
                    dictionary.Add(key.ToString(), value);
            }
            throw new Exception("ReadDictionary завершился без закрывающих скобок");
        }

	/// <summary>
        /// читает массив из объектов, заключенных в квадратные скобки
        ///первый символ '[' уже прочитан
        ///Массив может содержать 0 элементов
        ///массив может включать другие массивы, поэтому для чтения объектов использовать ReadToken
        ///[549 3.14 false(Ralph) / SomeName]
        /// </summary>
	/// <returns>
	/// массив из значений объектов
	/// </returns>
        public object[] ReadArray()
        {
            List<object> list = new List<object>();
	        //NextChar();
            while (true)
            {
                object o = ReadToken();
		if (o is char && (char)o == '\uffff')
		    break;
                if ((o is char) && (char)o == ']')
                    return list.ToArray();
                else
                    list.Add(o);  
            }
            throw new Exception("ReadArray завершился без закрывающей скобки");           
        }

	/// <summary>
        /// читает объект PDF
        /// objNum - номер объекта(положительный)
        /// в новом созданном файле равен 0, увеличивается при обновлении файла.
        /// Эти два номера образуют пару для уникальной идентификации объекта.
        /// Пример объекта:
        /// 12 0 obj
        /// (Brillig)
        /// endobj
        /// Чтение происходит с помощью метода ReadToken #11
        /// Проверяет, что объект записан именно в таком виде.
        /// Если вместо endobj присутствует ключевое слово stream, тогда вызывается чтение потока #30
        /// Для потока сохраняем словарь в параметре dict
        /// </summary>
        /// <param name="dict">выходной параметр - словарь объекта(для потоков)</param>
        /// <returns> возвращает данные объекта </returns>
        public object ReadIndirectObject(out Dictionary<string, object> dict)
        {
            object value, obj;

	    dict = null;
            value = ReadToken();
            if (!(value is int))
                throw new Exception("Неверный номер объекта (objNum)");
            value = ReadToken();
            if (!(value is int))
                throw new Exception("Неверный номер поколения (genNum)");
            value = ReadToken();
            if (value is string && (string)value != "obj")
                throw new Exception("Ожидался параметр obj");            
            obj = ReadToken();
            value = ReadToken();
            if (value is string && (string)value == "stream")
            {
                dict = (Dictionary<string, object>)obj;
                value = ReadStream(dict);
                return value;
            }
            else if (value is string && (string)value == "endobj")
                return obj;
            else
                throw new Exception("Ожидался параметр endobj");
        }

	/// <summary>
        /// Читает поток из файла (последовательность байт)
        /// 
        /// Поток записан в файле так:
        /// словарь #28
        /// stream
        /// 0 или более байт
        /// endstream
        /// Ключевое слово stream уже прочитано
        /// После stream следует перевод строки, его пропускаем #2
        /// После чего следует поток байт до слова endstream, 
        /// в словаре указывается точное число байт,
        /// перед endstream также есть перевод строки, 
        /// который не включается в последовательность байт.
        /// Поток может содержаться во внешнем файле, 
        /// который указывается в словаре, 
        /// тогда все байты до endstream игнорируются.
        /// В словаре обязательно содержится 
        /// поле Length - число закодированных байт, не включая перевод строки.
        /// Если число данных не соответствует длине, должна выдаваться ошибка.
        /// Остальные поля могут быть или нет.
        /// Поле Filter содержит один (объект имя) или 
        /// последовательность фильтров(массив имен), 
        /// которые нужно применить в указанном порядке.
        /// Поле DecodeParams содержит словарь или массив параметров к фильтрам.
        /// Если фильтр один, то параметры - в виде словаря.Если фильтров много, 
        /// то указывается массив содержащий по одному значению 
        /// для фильтров: или null (если у фильтра нет параметров) или словарь параметров.
        /// Если для всех фильтров не нужны параметры, то этого поля может не быть.
        /// Поле F указывает имя файла с внешними данными, 
        /// поле Length хранит длину данных, 
        /// поле FFilter - фильтры, FDecodeParams - параметры.
        /// Фильтры (параметры)
        /// ASCIIHexDecode no
        /// ASCII85Decode no
        /// LZWDecode yes
        /// FlateDecode yes
        /// RunLengthDecode no
        /// CCITTFaxDecode yes
        /// JBIG2Decode yes
        /// DCTDecode yes
        /// JPXDecode no
        /// Crypt yes
        /// </summary>
        /// <param name="dict">данные из объекта (словарь)</param>
        /// <returns>массив байт, полученный после применения всех фильтров</returns>
        public byte[] ReadStream(Dictionary<string, object> dict)
        {
            object filters = null;
            object parameters = null;
            List<string> sfilters = new List<string>();
            List<Dictionary<string, object>> sparams = new List<Dictionary<string, object>>();

            SkipEndOfLine();
            if (!dict.ContainsKey("Length"))
                throw new Exception("Отсуствует длина потока");

            byte[] array = new byte[(int)dict["Length"]];

            if (dict.ContainsKey("F"))
                using (FileStream fileStream = File.OpenRead((string)dict["F"]))
                {
                    fileStream.Read(array, 0, array.Length);
                }
            else
            {
//                Console.WriteLine($"pos = {stream.Position} lastChar = {lastChar}");
		array[0] = (byte)lastChar;
                stream.Read(array, 1, array.Length - 1);
  //              Console.WriteLine($"array = {array[0]} {array.Length}");
                SkipEndOfLine();
    //            Console.WriteLine($"pos = {stream.Position}");
            }
            NextChar();
            if ((string)ReadToken() != "endstream")
                throw new Exception("Ожидался конец потока");

            if (dict.ContainsKey("F") && dict.ContainsKey("FFilter"))
                filters = dict["FFilter"];
            else if (dict.ContainsKey("Filter"))
                filters = dict["Filter"];
            if (filters != null && filters is object[])
                foreach (object o in (object[])filters)
                    sfilters.Add((string)o);
            else
                sfilters.Add((string)filters);
            if (dict.ContainsKey("F") && dict.ContainsKey("FDecodeParams"))
                parameters = dict["FDecodeParams"];
            else if (dict.ContainsKey("DecodeParams"))
                parameters = dict["DecodeParams"];
            if (parameters != null && parameters is object[])
                foreach (object o in (object[])parameters)
                    sparams.Add((Dictionary<string, object>)o);
            else
                sparams.Add((Dictionary<string, object>)parameters);
            array = ApplyFilter(array, sfilters, sparams);
            return array;
        }

        /// <summary>
        /// Применение фильтра (для метода ReadStream)
        /// 
        /// Пояснение.
        /// Если будет один фильтр цикл выполниться один раз с выбором нужного фильтра.
        /// Если фильтров много выполняться все, которые совпадут.
        /// Если фильтров нет, цикл не запуститься и вернет исходные данные.
        /// </summary>
        /// <param name="data">поток</param>
        /// <param name="filters">фильтр</param>
        /// <param name="p">словарь параметров</param>
        /// <returns>Отфильтрованный поток</returns>
        private static byte[] ApplyFilter(byte[] data, List<string> filters, List<Dictionary<string, object>> p)
        {
            if (filters == null)
                return data;
            for (int i = 0; i < filters.Count; i++)
                if (filters[i] == "ASCIIHexDecode")
                    data = ASCIIHex.Decode(data);
                else if (filters[i] == "ASCII85Decode")
                    data = ASCII85.Decode(data);
                else if (filters[i] == "LZWDecode")
                    data = LZW.Decode(data, p[i]);
                else if (filters[i] == "FlateDecode")
                    data = Flate.Decode(data, p[i]);
                else if (filters[i] == "RunLengthDecode")
                    data = RunLength.Decode(data);
                else if (filters[i] == "CCITTFaxDecode")
                    data = CCITTFax.Decode(data, p[i]);
                else if (filters[i] == "JBIG2Decode")
                    data = JBIG2.Decode(data, p[i]);
                else if (filters[i] == "DCTDecode")
                    data = DCT.Decode(data, p[i]);
                else if (filters[i] == "JPXDecode")
                    data = JPX.Decode(data);
                else if (filters[i] == "Crypt")
                    data = Crypt.Decode(data, p[i]);

            return data;
        }
    }
}
