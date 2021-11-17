using System.IO;

namespace PdfCS
{
    /// <summary>
    /// Класс (рас)шифровки RC4
    /// </summary>
    public class RC4
    {
        static byte[] S = new byte[256];
        static int x = 0;
        static int y = 0;
	
        /// <summary>
        /// Метод (рас)шифровки RC4
        /// </summary>
        /// <param name="word">Данные для (рас)шифровки</param>
        /// <param name="key">Исходные данные для генарации ключа</param>
        /// <returns>зашифрованные данные</returns>
        public static byte[] Decode(byte[] word, byte[] key)
        {
            int j = 0;
            for (int i = 0; i < 256; i++)
                S[i] = (byte)i;
            for (int i = 0; i < 256; i++)
            {
                j = (j + S[i] + key[i % key.Length]) % 256;
                Swap(i, j);
            }
            for (int i = 0; i < word.Length; i++)
                word[i] = (byte)(word[i] ^ KeyGen());
            return word;
        }

        /// <summary>
        /// Метод генерации ключа
        /// </summary>
        /// <returns>1 байт ключа</returns>
        private static byte KeyGen()
        {
            x = (x + 1) % 256;
            y = (y + S[x]) % 256;
            Swap(x, y);
            return S[(S[x] + S[y]) % 256];
        }

        private static void Swap(int x, int y)
	{
	    byte temp = S[x];
            S[x] = S[y];
            S[y] = temp;
	}
    }
}
