using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PdfCS;

namespace PDFTest
{
    [TestClass]
    public class MatrixTest
    {
        [TestMethod]
        public void MultUnitMatrixByMatrix()
        {
            CollectionAssert.AreEqual(new Matrix().MultMatrix(new Matrix(2, 3, 3, 1, 6, 5)).GetValues(),
                                      new Matrix(2, 3, 3, 1, 6, 5).GetValues());
            CollectionAssert.AreEqual(new Matrix(2, 3, 3, 1, 6, 5).MultMatrix(new Matrix()).GetValues(),
                                      new Matrix(2, 3, 3, 1, 6, 5).GetValues());
        }

        [TestMethod]
        public void MultVectorByUnitMatrix()
        {
            const double inX = 3;
            const double inY = 4;

            double outX, outY;

            new Matrix().MultVector(inX, inY, out outX, out outY);

            CollectionAssert.AreEqual(new List<double> { inX, inY }, new List<double> { outX, outY });
        }

        [TestMethod]
        public void MultCombineMatrix1()
        {
            CollectionAssert.AreEqual(new Matrix(2, 3, 3, 1, 6, 5).MultMatrix(Matrix.Translate(3, 2))
                                                                  .MultMatrix(Matrix.Scale(4, 6))
                                                                  .MultMatrix(new Matrix())
                                                                  .GetValues(),
                                      new Matrix(8, 18, 12, 6, 36, 42).GetValues());
        }

        [TestMethod]
        public void MultCombineMatrix2()
        {
            CollectionAssert.AreEqual(new Matrix(10, 0, 3, 0, 6, 5).MultMatrix(Matrix.Translate(0, 2))
                                                                   .MultMatrix(Matrix.Scale(4, 0))
                                                                   .MultMatrix(new Matrix())
                                                                   .GetValues(),
                                      new Matrix(40, 0, 12, 0, 24, 0).GetValues());
        }

        [TestMethod]
        public void MultCombineVectorByMatrix()
        {
            double x = -3;
            double y = 10;

            Matrix.Translate(3, 2).MultMatrix(Matrix.Scale(4, 6))
                                  .MultVector(x, y, out x, out y);

            CollectionAssert.AreEqual(new List<double> { 0, 72 }, new List<double> { x, y });
        }
    }
}
