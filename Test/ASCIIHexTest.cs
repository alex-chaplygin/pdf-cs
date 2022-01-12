using System;
using System.Collections.Generic;
using System.Linq;
using PdfCS;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace PDFTest
{
    [TestClass]
    public class ASCIIHexTest
    {
        public byte[] Convert(string s)
        {
            List<byte> res = new List<byte>();

            foreach (var cur in s)
                res.Add((byte)cur);

            return res.ToArray();
        }

        public void TestDecode(byte[] encodeData, byte[] decodeData)
        {
            CollectionAssert.AreEqual(ASCIIHex.Decode(encodeData), decodeData);
        }

        [TestMethod]
        public void CorrectCase()
        {
            TestDecode(Convert("48 65 6c 6c 6f 20 57 6f 72 6c 64 21>"),
                Convert("Hello World!"));
        }

        [TestMethod]
        public void IncorrectData()
        {
            try
            {
                TestDecode(Convert("SGVsbG8gV29ybGQh"),
                   Convert("Hello World!"));
                Assert.Fail();
            }
            catch(Exception ex)
            {
                Assert.AreEqual(ex.Message, "Неверный символ ASCIIHex: S");
            }
        }

        [TestMethod]
        public void CorrectCaseOdd()
        {
            TestDecode(Convert("48 65 6c 6c 6f 20 57 6f 72 6c 64 9>"),
                Convert("Hello World	"));
        }

        [TestMethod]
        public void LastNotBracket()
        {
            TestDecode(Convert("48 65 6c 6c 6f 20 57 6f 72 6c 64 21"),
                Convert("Hello World!"));
        }
    }
}
