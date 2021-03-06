using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfCS
{
    /// <summary>
    ///   Класс декодирования ASCII85
    /// </summary>
    public class ASCII85
    {
	/// <summary>
	///   Степени основания 85
	/// </summary>
	private static int[] powers = new int[] {1, 85, 85 * 85, 85 * 85 * 85, 85 * 85 * 85 * 85}; 

	private static List<byte> DecodeSequence(List<byte> c)
	{
	    int count = c.Count;
	    long b = 0;
	    List<byte> output = new List<byte>();

	    for (int i = 0; i < 5 - count; i++)
		c.Add((byte)'u');
	    for (int i = 0; i < c.Count; i++)
		b += (long)(c[i] * powers[4 - i]);
	    if (b >= (long)0xffffffff)
		throw new Exception("Максимальное значение ASCII85");							  
	    for (int i = 0; i < count - 1; i++)
		output.Add((byte)((b >> (3 - i) * 8) & 0xff));
	    return output;
	}

	/// <summary>
	///   Декодирует последовательность, закодированную ASCII85
	///  конец данных - символы '~' и '>'
	/// все пустые символы игнорируются
	/// каждые 5 символов(c1 c2 c3 c4 c5) выдают 4 байта данных (b1 b2 b3 b4)
	/// соотношение:
	/// ( b 1 × 256^3 ) + ( b 2 × 256^2 ) + ( b 3 × 256 ) + b 4 =
	/// ( c 1 × 85^4 ) + ( c 2 × 85^3 ) + ( c 3 × 85^2 ) + ( c 4 × 85^1 ) + c5
	/// при декодировании из каждого символа вычитается символ '!'
	/// символ 'z' распаковывается в 5 байт со значением 0
	/// 'z' не может находиться в середине группы из 5
	/// последних символов может быть меньше 5, но не меньше 2
	/// </summary>
        /// <param name="stream">Массив закодированных байт</param>
        /// <returns>
        /// декодированный массив байт
        /// </returns>
        public static byte[] Decode(byte[] stream)
        {
	     // список раскодированных байтов
            List<byte> output = new List<byte>();
	    List<byte> c = new List<byte>();
	    byte[] zero = new byte[4];
            int pos = 0; 

            while (true)
            {
		if (pos >= stream.Length)
		    throw new Exception("Отсутствует конец данных ~> в ASCII85Decode");
		byte b = stream[pos++];
		if (b == '~' && pos  < stream.Length)
		{
		    if (stream[pos] == '>')
			break;
		    else
			throw new Exception("Неверный конец данных ~> в ASCII85Decode");
		}
		else if (Parser.IsWhitespace((char)b))
		    continue;
		else if (b == 'z')
		    output.AddRange(zero);
		else if (b < '!' || b > 'u')
		    throw new Exception("Неверный символ в ASCII85Decode");
		else
		    c.Add((byte)(b - '!'));
		if (c.Count == 5)
		{
		    output.AddRange(DecodeSequence(c));
		    c.Clear();
		}
	    }
	    output.AddRange(DecodeSequence(c));
            return output.ToArray();
        }
    }
}
