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

        public void TestSequences(string encodedSequence, string decodedSequence)
        {
            byte[] res    = ASCII85.Decode(ConvertToBytes(encodedSequence));
            byte[] answer = ConvertToBytes(decodedSequence);

            if (res.Length != answer.Length)
                Assert.Fail();

            for (int i = 0; i < res.Length; i++)
                if (res[i] != answer[i])
                    Assert.Fail();
        }

        [TestMethod]
        public void CorrectCase()
        {
            TestSequences("5uU-B8N8RM<+U,mBl7Q+:i^JaATMn~>", "ASCII85 Testing Process");
        }

        [TestMethod]
        public void IncorrectCaseZ()
        {
            try
            {
                TestSequences("5uU-Bzzz8N8RM<+U,mBl7Q+:i^JazzATMn~>", "ASCII85 Testing Process");
                Assert.Fail();
            }
            catch (Exception) { }
        }

        [TestMethod]
        public void InvalidEndOfDataError()
        {
            try
            {
                TestSequences("5uU-B8N8RM<+U,mBl7Q+:i^JaATMn~d", "ASCII85 Testing Process");
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
                TestSequences("5uU-B8N8RM<+U,mBl7Q+:i^JaATMn", "ASCII85 Testing Process");
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
                TestSequences("5uwU-B8N8wRM<+U,ml7Q+:i^JaATMn~>", "ASCII85 Testing Process");
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.Message, "Неверный символ в ASCII85Decode");
            }
        }
    }
}
