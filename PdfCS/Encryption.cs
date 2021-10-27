using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfCS
{
    /// <summary>
    ///   Класс шифрования для потоковых объектов
    /// </summary>
    public class Encryption
    {
        /// <summary>
        ///   Применяет фильтр для дешифрования потока
        /// </summary>
        /// <param name="stream">Зашифрованный поток</param>
        /// <param name="name">Имя фильтра (если Identity - то без шифрования)</param>
        /// <param name="params">Словарь параметров фильтра</param>
        /// <returns>
        /// Дешифрованный поток как массив байт
        /// </returns>
        public static byte[] ApplyFilter(byte[] stream, string name, Dictionary<string, object> param)
        {
            return stream;
        }
    }
}
