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
	public static byte[] Decode(byte[] data, int predictor, int colors, int bpp, int columns)
	{
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
      
