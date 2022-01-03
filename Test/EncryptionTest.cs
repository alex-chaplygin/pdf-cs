using Microsoft.VisualStudio.TestTools.UnitTesting;
using PdfCS;
using System.IO;

namespace PDFTest
{
    [TestClass]
    public class EncryptionTest
    {
        
        [TestMethod]
        public void Test()
        {
            PDFFile.Open(File.OpenRead("pdfs/test2.pdf"));
            Assert.IsTrue(Encryption.UserAuthentificate(""));
        }
    }
}
