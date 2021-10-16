﻿using System;
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
        /// Очередь токенов метода ReadToken
        /// </summary>
        private Queue<object> tokens;
	    
	/// <summary>
	///   Конструктор класса
	/// </summary>
        /// <param name="s">Поток, откуда будут считываться символы</param>
        public Parser(Stream s)
        {
            stream = s;
	    tokens = new Queue<object>();
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

	/// <summary>
	///   Читает строку из 0 или более символов внутри круглых скобок
	/// первый символ '(' уже прочитан
	/// Строка заключена в скобки, может содержать внутри любые символы, в том числе сбалансированные скобки:
	/// (This is a string)
	/// (Strings may contain newlines
	/// and such.)
	/// (Strings may contain balanced parentheses ( ) and
	/// special characters ( * ! & } ^ % and so on ).)
	/// (The following is an empty string.) ()
	/// (It has zero (0) length.)
	/// Если встречается символ '\', то необходимо обработать следующий символ
	/// считаются одним символом следующие последовательности: \n\r\t\b\f()\\
	/// \ddd, где ddd - восьмиричный код символа
	/// Если после '\' идет перевод строки, то он исключается из результирующей строки
	/// (These
	/// two strings
	/// are the same.)
	/// (These two strings are the same.)
	/// Если в строке встречается перевод строки, то он записывается в результирующую строку как '\n' (убирая '\r' если есть)
	/// (This string has an end-of-line at the end of it.
	/// )
	/// (So does this one .\n)
	/// Восьмиричная последовательность кодирует непечатаемые символы
	/// (This string contains \245two octal characters\307.)
	/// Эта последовательность может состоять из одного, двух или трех цифр, больше не должны учитываться.
	/// Пустые старшие места заполняются нулями.
	/// the literal (\0053)
	/// denotes a string containing two characters, \005 (Control-E) followed by the digit 3, whereas both
	/// (\053) and (\53) denote strings containing the single character \053, a plus sign (+).
	/// </summary>
        /// <returns>
        /// полученная строка в виде массива символов
        /// </returns>
	public char[] ReadString()
        {
            string charArray = "";
            int count = 1;
            string number = "";
            string tabIn = "nrtfavb\\";
            string tabOut = "\n\r\t\f\a\v\b\\";
            while (count > 0)
            {        
                NextChar();
                if (lastChar == '\uffff') throw new Exception("Ошибка, конец файла внутри строки");
                else if (lastChar == '(')
                    count++;
                else if (lastChar == ')')
                    count--;
                else if (lastChar == '\\')
                {
                    NextChar();
                    if(Parser.IsWhitespace(lastChar))
                        SkipEndOfLine();
                    else if (tabIn.Contains(lastChar))
                        lastChar = tabOut[tabIn.IndexOf(lastChar)];
                    else if (lastChar >= '0' && lastChar <= '7')
                    {
                        number = "";                     
                        for (int i = 0; i < 3; i++)
                        {
                            if (lastChar >= '0' && lastChar <= '7')
                                number += lastChar;
                            else
                                break;
                            NextChar();
                        }
                        charArray += Convert.ToInt32(number, 8);
                    }
                }
                charArray += lastChar;
            }
            return charArray.Remove(charArray.Length - 1).ToCharArray();
        }

	/// <summary>
        /// Считывает токены из строки.
        /// в зависимости от текущего символа lastChar вызывает соответствующий метод чтения объекта
        /// если конец потока, то вызывает исключение "Конец потока"
        /// если прочитано число, то оно заносится в очередь и читается следующий токен
        /// если он также число, то вновь заносится в очередь и читается еще токен.Если последний равен ключевому слову "R",
        /// то возвращается объект Tuple<int, int> из двух чисел
        /// Если последний токен не R, то возвращается объект из очереди
        /// </summary>
        /// <returns>
        /// </returns>
        public object ReadToken()
        {
            if (tokens.Count != 0)
                return tokens.Dequeue();
            SkipWhitespace();
            if (lastChar == '\uffff')
                throw new Exception("Конец потока");
            if (lastChar == '+' || lastChar == '-' || lastChar == '.' || Char.IsDigit(lastChar))
            {
                object o = ReadNumber();
                if (o is int)
                {
                    object o2 = ReadToken();
                    if (o2 is int)
                    {
                        object o3 = ReadToken();
                        if (o3 is string && (string)o3 == "R")
                            return Tuple.Create(o, o2);
                        else
                        {
                            tokens.Enqueue(o);
                            tokens.Enqueue(o2);
                            tokens.Enqueue(o3);
                            return tokens.Dequeue();
			}
                    }
                    else
                    {
                        tokens.Enqueue(o);
                        tokens.Enqueue(o2);
                        return tokens.Dequeue();
                    }
                }
                else
                    return o;
            }
            else if (lastChar == '<')
            {
                NextChar();
                if (lastChar == '<')
                    return ReadHex();
                else
                    return ReadDictionary();
            }
            else if (lastChar == '>')
            {
                NextChar();
                if (lastChar == '>')
                    return ">>";
                else
                    throw new Exception("ReadToken встретилась скобка >");
            }
            else if (lastChar == '[')
                return ReadArray();
            else if (lastChar == ']')
                return ']';
            else if (lastChar == 't' || lastChar == 'f')
                return ReadBoolean();
            else if (lastChar == '/')
                return ReadNameObject();
            else if (lastChar == 'n')
                return ReadNull();
            else
                return ReadKeyword();
        }

	public object ReadDictionary() 
        {
            return null;
        }

        public object ReadArray() 
        {
            return null;
        }
    }
}
