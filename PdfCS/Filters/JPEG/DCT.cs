using System;

namespace PdfCS
{
    /// <summary>
    ///   Класс для DCT, квантования, сдвига уровней
    /// </summary>
    public class DCT
    {
	/// <summary>
	///   1 / sqrt(2)
	/// </summary>
        private readonly static double rcpSqrt2 = 0.7071067811865475;

	/// <summary>
	///   PI / 16
	/// </summary>
        private readonly static double piDiv16 = 0.19634954084936207;

	/// <summary>
	///   Матрица зиг-заг обхода
	/// </summary>
        private readonly static int[,] zigzag = {{0, 1, 5, 6, 14, 15, 27, 28},
                                                {2, 4, 7, 13, 16, 26, 29, 42},
                                                {3, 8, 12, 17, 25, 30, 41, 43},
                                                {9, 11, 18, 24, 31, 40, 44, 53},
                                                {10, 19, 23, 32, 39, 45, 52, 54},
                                                {20, 22, 33, 38, 46, 51, 55, 60},
                                                {21, 34, 37, 47, 50, 56, 59, 61},
                                                {35, 36, 48, 49, 57, 58, 62, 63}};

        /// <summary>
        /// распаковка одномерного массива 
        /// в двумерный массив с зигзаг обходом
        /// </summary>
        /// <param name="coef">массив коэффициентов</param>
        /// <returns>двумерный блок коэффициентов</returns>
        static short[,] UnZip(short[] coef)
        {
            short[,] block = new short[8, 8];
            int x, y;

            for (x = 0; x < 8; x++)
                for (y = 0; y < 8; y++)
                    block[x, y] = coef[zigzag[y, x]];
            return block;
        }

        /// <summary>
        /// Обратное квантование
        /// </summary>
        /// <param name="coef">исходные коэффициенты</param>
        /// <param name="quantTable">коэффициенты квантования</param>
        /// <returns>деквантованные коэффициенты</returns>
        static short[] DeQuant(short[] coef, byte[] quantTable)
        {
            for (int i = 0; i < 64; i++)
                coef[i] *= quantTable[i];
            return coef;
        }

        /// <summary>
        /// Обратное дискретное косинусное преобразование
        /// </summary>
        /// <param name="block">блок коэффициентов</param>
        /// <returns>блок отсчетов</returns>
        static short[,] IDCT(short[,] block)
        {
            short[,] matrix2 = new short[8, 8];
            double s = 0;
            double cV = 1;
            double cU = 1;

            for (int x = 0; x < 8; x++)
                for (int y = 0; y < 8; y++)
                {
                    s = 0;
                    for (int u = 0; u < 8; u++)
                        for (int v = 0; v < 8; v++)
                        {
                            cV = 1;
                            cU = 1;
                            if (u == 0)
                                cU = rcpSqrt2;
                            if (v == 0)
                                cV = rcpSqrt2;
                            short sh = block[v, u];
                            double vv = (double)sh;
                            s += cU * cV * (vv * Math.Cos((2 * x + 1) * u * piDiv16) * Math.Cos((2 * y + 1) * v * piDiv16));
                        }
                    matrix2[y, x] = (short)Math.Round(s / 4.0);
                }
            return matrix2;
        }

        /// <summary>
        /// Ограничение диапазона по двум границам
        /// </summary>
        /// <param name="i">число</param>
        /// <returns>ограниченно число</returns>
        static byte ClampByte(int i)
        {
            if (i > 255)
                return 255;
            else if (i < 0)
                return 0;
            return (byte)i;
        }

        /// <summary>
        /// Сдвиг уровня
        /// </summary>
        /// <param name="block">блок отсчетов</param>
        /// <returns>блок сдвинутых отсчетов</returns>
        static short[,] LevelShift(short[,] block)
        {
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    block[i, j] = ClampByte(block[i, j] + 128);
            return block;
        }
    }
}
