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

	public void TestSequencesColors(byte[] encodedSequence, byte[] decodedSequence)
        {
            CollectionAssert.AreEqual(Predictor.Decode(encodedSequence, 1, 2, 1, 3), decodedSequence);
        }

        [TestMethod]
        public void CorrectCase()
        {
            TestSequences(new byte[] {  1, 1, 1, 
                                        4, 1, 1 },
                          new byte[] {  1, 2, 3,
                                        4, 5, 6 });
        }

	[TestMethod]
        public void CorrectCaseColors()
        {
            TestSequencesColors(new byte[] {    1, 1, 1,   4, 1, 1,
                                                1, 1, 1,   1, 1, 1},

                          new byte[] {  1, 1, 1,  5, 2, 2,
                                        1, 1, 1,  2, 2, 2});
        }
    }
}
