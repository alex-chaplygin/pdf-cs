using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfCS
{
    /// <summary>
    /// Класс для представления 
    /// трехмерной матрицы состояния объектов
    /// </summary>
    public class Matrix
    {
        /// <summary>
        /// Модель матрицы, 
        /// представляемой объектом класса Matrix
        /// | a   b   0|
        /// | c   d   0|
        /// | e   f   1|
        /// </summary>
        private double a = 0;
        private double b = 0;
        private double c = 0;
        private double d = 0;
        private double e = 0;
        private double f = 0;

        /// <summary>
        /// Конструктор, инициализирующий единичную матрицу
        /// </summary>
        public Matrix()
        {
            a = 1.0;
            d = 1.0;
        }

        /// <summary>
        /// Конструктор, инициализирующий матрицу
        /// в соответствие с вышепредставленной моделью
        /// </summary>
        /// <param name="_a"></param>
        /// <param name="_b"></param>
        /// <param name="_c"></param>
        /// <param name="_d"></param>
        /// <param name="_e"></param>
        /// <param name="_f"></param>
        public Matrix(double _a, double _b, double _c, double _d, double _e, double _f)
        {
            a = _a;
            b = _b;
            c = _c;
            d = _d;
            e = _e;
            f = _f;
        }

        /// <summary>
        /// Умножает вектор на текущую матрицу
        /// </summary>
        /// <param name="x"> Входная x-координата вектора </param>
        /// <param name="y"> Входная y-координата вектора </param>
        /// <param name="xx">Выходная x-координата вектора</param>
        /// <param name="yy">Выходная y-координата веткора</param>
        public void MultVector(double x, double y, out double xx, out double yy)
        {
            xx = a * x + c * y + e;
            yy = b * x + d * y + f;
        }

        /// <summary>
        /// Умножает текущую матрицу на матрицу m
        /// </summary>
        /// <param name="m"> Входная матрица </param>
        /// <returns> Возвращает результат умножения матриц</returns>
        public Matrix Mult(Matrix m)
        {
            double _a = a * m.a + b * m.c;
            double _b = a * m.b + b * m.d;

            double _c = c * m.a + d * m.c;
            double _d = c * m.b + d * m.d;

            double _e = e * m.a + f * m.c + m.e;
            double _f = e * m.b + f * m.d + m.f;

            return new Matrix(_a, _b, _c, _d, _e, _f);
        }

        /// <summary>
        /// Возвращает матрицу переноса
        /// с соответствующими параметрами
        /// </summary>
        /// <param name="tx"></param>
        /// <param name="ty"></param>
        /// <returns></returns>
        public static Matrix Translate(double tx, double ty)
        {
            return new Matrix(1, 0, 0, 1, tx, ty);
        }

        /// <summary>
        /// Возвращает матрицу масштабирования
        /// с соответствующими параметрами
        /// </summary>
        /// <param name="sx"></param>
        /// <param name="sy"></param>
        /// <returns></returns>
        public static Matrix Scale(double sx, double sy)
        {
            return new Matrix(sx, 0, 0, sy, 0, 0);
        }

        /// <summary>
        /// Возвращает элементы текущей матрицы 
        /// в виде списка в порядке инициализации
        /// </summary>
        /// <returns></returns>
        public List<double> GetValues()
        {
            return new List<double> { a, b, c, d, e, f };
        }
    }
}
