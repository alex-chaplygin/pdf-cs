using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace PdfCS
{
    /// <summary>
    ///   Лексический анализатор
    /// </summary>
    public class Lexer
    {
        /// <summary>
        ///   Поток, откуда считываются символы
        /// </summary>
        private Stream stream;

        /// <summary>
        ///   Последний прочитанный символ
        /// </summary>
        private char lastChar;

        public Lexer(Stream s)
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
            while (lastChar == '%' || Lexer.IsWhitespace(lastChar))
            {
                if (lastChar == '%')
                    SkipComment();
                NextChar();
            }
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
            if (str.IndexOf(".") < 0)
                if (int.TryParse(str, out resi))
                    return resi;
                else
                    throw new Exception("Ошибка в целом числе");
            else if (double.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out resd))
                return resd;
            else
                throw new Exception("Ошибка в вещественном числе");
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
            string s = lastChar.ToString();

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
            NextChar();
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
            f: if (lastChar == '\uffff') throw new Exception("Ошибка, конец файла внутри строки");
                else if (lastChar == '(')
                    count++;
                else if (lastChar == ')')
                    count--;
                else if (lastChar == '\r')
                    continue;
                else if (lastChar == '\\')
                {
                    NextChar();
                    if (tabIn.Contains(lastChar))
                        lastChar = tabOut[tabIn.IndexOf(lastChar)];
                    else if (Lexer.IsWhitespace(lastChar))
                        continue;
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
                        charArray += (char)Convert.ToByte(number, 8);
                        goto f;
                    }
                }
                charArray += lastChar;
            }
            NextChar();
            return charArray.Remove(charArray.Length - 1).ToCharArray();
        }

        /// <summary>
        ///   Читает из потока ключевое слово до пустого символа или разделителя (IsWhitespace, IsDelimeter)
        ///  первый символ уже прочитан
        /// </summary>
        /// <returns>
        /// строка ключевого слова, например "stream"
        /// </returns>
        public string ReadId()
        {
            string res = lastChar.ToString();
            while (lastChar != '\uffff' && !Lexer.IsWhitespace(lastChar) &&
               !Lexer.IsDelimeter(lastChar))
                res += NextChar();
            NextChar();
            return res.Substring(0, res.Length - 1);
        }

        /// <summary>
        /// Считывает очередную лексему из потока.
        /// в зависимости от текущего символа lastChar вызывает соответствующий метод чтения объекта
        /// если конец потока, то вызывает исключение "Конец потока"
        /// если прочитано число, то оно заносится в очередь и читается следующий токен
        /// если он также число, то вновь заносится в очередь и читается еще токен.Если последний равен ключевому слову "R",
        /// то возвращается объект Tuple<int, int> из двух чисел
        /// Если последний токен не R, то возвращается объект из очереди
        /// </summary>
        /// <returns>
        /// текущий объект
        /// </returns>
        public object ReadToken()
        {
            SkipWhitespace();
            if (lastChar == '\uffff')
                return '\uffff';
            if (lastChar == '+' || lastChar == '-' || lastChar == '.' || Char.IsDigit(lastChar))
		return ReadNumber();
            else if (lastChar == '(')
                return ReadString();
	    else if (lastChar == '/')
		return ReadNameObject();
            else if (lastChar == '<')
            {
                NextChar();
                if (lastChar != '<')
                    return ReadHex();
                else
                    return "<<";
            }
            else if (lastChar == '>')
            {
                NextChar();
                if (lastChar == '>')
                {
                    NextChar();
                    return ">>";
                }
                else
                    throw new Exception("ReadToken встретилась скобка >");
            }
            else if (Lexer.IsDelimeter(lastChar))
            {
		char c = lastChar;
		NextChar();
		return c;
            }
            else
                return ReadId();
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
        /// объект класса NameObject
        /// </returns>
        public NameObject ReadNameObject()
        {
            string res = NextChar().ToString();
            while (lastChar != '\uffff')
            {
                if (lastChar == '\x00')
                    throw new Exception("В имени содержится неверный символ '0'");
                else if (Lexer.IsWhitespace(lastChar) || Lexer.IsDelimeter(lastChar))
                    break;
                else if (lastChar == '#')
                {
                    res = res.Remove(res.Length - 1, 1);
                    res += (char)Convert.ToByte(NextChar().ToString() + NextChar().ToString(), 16);
                }
                else
                    res += NextChar();
            }
            return new NameObject(res.Substring(0, res.Length - 1));
        }
    }
}
