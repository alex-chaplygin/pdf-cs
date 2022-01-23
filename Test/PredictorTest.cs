using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using PdfCS;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PDFTest
{
    [TestClass]
    public class PredictorTest
    {
public void TestSequences(byte[] encodedSequence, byte[] decodedSequence, int colors, int bpp, int columns)
        {
            CollectionAssert.AreEqual(Predictor.Decode(encodedSequence, 1, colors, bpp, columns), decodedSequence);
        }

        [TestMethod]
        public void CorrectCase()
        {
            TestSequences(new byte[] {  1,  1,  1,
                                        4,  1,  1 },

                          new byte[] {  1,  2,  3,
                                        4,  5,  6  }, 1, 1, 3);
        }

        [TestMethod]
        public void IncorrectCase()
        {
            try
            {
                TestSequences(new byte[] {  3,  1,  1,
                                            4,  5,  1  },

                              new byte[] {  1,  2,  3,
                                            4,  5,  6  }, 1, 1, 3);
                Assert.Fail();
            }
            catch (Exception)
            { }
        }

        [TestMethod]
        public void CorrectCaseColors()
        {
            TestSequences(new byte[] {  1, 1, 1,    4, 1, 1,
                                        1, 1, 1,    1, 1, 1},

                          new byte[] {  1, 1, 1,    5, 2, 2,
                                        1, 1, 1,    2, 2, 2}, 3, 1, 2);
        }

        [TestMethod]
        public void IncorrectCaseColors()
        {
            try
            {
                TestSequences(new byte[] {  5, 1, 2,    4, 1, 1,
                                            1, 1, 1,    1, 1, 1},

                              new byte[] {  1, 1, 1,    5, 2, 2,
                                            1, 1, 1,    2, 2, 2}, 3, 1, 2);
                Assert.Fail();
            }
            catch (Exception)
            { }
        }

        [TestMethod]
        public void CorrectCase4bit()
        {
            TestSequences(new byte[] {  0x11, 0x11,
                                        0x51, 0x11 },

                          new byte[] {  0x12, 0x34,
                                        0x56, 0x78}, 1, 4, 4);
        }
    }
}
