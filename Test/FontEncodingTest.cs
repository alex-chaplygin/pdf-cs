using Microsoft.VisualStudio.TestTools.UnitTesting;
using PdfCS;
using System.Collections.Generic;

namespace PDFTest
{
    [TestClass]
    public class FontEncodingTest
    {
        [TestMethod]
        public void Differences()
        {
            object[][] differences = new object[][]
            {
                new object[] { 39, "quotesingle" },
                new object[] { 128, "Adieresis", "Aring", },
            };

            FontEncoding fontEncoding = new FontEncoding(
                new Dictionary<string, object> { 
                    { 
                        "Differences", 
                        differences
                    }
                }, 
                200
            );

            Assert.AreEqual(differences[0][1], fontEncoding.encoding[39]);
            Assert.AreEqual(differences[1][1], fontEncoding.encoding[128]);
            Assert.AreEqual(differences[1][2], fontEncoding.encoding[129]);
        }
    }
}
