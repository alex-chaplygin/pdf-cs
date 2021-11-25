using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfCS
{
    public class Matrix
    {
        /// <summary>
        /// конструктор
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="e"></param>
        /// <param name="f"></param>
        public Matrix(double a, double b, double c, double d, double e, double f)
        {

        }

        /// <summary>
        /// конструктор
        /// </summary>
        public Matrix()
        { 

        }

        /// <summary>
        /// единичная матрица
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name=""></param>
        public void MultVector(double x, double y, double xx, double yy)
        {

        }

        /// <summary>
        /// Умножает текущую матрицу на матрицу m
        /// </summary>
        /// <param name="m"></param>
        /// <returns>возвращает новую матрицу</returns>
        public Matrix Mult(Matrix m)
        {
            return m;
        }
    }
}
