using System;
using System.Collections.Generic;
using System.IO;
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
        private const int PrintAccess = 1 << 2;

        /// <summary>
        /// – модификация (прочая);
        /// </summary>
        private const int ModifyAccess = 1 << 3;

        /// <summary>
        /// – копирование текста и графики из документа; 
        /// </summary>
        private const int CopyAccess = 1 << 4;

        /// <summary>
        /// – добавление аннотаций, заполнение полей форм;
        /// </summary>
        private const int AddAccess = 1 << 5;

        /// <summary>
        /// – заполнение полей форм (даже если AddAccess = 0); 
        /// </summary>
        private const int FillAccess = 1 << 8;

        /// <summary>
        /// – извлечение текста и графики; 
        /// </summary>
        private const int ExtractAccess = 1 << 9;

        /// <summary>
        /// – добавление, поворот, удаление страниц, создание пометок и иконок; 
        /// </summary>
        private const int AssembleAccess = 1 << 10;

        /// <summary>
        /// – печать в полном качестве.
        /// </summary>
        private const int PrintFullAccess = 1 << 11;

        /// <summary>
        /// Поля класса:
        /// – версия шифрования;
        /// </summary>
        private static int R;

        /// <summary>
        /// – код алгоритма;
        /// </summary>
        private static int V;

        /// <summary>
        /// – строка для генерации ключа;
        /// </summary>
        private static byte[] O;

        /// <summary>
        /// – строка пароля пользователя;
        /// </summary>
        private static byte[] U;

        /// <summary>
        /// – флаги доступа;
        /// </summary>
        private static int P;

        /// <summary>
        /// – зашифрованы метаданные.  
        /// </summary>
        private static bool encryptMetadata;

        /// <summary>
        /// Длина ключа в битах от 40 до 128, кратная 8
        /// </summary>
        private static int Length;

        /// <summary>
        /// Ключ шифрования
        /// </summary>
        private static byte[] encryptionKey;

        private static byte[] id0;

	/// <summary>
        /// Имя фильтра
        /// </summary>
        private static string Filter = "Standard";

        /// <summary>
        /// Имя другого фильтра
        /// </summary>
        private static string SubFilter;

        /// <summary>
        /// Cловарь фильтров Crypt
        /// </summary>
        private static Dictionary<string, object> CF;

        /// <summary>
        /// Имя фильтра для шифрования потоков
        /// </summary>
        private static string StmF = "Identity";

        /// <summary>
        /// Имя фильтра для шифрования строк
        /// </summary>
        private static string StrF = "Identity";

        /// <summary>
        /// Имя фильтра для шифрования встроенных файловых потоков
        /// </summary>
        private static string EFF;
	
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
        public static bool OwnerAuthentificate(string pass)
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
	    /// Аутентификация пользователя
	    ///
        /// 1. В зависимости от версии шифрования вызывает
        /// Вычисление строки пароля пользователя #44 (2 версия)
        /// Вычисление строки пароля пользователя #45 (для версии шифрования 3 и выше)
        /// 2. Сравниваем полученную строку со строкой U, для версии 3 и выше сравниваем только 16 байт
        /// 3. Если значения равны, то аутентификация успешная
        /// </summary>
        /// <param name="pass">Пароль пользователя</param>
        /// <returns>Успешная ли аутентификация</returns>
        public static bool UserAuthentificate(string pass)
        {
            byte[] temp;
            if (R == 2)
            {
                temp = ComputeUserPasswordV2(pass);
		/*		Console.Write("\nu: ");
		PrintBytes(temp);
		Console.Write("U: ");
		PrintBytes(U);*/
                return temp.SequenceEqual(U);
            }
            else if (R >= 3)
            {
                temp = ComputeUserPasswordV3(pass);
                return temp.Take(16).SequenceEqual(U.Take(16));
            }
            return false;
        }

	public static void PrintBytes(byte[] b)
	{
	    for (int i = 0; i < b.Length; i++)
		Console.Write(Convert.ToString(b[i], 16) + " ");
	    Console.WriteLine();
	}

        /// <summary>
        /// заглушка
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] DecodeRC4(byte[] data, byte[] key)
        {
            return RC4.Decode(data, key);
        }

	/// <summary>
        /// Инициализирует шифрование.
        /// Метод static void Init(Dictionary<string, object> encrypt, object[] id) инициализирует шифрование
        /// encrypt - словарь из хвоста документа
        /// id - массив из двух байтовых строк byte[]
        /// 
        /// Поля словаря:
        /// Filter - имя фильтра(Standard - стандарный фильтр с паролями)
        /// SubFilter(необязательный) - имя другого фильтра(вместо Filter)
        /// V - код алгоритма
        /// 0 - невозможное значение
        /// 1 - RC4 или AES длина ключа меньше 40 бит
        /// 2- RC4 или AES длина ключа больше 40 бит
        /// 3 - невозможное значение
        /// 4 - определяется полями CF, StmF, StrF
        /// Length(необязательно) - длина ключа(от 40 до 128), по умолчанию - 40
        /// CF(необязательно) - словарь фильтров Crypt
        /// StmF(необязательно) - имя фильтра для потоков(ключ в CF), по-умолчанию - Identity
        /// StrF(необязательно) - имя фильтра для строк(ключ в CF), по-умолчанию - Identity
        /// EFF(необязательно) - имя фильтра для встроенных файлов(ключ в CF)
        /// Если имя Filter = Standard то инициализируем стандартный фильтр #41
        /// <summary>
        public static void Init(Dictionary<string, object> encrypt, object[] id)
        {
            Filter = (string)encrypt["Filter"];

            id0 = (byte[])id[0];

            V = (int)encrypt["V"];
            if (encrypt.ContainsKey("SubFilter"))
                SubFilter = (string)encrypt["SubFilter"];
            if (encrypt.ContainsKey("Length"))
                Length = (int)encrypt["Length"];

            if (encrypt.ContainsKey("CF"))
            {
                CF = (Dictionary<string, object>)encrypt["CF"];
                if (CF.ContainsKey("StmF"))
                    StmF = (string)CF["StmF"];
                if (CF.ContainsKey("StrF"))
                    StrF = (string)CF["StrF"];
                if (CF.ContainsKey("EFF"))
                    EFF = (string)CF["EFF"];
            }

            if ((V == 0) || (V == 3))
                throw new Exception("Невозможное значение кода алгоритма.");
            if ((Length < 40) || (Length > 128))
                throw new Exception("Неверная длина ключа.");

            if (Filter == "Standard") InitStandardFilter(encrypt);
            else throw new Exception("Неизвестный фильтр.");
        }

        /// <summary>
        /// Инициализирует параметры стандартного фильтра дешифрования.
	///
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
        public static void InitStandardFilter(Dictionary<string, object> param)
        {
            R = (int)param["R"];
            V = (int)param["V"];
	        O = ((char[])param["O"]).Select<char, byte>(x => (byte)x).ToArray();
            U = ((char[])param["U"]).Select<char, byte>(x => (byte)x).ToArray();
	        P = (int)param["P"];
            if (param.ContainsKey("encryptMetadata"))
		encryptMetadata = (bool)param["encryptMetadata"];
	    else
		encryptMetadata = true;

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
            byte[] hash = md5.ComputeHash(array);

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
                array = PadString(s);
            return array;
        }

        /// <summary>
        /// Дополнение строки пароля (для метода GetKey)
        /// </summary>
        /// <param name="s">пароль владельца || пароль пользователя</param>
        /// <returns>возвращает дополненную строку</returns>
        public static byte[] PadString(string s)
        {
            byte[] ext = {
                0x28, 0xBF, 0x4E, 0x5E, 0x4E, 0x75, 0x8A, 0x41,
                0x64, 0x00, 0x4E, 0x56, 0xFF, 0xFA, 0x01, 0x08,
                0x2E, 0x2E, 0x00, 0xB6, 0xD0, 0x68, 0x3E, 0x80,
                0x2F, 0x0C, 0xA9, 0xFE, 0x64, 0x53, 0x69, 0x7A
            };
            byte[] o = new byte[32];
            for (int i = 0; i < 32; i++)
                if (i >= s.Length)
                    o[i] = ext[i - s.Length];
                else
                    o[i] = (byte)s[i];
            return o;
        }

	    /// <summary>
	    /// вычисляет ключ шифрования
	    ///
	    /// Алгоритм:
	    /// Если длина строки больше 32 символов, обрезает строку до 32 символов
	    /// если меньше, то строка дополняется до 32 символов, символами из следующей строки (строка дополнения):
	    /// < 28 BF 4E 5E 4E 75 8A 41 64 00 4E 56 FF FA 01 08
	    /// 2E 2E 00 B6 D0 68 3E 80 2F 0C A9 FE 64 53 69 7A >
	    /// например, если длина n, то дополняется 32 - n символами
	    /// Если пароль пустой, то берется вся строка дополнения
	    /// Собирается строка для MD5 хеш-функции (смотри класс MD5), начинается со строки пароля
	    /// Добавляется строка O
	    /// Флаги P преобразуются в массив из 4х байт (младший впереди) и добавляется к строке MD5
	    /// Добавляется первая строка из id
	    /// Если версия шифрования 4 или больше и метаданные не зашифрованы, то 0xffffffff добавляется к строке MD5
	    /// Вычисляется хеш
	    /// Если версия шифрования 3 или больше, то 50 раз повторяется:
	    /// у предыдущего результата хеша берется n байт (n - число байт в ключе, вычисляется из Length) и от этого значения еще раз вычисляется MD5
	    /// У полученной строки байт берется n байт как ключ шифрования,
	    /// где n = 5 для версии шифрования 2, а для версий 3 и более вычисляется из Length как число байт в ключе
	    /// </summary>
        /// <param name="pass"> строка пароля</param>
        /// <returns>ключ шифрования</returns>
        public static byte[] ComputeDecryptionKey(string pass)
        {
	        byte[] key = PadString(pass);
            key = key.Concat(O).ToArray();
            byte[] bytesP = BitConverter.GetBytes(P);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(bytesP);
            key = key.Concat(bytesP).ToArray();
            key = key.Concat(id0).ToArray();
            byte[] ff = new byte[] { 255, 255, 255, 255 };
            if ((R == 4) || (encryptMetadata == false))
                key = key.Concat(ff).ToArray();
            key = MD5Hash(key);
	    encryptionKey = key;
	    Console.Write("Key = ");
	    PrintBytes(encryptionKey);
            return key;
        }

        /// <summary>
        /// Вычисляет строку для пароля пользователя (для сравнения с U), версия шифрования 2.
	    ///
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
        public static byte[] ComputeUserPasswordV2(string pass)
        {
            return DecodeRC4(
                PadString(pass),
                ComputeDecryptionKey(pass)
            );
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
        /// Преобразование int в byte[] в little-endian порядке
        /// </summary>
        /// <param name="i">число для преобразования</param>
        /// <param name="num">нужное количество байт</param>
        private static byte[] IntToBytes(int i, int num)
        {
            byte[] bytes = BitConverter.GetBytes(i);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            if (bytes.Length < num)
            {
                return Enumerable.Repeat((byte)0x00, num - bytes.Length).Concat(bytes).ToArray();
            }
            if (bytes.Length > num)
            {
                return bytes.Skip(bytes.Length - num).ToArray();
            }
            return bytes;
        }

        /// <summary>
        /// Декодирование объекта
        /// Алгоритм:
        /// 1. Берем encryptionKey
        /// 2. Добавляем 3 байта от objNum
        /// 3. Добавляем 2 байта от genNum
        /// 4. Если aes, добавляем дополнительные 4 байта
        /// 5. Получаем 16 байт MD5 из ключа
        /// 6. Передаем объект и полученный хэш в соответсвующие типу кодировки методы
        /// 7. Преобразуем формат результата для соответствия входному типу
        /// 8. Возвращаем результат
        /// </summary>
        /// <param name="o">дешифрруемый объект(строка- char[], поток - byte[])</param>
        /// <param name="objNum">номер</param>
        /// <param name="genNum">поколение</param>
        /// <returns>дешифрованный объект char[] или byte[], тип вывода соответсвует o</returns>
        public static object DecryptObject(object o, int objNum, int genNum, bool aes=false)
        {
            byte[] obj;
	    if (R < 2)
		return o;
            if (o.GetType() == typeof(char[]))
                obj = ((char[])o).Select(x => (byte)x).ToArray();
            else
                obj = (byte[])o;
            List<byte> key = encryptionKey.ToList();
            key.AddRange(IntToBytes(objNum, 3));
            key.AddRange(IntToBytes(genNum, 2));
	    Console.Write("Hash = ");
	    PrintBytes(key.ToArray());
            if (aes)
                key.AddRange(new List<byte> {0x73, 0x41, 0x6c, 0x54});
            byte[] hash = MD5.Create().ComputeHash(key.ToArray()).Take(16).ToArray();
            byte[] result;
            if (aes) 
                result = DecodeAES(obj, hash, ((byte[])o).Take(16).ToArray());
            else 
                result = DecodeRC4(obj, hash);
            if (o.GetType() == typeof(char[]))
                return result.Select(x => (char)x).ToArray();
            return result;
        }

	/// <summary>
        /// Декодирование AES
        /// </summary>
        /// <param name="data">зашифрованные данные</param>
        /// <param name="Key">ключ шифрования</param>
        /// <param name="IV">Вектор инициализации</param>
        /// <returns>возвращает дешифрованные данные</returns>
        private static byte[] DecodeAES(byte[] data, byte[] Key, byte[] IV)
        {
            using (Aes alg = Aes.Create())
            {
                alg.Key = Key;
                alg.IV = IV;

                ICryptoTransform decryptor = alg.CreateDecryptor(alg.Key, alg.IV);

                using (MemoryStream encoded = new MemoryStream(data))
                    using (CryptoStream crypt = new CryptoStream(encoded, decryptor, CryptoStreamMode.Read))
                    {
                        byte[] decoded = new byte[crypt.Length];
                        crypt.Read(decoded, 0, (int)crypt.Length);
                        return decoded;
                    }
            }
        }   

        /// <summary>
        /// заглушка
        /// </summary>
        /// <param name="pass"></param>
        /// <returns></returns>
        private static byte[] ComputeUserPasswordV3(string pass)
        {
            return new byte[0];
	}
	
        /// <summary>
        /// Декодирование DES
        /// </summary>
        /// <param name="data">зашифрованные данные</param>
        /// <param name="key">ключ шифрования</param>
        /// <returns>возвращает дешифрованные данные</returns>
        public static byte[] DecodeDES(byte[] data, byte[] key)
        {
            using (DESCryptoServiceProvider desCrypt = new DESCryptoServiceProvider { Key = key })
            using (ICryptoTransform cryptTrans = desCrypt.CreateDecryptor())
            using (var memoryStr = new MemoryStream())
            {
                using (var cpyptStr = new CryptoStream(memoryStr, cryptTrans, CryptoStreamMode.Write))
                {
                    cpyptStr.Write(data, 0, data.Length);
                    cpyptStr.FlushFinalBlock();
                }
                return memoryStr.ToArray();
            }
        }

        /// <summary>
        /// Декодирование RC2
        /// </summary>
        /// <param name="data">зашифрованные данные</param>
        /// <param name="key">ключ шифрования</param>
        /// <returns>возвращает дешифрованные данные</returns>
        public static byte[] DecodeRC2(byte[] data, byte[] key)
        {
            using (RC2CryptoServiceProvider rc2Crypt = new RC2CryptoServiceProvider { Key = key })
            using (ICryptoTransform cryptTrans = rc2Crypt.CreateDecryptor())
            using (var memoryStr = new MemoryStream())
            {
                using (var cpyptStr = new CryptoStream(memoryStr, cryptTrans, CryptoStreamMode.Write))
                {
                    cpyptStr.Write(data, 0, data.Length);
                    cpyptStr.FlushFinalBlock();
                }
                return memoryStr.ToArray();
            }
        }
    }
}
