using Microsoft.VisualStudio.TestTools.UnitTesting;
using PdfCS;
using System.IO;
using System.Linq;

namespace PDFTest
{
    [TestClass]
    public class EncryptionTest
    {
        
        [TestMethod]
        public void TestAuth()
        {
            PDFFile.Open(File.OpenRead("pdfs/test2.pdf"));
            Assert.IsTrue(Encryption.UserAuthentificate(""));
        }

        [TestMethod]
        public void TestRC4()
        {
	    byte[] key = new byte[] {1, 2, 3, 4, 5};
	    byte[] s = "TEST".Select<char, byte>(x => (byte)x).ToArray();
	    byte[] enc = Encryption.DecodeRC4(s, key);
            CollectionAssert.AreEqual(Encryption.DecodeRC4(enc, key), s);
        }

	[TestMethod]
        public void TestPadString()
        {
            CollectionAssert.AreEqual(
                new byte[] {
                    0x28, 0xBF, 0x4E, 0x5E, 0x4E, 0x75, 0x8A, 0x41,
                    0x64, 0x00, 0x4E, 0x56, 0xFF, 0xFA, 0x01, 0x08,
                    0x2E, 0x2E, 0x00, 0xB6, 0xD0, 0x68, 0x3E, 0x80,
                    0x2F, 0x0C, 0xA9, 0xFE, 0x64, 0x53, 0x69, 0x7A
                }, 
                Encryption.PadString("")
            );
        }
    }
}
