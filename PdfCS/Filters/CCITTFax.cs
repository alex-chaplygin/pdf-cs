using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfCS
{
    /// <summary>
    ///   Класс дешифрования TIFF-изображения
    /// </summary>
    public class CCITTFax
    {
        /// <summary>
        /// Декодирует изображение формата TIFF
        /// </summary>
        /// <param name="stream">поток сжатой информации</param>
        /// <param name="param">словарь параметров фильтра</param>
        /// <returns>декодированное изображение</returns>
        public static byte[] Decode(byte[] stream, Dictionary<string, object> param = null)
        {
            throw new Exception ("CCITTFax не реализован");
        }
    }
}
