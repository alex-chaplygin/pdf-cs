using System.Collections.Generic;
using System;
using PdfCS;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PDFTest
{
    [TestClass]
    public class LZWTest
    {
        public void TestSequences(byte[] encodedSequence, byte[] decodedSequence)
        {
            CollectionAssert.AreEqual(LZW.Decode(encodedSequence, new Dictionary<string, object>()), decodedSequence);
        }

        [TestMethod]
        public void CorrectCase()
        {
            TestSequences(new byte[] { 0x80, 0x0B, 0x60, 0x50, 0x22, 0x0C, 0x0C, 0x85, 0x01 },
                          new byte[] { 45, 45, 45, 45, 45, 65, 45, 45, 45, 66 });
        }

        [TestMethod]
        public void IncorrectCase()
        {
            try
            {
                TestSequences(new byte[] { 0x80, 0x0B, 0x60, 0x50, 0x22, 0x10, 0x0C, 0x85, 0x01 },
                              new byte[] { 45, 45, 45, 45, 45, 65, 45, 45, 45, 66 });
                Assert.Fail();
            }
            catch(Exception)
            { }
        }
    }
}
