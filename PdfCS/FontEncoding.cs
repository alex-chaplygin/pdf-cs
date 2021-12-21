using System.Collections.Generic;

namespace PdfCS
{
    /// <summary>
    /// Класс кодировки символов шрифта
    /// </summary>
    public class FontEncoding
    {
        /// <summary>
        /// Имя базовой кодировки
        /// </summary>
        public string baseEncoding;

        /// <summary>
        /// Массив соответствия кодов именам символов
        /// </summary>
        public string[] encoding;

        /// <summary>
        /// Поля словаря:
        /// Type(необязательное) - должно быть "Encoding"
        /// BaseEncoding(необязательное) - имя базовой кодировки
        /// Differences(необязательное) - массив отличий от базовой кодировки или полный массив кодировки
        /// содержит записи
        /// code1(число) name1(имя) name2...
        /// code2(число) name1(имя) name2...
        /// каждый код - это первый индекс кода для последующих имен, которые должны быть изменены
        /// первое имя соответствует этому коду, последующие увеличиваются на единицу, пока не встретится новый код
        /// последовательности могут быть в любом порядке, но не пересекаются
        /// </summary>
        /// <param name="dic">словарь из поля Encoding у шрифта</param>
        /// <param name="lastChar">наибольший код символа</param>
        public FontEncoding(Dictionary<string, object> dic, int lastChar)
        {
            encoding = new string[lastChar];
            if (dic.ContainsKey("BaseEncoding"))
                baseEncoding = (string)dic["BaseEncoding"];
            if (dic.ContainsKey("Differences"))
            {
                var differences = (object[])dic["Differences"];
                int startValueIndex = 0;
                for (int i = 0; i < differences.Length; i++)
                {
                    if (differences[i] is int)
                    {
                        startValueIndex = i;
                        continue;
                    }
                    encoding[(int)differences[startValueIndex] + i - startValueIndex - 1] = (string)differences[i];
                }
            }
        }
    }
}
