using System.Collections.Generic;
using System;
using PdfCS;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PDFTest
{
    [TestClass]
    public class ASCII85Test
    {
        public static byte[] ConvertToBytes(string s)
        {
            List<byte> res = new List<byte>();

            foreach (var cur in s)
                res.Add((byte)cur);

            return res.ToArray();
        }

        public void TestSequences(byte[] encodedSequence, byte[] decodedSequence)
        {
            CollectionAssert.AreEqual(ASCII85.Decode(encodedSequence), decodedSequence);
        }

        [TestMethod]
        public void CorrectCase()
        {
            TestSequences(ConvertToBytes("5uU-B8N8RM<+U,mBl7Q+:i^JaATMn~>"), 
                          ConvertToBytes("ASCII85 Testing Process"));
        }

        [TestMethod]
        public void IncorrectCaseZ()
        {
            try
            {
                TestSequences(ConvertToBytes("5uU-Bzzz8N8RM<+U,mBl7Q+:i^JazzATMn~>"), 
                              ConvertToBytes("ASCII85 Testing Process"));
                Assert.Fail();
            }
            catch (Exception) { }
        }

        [TestMethod]
        public void InvalidEndOfDataError()
        {
            try
            {
                TestSequences(ConvertToBytes("5uU-B8N8RM<+U,mBl7Q+:i^JaATMn~d"), 
                              ConvertToBytes("ASCII85 Testing Process"));
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.Message, "Неверный конец данных ~> в ASCII85Decode");
            }
        }

        [TestMethod]
        public void MissingEndOfDataError()
        {
            try
            {
                TestSequences(ConvertToBytes("5uU-B8N8RM<+U,mBl7Q+:i^JaATMn"), 
                              ConvertToBytes("ASCII85 Testing Process"));
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.Message, "Отсутствует конец данных ~> в ASCII85Decode");
            }
        }

        [TestMethod]
        public void IncorrectCharacterError()
        {
            try
            {
                TestSequences(ConvertToBytes("5uwU-B8N8wRM<+U,ml7Q+:i^JaATMn~>"), 
                              ConvertToBytes("ASCII85 Testing Process"));
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.Message, "Неверный символ в ASCII85Decode");
            }
        }
    }
}
