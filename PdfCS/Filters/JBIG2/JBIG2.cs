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
        /// Ширина изображения
        /// </summary>
        private static int width;

        /// <summary>
        /// Высота изображения
        /// </summary>
        private static int height;

        /// <summary>
        /// Данные страницы, 1 бит на точку изображения, точки идут слева-направо, сверху-вниз
        /// </summary>
        public static byte[] page;

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
	    JBIG2.height = (int)param["Height"];
            JBIG2.width = (int)param["Width"];
            page = new byte[(width / 8) * height];
            return page;	    
        }

	/// <summary>
        /// Помещает точку на страницу
        /// </summary>
        static void PutPixel(int x, int y)
        {
            int bytenumber = y * width + x / 8;
            int bitnumber = 7 - x % 8;
            page[bytenumber] = (byte) (page[bytenumber] | 1 << bitnumber);
        }	
    }
}
