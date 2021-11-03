using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfCS
{
    /// <summary>
    ///   Класс дешифрования документа
    /// </summary>
    public class Encryption
    {
	/// <summary>
        /// Флаги доступа к документу:
        /// – печать (если версия 3, то печать в низком качестве);
        /// </summary>
        const int PrintAccess = 1 << 2;

        /// <summary>
        /// – модификация (прочая);
        /// </summary>
        const int ModifyAccess = 1 << 3;

	/// <summary>
        /// – копирование текста и графики из документа; 
        /// </summary>
        const int CopyAccess = 1 << 4;

	/// <summary>
        /// – добавление аннотаций, заполнение полей форм;
        /// </summary>
        const int AddAccess = 1 << 5;

	/// <summary>
        /// – заполнение полей форм (даже если AddAccess = 0); 
        /// </summary>
        const int FillAccess = 1 << 8;

	/// <summary>
        /// – извлечение текста и графики; 
        /// </summary>
        const int ExtractAccess = 1 << 9;

	/// <summary>
        /// – добавление, поворот, удаление страниц, создание пометок и иконок; 
        /// </summary>
        const int AssembleAccess = 1 << 10;

	/// <summary>
        /// – печать в полном качестве.
        /// </summary>
        const int PrintFullAccess = 1 << 11;

        /// <summary>
        /// Поля класса:
        /// – версия шифрования;
        /// </summary>
        static int R;

	/// <summary>
        /// – код алгоритма;
        /// </summary>
        static int V;

	/// <summary>
        /// – строка для генерации ключа;
        /// </summary>
        static string O;

	/// <summary>
        /// – строка пароля пользователя;
        /// </summary>
        static string U;

	/// <summary>
        /// – флаги доступа;
        /// </summary>
        static int P;

	/// <summary>
        /// – зашифрованы метаданные.  
        /// </summary>
        static bool encryptMetadata;

        /// <summary>
        /// Инициализирует параметры стандартного фильтра дешифрования.
        /// Возможные значения:
        /// R - версия шифрования;
        /// 2 - Если был параметр V меньше 2 #37;
        /// 3 - если V имеет значение 2 или 3;
        /// 4 - значение V должно быть 4;
        /// O - строка из 32 байт, основана на паролях пользователя и владельца, 
        /// используется для вычисления ключа;
        /// U - строка из 32 байт, основана на пароле пользователя, 
        /// определяет спрашивать ли пароль или нет;
        /// P - набор флагов доступа;
        /// EncryptMetadata (необязательно) - если true то поток метаданных документа зашифрован, 
        /// значение по умолчанию: true;
        /// </summary>
        /// <param name="param">Словарь параметров</param>
        static void InitStandardFilter(Dictionary<string, object> param)
        {
            R = (int)param["R"];
            V = (int)param["V"];
            O = (string)param["O"];
            U = (string)param["U"];
            P = (int)param["P"];
            encryptMetadata = (bool)param["encryptMetadata"];

            if ((V < 2 && R != 2) || ((V == 2 || V == 3) && R != 3) || (V == 4 && R != 4))
                throw new Exception("Неверная версия шифрования/код алгоритма");

            if (O.Length != 32)
                throw new Exception("Строка для генерации ключа имела неверный формат");

            if (U.Length != 32)
                throw new Exception("Строка пароля пользователя имела неверный формат");
        }
	
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
