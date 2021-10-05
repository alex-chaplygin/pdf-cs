using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PdfCS
{
    /// <summary>
    ///   Класс синтаксического разбора объектов PDF
    /// </summary>
    public class Parser
    {
	/// <summary>
	///   Поток, откуда считываются символы
	/// </summary>
        private Stream stream;

	/// <summary>
	///   Последний прочитанный символ
	/// </summary>
        private char lastChar;

	/// <summary>
	///   Конструктор класса
	/// </summary>
        /// <param name="s">Поток, откуда будут считываться символы</param>
        public Parser(Stream s)
        {
            stream = s;
        }

	/// <summary>
	///   Считывает следующий символ из потока
	/// </summary>
        /// <returns>
        /// Прочитанный символ
        /// </returns>
        public char NextChar()
        {
            lastChar = (char)stream.ReadByte();
            return lastChar;
        }

	/// <summary>
	///   Проверяет является ли символ пустым
	/// </summary>
        /// <param name="c">Символ</param>
        /// <returns>
        /// true - если символ пустой, false - иначе
        /// </returns>
        public static bool IsWhitespace(char c)
        {
            string w = "\x00\t\r\n \x0c";

            return w.IndexOf(c) != -1;
        }

	/// <summary>
	///   Проверяет является ли символ разделителем
	/// </summary>
        /// <param name="c">Символ</param>
        /// <returns>
        /// true - если символ является разделителем, false - иначе
        /// </returns>
        public static bool IsDelimeter(char c)
        {
            string d = "()<>{}[]/%";

            return d.IndexOf(c) != -1;
        }

	/// <summary>
	///   Пропускает комментарий
	/// </summary>
        private void SkipComment()
        {
            while (NextChar() != '\n') ;
        }

	/// <summary>
	///   Пропускает все последовательные пустые символы и комментарии
	/// </summary>
        private void SkipWhitespace()
        {
            while (lastChar == '%' || Parser.IsWhitespace(lastChar))
	    {
                if (lastChar == '%')
		    SkipComment();
                NextChar();
            }
        }

	/// <summary>
	///   Читает пустой объект null
	///  если ошибка в слове, то вызывает исключение "Ошибка в null"
	/// </summary>
        /// <returns>
        /// null
        /// </returns>
	public object ReadNull()
        {
            string f = "";

            for (int i = 0; i < 3; i++)
                f += NextChar();
            if (f != "ull")
                throw new  Exception("Ошибка в null");
            return null;
        }

	/// <summary>
	///   Читает логический объект
	/// первый символ прочитан ('t' или 'f')
	/// читает посимвольно слова "true" или "false"
	/// возвращает соответственно значения true или false
	/// если в словах ошибка, то выбрасывает исключение "Ошибка Boolean"
	/// </summary>
        /// <returns>
        /// true или false в зависимости от прочитанного слова
        /// </returns>
	public bool ReadBoolean()
        {
	    // Случай true
            if (lastChar == 't')
            {
                string _true = "true";
                for (int i = 1; i < 5; i++)
                    if (lastChar == _true[i - 1])
                        NextChar();
                    else
                        throw new Exception("Ошибка true Boolean");
                return true;
            }
            else if (lastChar == 'f')
            {
                string _false = "false";
                for (int i = 1; i < 6; i++)
                    if (lastChar == _false[i - 1])
			NextChar();
                    else
                        throw new Exception("Ошибка false Boolean");
                return false;
            }
            else
		throw new Exception("Неверный Boolean");
        }

	/// <summary>
	///   Пропускает конец строки.
	///  Конец строки может быть один символ '\n', или два символа '\r' и '\n'
	///  первый символ уже прочитан и находится в lastChar
	///  пропускает до первого символа (не '\n' и '\r')
	/// </summary>
	public void SkipEndOfLine()
        {
            if (lastChar == '\n')
                NextChar();
            else if (lastChar == '\r')
            {
                if (NextChar() != '\n')
                    throw new Exception("Ошибка при переводе строки");
                NextChar();
            }
        }

	/// <summary>
	///   Читает из потока ключевое слово до пустого символа или разделителя (IsWhitespace, IsDelimeter)
	///  первый символ уже прочитан
	/// </summary>
        /// <returns>
        /// строка ключевого слова, например "stream"
        /// </returns>
	public string ReadKeyword()
	{
	    string res = lastChar.ToString();
	    while (lastChar != '\uffff' && !Parser.IsWhitespace(lastChar) &&
		   !Parser.IsDelimeter(lastChar))
		res += NextChar();
	    return res.Substring(0, res.Length - 1);
	}

	/// <summary>
	///   Читает целое или вещественное число
	///  Первый символ прочитан ('+' '-' '.' или цифра)
	///  Целые числа: 123
	/// 43445
	/// +17
	/// -98
	/// 0
	/// Вещественные числа:
	/// 34.5
	/// -3.62
	/// +123.6
	/// 4.
	/// -.002
	/// 0.0
	/// .023
	/// если неверные символы в числе вызывает исключение
	/// </summary>
        /// <returns>
        /// целое или вещественное число, упакованное в object
        /// </returns>
	public object ReadNumber()
        {
            string str = "";
            int resi;
            double resd;
            
            while (lastChar != '\uffff' && 
                (lastChar == '+' || lastChar == '-' || lastChar == '.' || Char.IsDigit(lastChar)))
            {
                str += lastChar;
                NextChar();
            }
            str = str.Replace('.', ',');
            if (str.IndexOf(",") < 0)
                if (int.TryParse(str, out resi))
                    return resi;
                else
                    throw new Exception("Ошибка в числе: " + str);
            else if (double.TryParse(str, out resd))
                return resd;
            else
                throw new Exception("Ошибка в числе: " + str);
        }

	/// <summary>
	///   Читает имя, которое начинается с символа '/'
	/// Символ '/' уже прочитан, он не является частью имени
	/// Имя может состоять из любых символов кроме '\x00'
	/// Имя заканчивается символом разделителем(IsDelimeter) или пустым(IsWhitespace)
	/// Последовательность #dd интерпретируется как символ с кодом dd
	/// Одиночный символ '#' не может встречаться (только как #23 со своим кодом)
	/// Возможно пустое имя, когда нет символов до разделителя или пустого символа
	/// Примеры имен
	/// /Name1 Name1
	/// /ASomewhatLongerName ASomewhatLongerName
	/// /A;Name_With-VariousCharacters? A;Name_With-VariousCharacters?
	/// /1.2 1.2
	/// /$$ $$
	/// /@pattern @pattern
	/// /.notdef .notdef
	/// /lime#20Green Lime Green
	/// /paired#28#29parentheses paired( )parentheses
	/// /The_Key_of_F#23_Minor The_Key_of_F#_Minor
	/// /A#42 AB
	/// </summary>
        /// <returns>
        /// строка объекта имени
        /// </returns>
	public string ReadNameObject()
        {
            string res = NextChar().ToString();
            while (lastChar != '\uffff')
            {
                if (lastChar == '\x00')
                    throw new Exception("В имени содержится неверный символ '0'");
                else if (Parser.IsWhitespace(lastChar) || Parser.IsDelimeter(lastChar))
                    break;
		else if (lastChar == '#')
		    res += (char)Convert.ToByte(NextChar().ToString() + NextChar().ToString(), 16);
		else
		    res += NextChar();
            }
            return res.Substring(0, res.Length - 1); ;
        }

	/// <summary>
	///   Читает шестнадцатиричную строку.
	/// Первый символ '<' уже прочитан
	/// Шестнадцатиричные значения состоят из цифр 0-9 и букв a-f или A-F
	/// <4E6F762073686D6F7A206B6120706F702E>
	/// Каждая пара определяет один байт
	/// Если последний символ отсутствует (нечетное число символов), то вместо него в результат подставляется 0
	/// <901FA3> - 0x90, 0x1f, 0xa3
	/// <901FA> - 0x90, 0x1f, 0xa0
	/// </summary>
        /// <returns>
        /// массив прочитанных байт
        /// </returns>
	public byte[] ReadHex()
	{
	    List<byte> bytes = new List<byte>();
	    string s = "";

	    while (lastChar != '\uffff')
	    {
		if (NextChar() == '>')
		    break;
		else if (Char.IsDigit(lastChar) || lastChar >= 'a' && lastChar <= 'f' ||
		    lastChar >= 'A' && lastChar <= 'F')
		    s += lastChar;
		else
		    throw new Exception("Неверный символ в ReadHex: " + lastChar);
		if (s.Length == 2)
		{
		    bytes.Add(Convert.ToByte(s, 16));
		    s = "";
		}
	    }
	    if (lastChar == '\uffff')
		throw new Exception("Нет скобки в ReadHex");
	    if (s != "")
		bytes.Add(Convert.ToByte(s + "0", 16));
	    return bytes.ToArray();
	}
    }
}
