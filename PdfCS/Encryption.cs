using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
        /// Длина пароля, которая должна составлять 32 байта
        /// </summary>
        private const int LengthPass = 32;

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
        static byte[] O;

        /// <summary>
        /// – строка пароля пользователя;
        /// </summary>
        static byte[] U;

        /// <summary>
        /// – флаги доступа;
        /// </summary>
        static int P;

        /// <summary>
        /// – зашифрованы метаданные.  
        /// </summary>
        static bool encryptMetadata;

        /// <summary>
        /// Длина ключа в битах от 40 до 128, кратная 8
        /// </summary>
        private static int Length;

        /// <summary>
        /// Ключ шифрования
        /// </summary>
        static byte[] encryptionKey;

        /// <summary>
        /// Аутентификация владельца
        /// 
        /// Алгоритм:
        /// 1. Вычисляем ключ шифрования с помощью метода 
        /// "Вычисление строки пароля владельца #43" и сохраняем его
        /// 2. Для версии шифрования 2 дешифруем строку владельца O 
        /// методом "RC4 Дешифровка RC4 #38" с ключом шифрования
        /// 3. Для версии шифрования 3 и больше повторяем 20 раз: 
        /// первая итерация дешифруем O как в шаге 2
        /// следующие итерации дешифруем предыдущее значение с помощью RC4 и ключа, 
        /// который получается из encryptionKey 
        /// и побитовым XOR с каждым байтом ключа и номером итерации от 19 до 0
        /// 4. Результат шагов 2 или 3 аутентифицируем 
        /// как пароль пользователя с помощью "Аутентификация пользователя #46"
        /// </summary>
        /// <param name="pass">пароль владельца</param>
        /// <returns>Прошла аутентификация или нет</returns>
        static bool OwnerAuthentificate(string pass)
        {
            encryptionKey = ComputeOwnerPassword(pass, null);
            byte[] result = null;

            result = DecodeRC4(O, encryptionKey);
            
            if (R >= 3)
                for (int i = 19; i >= 0; i--)
                {
                    for (int j = 0; j < encryptionKey.Length; j++)
                        encryptionKey[j] ^= (byte)i;
                    result = DecodeRC4(result, encryptionKey);
                }

            return UserAuthentificate(result.ToString());
        }

        /// <summary>
        /// В зависимости от версии шифрования вызывает
        /// Вычисление строки пароля пользователя #44 (2 версия)
        /// Вычисление строки пароля пользователя #45 (для версии шифрования 3 и выше)
        /// Сравниваем полученную строку со строкой U, для версии 3 и выше сравниваем только 16 байт
        /// Если значения равны, то аутентификация успешная
        /// </summary>
        /// <param name="pass">Пароль пользователя</param>
        /// <returns>Успешная ли аутентификация</returns>
        static bool UserAuthentificate(string pass)
        {
            byte[] temp;
            if (R == 2)
            {
                temp = PadString(pass, pass.Length);
                for (int i = 0; i < temp.Length; i++)
                    if (temp[i] != U[i])
                        return false;
                return true;
            }
            else if (R >= 3)
            {
                temp = ComputeUserPasswordV3(pass);
                for (int i = 0; i < 16; i++)
                    if (temp[i] != U[i])
                        return false;
                return true;
            }
            return false;
        }

        /// <summary>
        /// заглушка
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        static byte[] DecodeRC4(byte[] data, byte[] key)
        {

            return null;
        }

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
            O = (byte[])param["O"];
            U = (byte[])param["U"];
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
        /// Вычисление строки пароля владельца
        /// 
        /// Алгоритм:
        /// 1. Вычисляем ключ шифрования с помощью метода 
        /// "Вычисление строки пароля владельца #43" и сохраняем его
        /// 2. Для версии шифрования 2 дешифруем строку владельца O 
        /// методом "RC4 Дешифровка RC4 #38" с ключом шифрования
        /// 3. Для версии шифрования 3 и больше повторяем 20 раз: 
        /// первая итерация дешифруем O как в шаге 2
        /// следующие итерации дешифруем предыдущее значение с помощью RC4 и ключа, 
        /// который получается из encryptionKey 
        /// и побитовым XOR с каждым байтом ключа и номером итерации от 19 до 0
        /// 4. Результат шагов 2 или 3 аутентифицируем 
        /// как пароль пользователя с помощью "Аутентификация пользователя #46"
        /// </summary>
        /// <param name="ownPass">пароль владельца</param>
        /// <param name="userPass">пароль пользователя</param>
        /// <returns>возвращает строку байт для значения O</returns>
        public static byte[] ComputeOwnerPassword(string ownPass, string userPass)
        {
            if (ownPass == "")
                ownPass = userPass;
            return MD5Hash(GetPass(ownPass));
        }

        /// <summary>
        /// Вычисление ключа для пароля владельца
        /// </summary>
        /// <param name="array">массив байт пароля владельца</param>
        /// <returns>ключ в виде массива байт</returns>
        private static byte[] MD5Hash(byte[] array)
        {
            var md5 = MD5.Create();
            int n = 5;
            byte[] hash;

            hash = md5.ComputeHash(array);

            if (R >= 3)
            {
                for (int i = 0; i < 50; i++)
                    hash = md5.ComputeHash(hash);

                Array.Resize(ref hash, Length);
            }
            else if (R == 2)
                Array.Resize(ref hash, n);

            return hash;
        }

        /// <summary>
        /// Вычисление пароля владельца и пользователя (для метода ComputeOwnerPassword)
        /// </summary>
        /// <param name="s">пароль пользователя</param>
        /// <returns>пароль</returns>
        private static byte[] GetPass(string s)
        {
            byte[] array;

            if (s.Length / 8 > 32)
                array = Encoding.UTF8.GetBytes(s.Substring(0, 32));
            else
                array = PadString(s, s.Length);

            return array;
        }

        /// <summary>
        /// Дополнение строки пароля (для метода GetKey)
        /// </summary>
        /// <param name="s">пароль владельца || пароль пользователя</param>
        /// <returns>возвращает дополненную строку</returns>
        private static byte[] PadString(string s, int n)
        {
            int m = LengthPass - n - 1;
            byte[] extensionArray = { 0x28, 0xBF, 0x4E, 0x5E, 0x4E, 0x75, 0x8A, 0x41,
                                      0x64, 0x00, 0x4E, 0x56, 0xFF, 0xFA, 0x01, 0x08,
                                      0x2E, 0x2E, 0x00, 0xB6, 0xD0, 0x68, 0x3E, 0x80,
                                      0x2F, 0x0C, 0xA9, 0xFE, 0x64, 0x53, 0x69, 0x7A};

            s += extensionArray[m];

            return Encoding.UTF8.GetBytes(s);
        }

	public static byte[] ComputeDecryptionKey(string pass){
            return null;
        }

	/// <summary>
        /// Вычисляет строку для пароля пользователя
        /// (для сравнения с U), версия шифрования 2.
        /// Алгоритм:
        /// 1. Вычислить ключ шифрования (Вычисление ключа шифрования #42) и сохранить его;
        /// 2. С помощью алгоритма RC4 (Дешифровка RC4 #38) зашифровать строку полученную
        /// после функции PadString к строке пароля, в качестве ключа берется результат шага 1;
        /// 3. Вернуть результат шага 2.
        /// </summary>
        /// <param name="pass">Строка пароля пользователя</param>
        /// <returns>
        /// Строка для пароля пользователя
        /// </returns>
        public static byte[] ComputeUserPasswordV2(string pass){
            byte[] key = ComputeDecryptionKey(pass);
            byte[] result;

            if(pass.Length < 32)
                result = PadString(pass, 32 - pass.Length);
            else
                result = Encoding.UTF8.GetBytes(pass.Substring(0, 32));
            encryptionKey = result;
            return DecodeRC4(result, key);
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

        /// <summary>
        /// заглушка
        /// </summary>
        /// <param name="pass"></param>
        /// <returns></returns>
        static byte[] ComputeUserPasswordV3(string pass)
        {
            return new byte[0];
        }
    }
}
