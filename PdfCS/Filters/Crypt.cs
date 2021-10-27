using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfCS
{
    /// <summary>
    ///   Класс дешифрования для потоковых объектов
    /// </summary>
    public class Crypt
    {
        /// </summary>
        ///   Декодирует поток с помощью указанного фильтра с помощью класса Encryption
        /// </summary>
        /// <param name="stream">Поток для декодирования</param>
        /// <param name="param">Словарь с параметрами</param>
        /// <returns>
        /// Дешифрованный поток как массив байт
        /// </returns>
        public static byte[] Decode(byte[] stream, Dictionary<string, object> param)
        {
            if (!param.ContainsKey("Name"))
                param["Name"] = "Identity";
            return Encryption.ApplyFilter(stream, param["Name"].ToString(), param);
        }
    }
}
