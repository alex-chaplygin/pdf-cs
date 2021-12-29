using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfCS
{
    /// <summary>
    /// Прямоугольная область
    /// </summary>
    public class Rectangle
    {
        /// <summary>
        /// Левый нижний угол
        /// </summary>
        public double llx;
        public double lly;
        /// <summary>
        /// Правый верхний угол
        /// </summary>
        public double urx;
        public double ury;

        public Rectangle(double x1, double y1, double x2, double y2)
        {
            llx = x1;
            lly = y1;

            urx = x2;
            ury = y2;
        }
    }
}
