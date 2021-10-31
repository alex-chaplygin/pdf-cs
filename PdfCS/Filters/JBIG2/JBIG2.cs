using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfCS
{
    /// <summary>
    /// Класс, декодирующий изображение формата JBIG2
    /// </summary>
    public class JBIG2
    {
        /// <summary>
        /// Метод декодирования изображения JBIG2
        /// </summary>
        /// <param name="stream"> Закодированный поток байт</param>
        /// <param name="params"> Особые параметры декодирования </param>
        /// <returns>
        /// Распакованный массив байт
        /// </returns>
        public static byte[] Decode(byte[] stream, Dictionary<string, object> param= null)
        {
	    throw new Exception("JBIG 2 не реализован");
            return stream;
        }
    }
}
