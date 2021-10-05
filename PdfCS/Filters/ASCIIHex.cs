using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PdfCS
{
    /// <summary>
    ///   Класс декодирования двоичных байт как ASCII символов
    /// </summary>
    public class ASCIIHex
    {
	/// <summary>
	///   Декодирует последовательность ASCII
	/// каждые два байта образуют пару из символов 0-9, a-f, A-F
	/// все пустые символы игнорируются
	/// символ '>' означает конец данных
	/// любой другой символ означает ошибку - выбрасывание исключения
	/// если конец данных встретился вместо четного символа, то берется значение 0 и последний знак
	/// </summary>
        /// <param name="stream">Массив закодированных байт</param>
        /// <returns>
        /// декодированный массив байт
        /// </returns>
        public static byte[] Decode(byte[] stream)
        {
            string s = "";
            List<byte> bt = new List<byte>();
            for (int i = 0; i < stream.Length; i++)
            {
                char c = (char)stream[i];
                if (c == '>')
                    break;
                else if (Parser.IsWhitespace(c))
                    continue;
                else if (Char.IsDigit(c) || c >= 'a' && c <= 'f' || c >= 'A' && c <= 'F')
                    s += c;
                else
                    throw new Exception($"Неверный символ ASCIIHex: {c}");
                if (s.Length == 2)
                {
                    bt.Add(Convert.ToByte(s, 16));
                    s = "";
                }
            }
            if (s != "")
                bt.Add(Convert.ToByte(s, 16));
            return bt.ToArray();
        }
    }
}
