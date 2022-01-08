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
        public void TestSequences(byte[] encodedSequence, byte[] decodedSequence)
        {
            CollectionAssert.AreEqual(Predictor.Decode(encodedSequence, 1, 1, 1, 3), decodedSequence);
        }

        [TestMethod]
        public void CorrectCase()
        {
            TestSequences(new byte[] {  1, 1, 1, 
                                        4, 1, 1 },

                          new byte[] {  1, 2, 3,
                                        4, 5, 6 });
        }
    }
}
