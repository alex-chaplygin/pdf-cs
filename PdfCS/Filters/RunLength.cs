using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PdfCS
{
    /// <summary>
    /// Класс декодирования длин серий  
    /// </summary>
    public class RunLength
    {
	/// <summary>
	///   код окончания данных
	/// </summary>
	private const int endOfData = 128;
	
	/// <summary>
	///   Дан массив байт, закодированных длин серий.
	/// Этот массив состоит из серий, каждая серия включает в себя байт длины.
	/// Если этот байт имеет значение от 0 до 127, то к длине прибавляется единица,
	/// и последующие байты в количестве равном длине копируются
	/// в выходной поток (список, который потом преобразуется в массив).
	/// Если байт длины имеет значение от 129 до 255, то вычисляется
	/// длина как 257 минус значение, и байт, который следует за длиной, копируется в поток числом раз равным длине.
	/// Если байт длины равен 128, то это значит данные завершились.
	/// </summary>
        /// <param name="array">Массив закодированных байт</param>
        /// <returns>
        /// распакованный массив байт
        /// </returns>
	public static byte[] Decode(byte[] array)
	{
	    List<byte> list = new List<byte>();
	    int pos = 0;
	    // цикл пока не конец массива или конец данных
	    while (pos < array.Length)
	    {
		// конец данных
		if (array[pos] == endOfData)
		    return list.ToArray();
		// копирование байт
		else if (array[pos] < endOfData)
		{
		    int len = array[pos++] + 1;
		    for (int i = 0; i < len; i++)
			list.Add(array[pos++]);
		}
		// распаковка (дублирование байта)
		else
		{
		    int len = 257 - array[pos++];
		    for (int i = 0; i < len; i++)
			list.Add(array[pos]);
		    pos += len;
		}
	    }
	    throw new Exception("RunLength - нет конца данных");
	}
    }
}
