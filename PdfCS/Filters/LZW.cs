using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfCS
{
    /// <summary>
    ///   Класс распаковки методом LZW
    /// </summary>
    public class LZW
    {
	/// <summary>
	///   минимальная длина кода
	/// </summary>
        private const int minBitsCount = 9;

	/// <summary>
	///   максимальная длина кода
	/// </summary>
        private const int maxBitsCount = 12;

	/// <summary>
	///   код очистки таблицы кодов
	/// </summary>
        private const int clear = 256;

	/// <summary>
	///   код конца данных
	/// </summary>
        private const int EOD   = 257;

	/// <summary>
	///   текущее число бит
	/// </summary>
        private static int bitsCount;

	/// <summary>
	///   текущий размер таблицы кодов
	/// </summary>
        private static int decodeTableSize;

	/// <summary>
	///   таблица кодов
	/// </summary>
        private static string[] decodeTable;

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

	/// <summary>
	///   раннее изменение размера кода
	/// </summary>
	private static bool early;

        private static List<byte> ConvertFromString(string s)
        {
            List<byte> res = new List<byte>();

            for (int i = 0; i < s.Length; i++)
                res.Add((byte)s[i]);

            return res;
        }

        private static void InitTable()
        {
            decodeTable = new string[1 << maxBitsCount];
            decodeTableSize = 0;
            bitsCount = minBitsCount;

            for (int i = 0; i <= EOD; i++)
                decodeTable[decodeTableSize++] = ((char)i).ToString();
        }

        private static int NextCode(BitReader bR)
        {
            if ((!early && decodeTableSize >= (1 << bitsCount))
		|| (early && decodeTableSize >= (1 << bitsCount) - 1))
                bitsCount++;
            if (bitsCount > maxBitsCount)
	    {
                bitsCount = minBitsCount;
		decodeTableSize = EOD + 1;
	    }
            return bR.GetBitsMS(bitsCount);
        }

	private static void InitParams(Dictionary<string, object> p)
	{
	    predictor = p.ContainsKey("Predictor") ? (int)p["Predictor"] : 1;
	    colors = p.ContainsKey("Colors") ? (int)p["Colors"] : 1;
	    bpp = p.ContainsKey("BitsPerComponent") ? (int)p["BitsPerComponent"] : 8;
	    if (p.ContainsKey("Columns"))
		columns = (int)p["Columns"];
	    early = p.ContainsKey("EarlyChange") ? (int)p["EarlyChange"] == 1 : true;
	}

	/// <summary>
	///   Распаковывает поток, сжатый методом LZW
	///  использует параметры
	/// Параметры хранятся в словаре #28
	/// Возможные виды параметров:
	/// Predictor - код алгоритма предсказания #29
	/// если он равен 1, то алгоритм предсказания не вызывается. Значение по умолчанию: 1
	/// Colors (используется если Predictor > 1) - число перемешанных цветовых компонент в изображении (от 1 до 4). Значение по умолчанию: 1
	/// BitsPerComponent (используется если Predictor > 1) - число бит на один компонент цвета (1, 2, 4, 8, 16). Значение по умолчанию: 8
	/// Columns (используется если Predictor > 1) - число отсчетов в одной строке изображения
	/// EarlyChange - индикация, когда увеличивать длину кода.
	/// 0 - откладывать увеличение длины кода как можно дольше
	/// (длина кода увеличивается когда все значения данного кода заполнены),
	/// 1 - раннее увеличение длины кода (длина кода увеличивается перед записью последнего элемента). Значение по умолчанию: 1
	/// </summary>
        /// <param name="stream">Массив закодированных байт</param>
        /// <param name="params_">параметры кодирования</param>
        /// <returns>
        /// распакованный массив байт
        /// </returns>
        public static byte[] Decode(byte[] stream, Dictionary<string, object> params_ = null)
        {
            List<byte> res = new List<byte>();
            InitTable();
	    if (params_ != null)
		InitParams(params_);
            BitReader bR = new BitReader(stream);
            int code;
	    try
	    {
		while (true)
		{
		    code = NextCode(bR);
		    if (code == EOD)
			break;
		    else if (code == clear)
		    {
			bitsCount = minBitsCount;
			decodeTableSize = EOD + 1;
		    }
		    else if (code < decodeTableSize)
		    {
			decodeTable[decodeTableSize - 1] += decodeTable[code][0];
			res.AddRange(ConvertFromString(decodeTable[code]));
			decodeTable[decodeTableSize++] = decodeTable[code];
		    }
		    else
			throw new Exception("Некорректный код");
		}
	    }
	    catch (Exception e)
	    {
		throw new Exception("LZW Decode: " + e.Message);
	    }
            return Predictor.Decode(res.ToArray(), predictor, colors, bpp, columns);
        }
    }
}
