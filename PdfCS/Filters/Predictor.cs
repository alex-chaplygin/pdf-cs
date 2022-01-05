using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfCS
{
    /// <summary>
    ///   Класс восстановления изображения по предсказанию
    /// </summary>
    public class Predictor
    {
	/// <summary>
	///   восстановление изображения по предсказанию
	/// Для лучшего сжатия изображений, в качестве значений используется разница между соседними
	/// отсчетами, вместо самих отсчетов.
	/// Если у изображения несколько компонент, то разница берется между текущим и предыдущим значением одного компонента.
	/// Виды предсказания:
	/// None - без предсказания
	/// Sub - предсказание для отсчета слева
	/// Up - предсказание для отсчета сверху
	/// Average - предсказание как среднее между отсчетом слева и сверху
	/// Paeth - нелинейная функция между отсчетами сверху, сверху-слева, сверху-справа
	/// Коды предсказания
	/// 1 - None
	/// 2 - для каждого цветового компонента берется разница с отсчетом слева (может быть внутри байта)
	/// PNG предсказания (>=10, конкретный алгоритм указан в данных)
	/// 10 - None для всех строк
	/// 11 - Sub для всех строк
	/// 12 - Up для всех строк
	/// 13 - Average для всех строк
	/// 14 - Paeth для всех строк
	/// 15 - PNG Optimum
	/// Все отсчеты идут сверху-вниз, слева-направо.
	/// Каждая строка изображения занимает целое число байт (округляется если необходимо)
	/// Отсчеты меньше байта упакованы в байты с порядком от старшего к младшему (MSB)
	/// Все отсчеты для всех цветовых компонент вне изображения равны 0
	/// Для PNG предсказаний, код предсказания идет первым в каждой строке (каждая строка может иметь разный
	/// алгоритм предсказания).
	/// Для алгоритма 2 (TIFF) - предсказание каждого цветового компонента учитывает сколько бит занимает компонент.
	/// Для PNG алгоритмов предсказание берется для целых байтов
	/// соответствующих другим отсчетам, даже если в одном байте несколько цветовых компонент, или же один компонент занимает несколько байт.
	/// </summary>
	/// <param name="data">входной массив данных</param>
	/// <param name="predictor">код предсказания</param>
	/// <param name="colors">число перемешанных компонент цвета</param>
	/// <param name="bpp">число бит на один компонент, может быть 1,2,4,8,16</param>
	/// <param name="columns">число точек в строке изображения</param>
	public static byte[] Decode(byte[] data, int predictor, int colors, int bpp, int columns)
	{
	    return data;
	    byte[] pred = new byte[colors];
            for (int i = 0; i < data.Length / colors; i++)
                for (int j = 0; j < colors; j++)
                {
                    if ((i * colors) % columns == 0)
                        pred[j] = 0;
                    else
                        pred[j] = data[(i - 1) * colors + j];
                    data[i * colors + j] += pred[j];
                }
            return data;
	}
    }
}
      
