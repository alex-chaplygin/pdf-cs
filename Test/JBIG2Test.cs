using Microsoft.VisualStudio.TestTools.UnitTesting;
using PdfCS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDFTest
{
    [TestClass]
    public class JBIG2Test
    {
        [TestMethod]
        public void SegmentTest()
        {
            byte[] s = new byte[] {0x00, 0x00, 0x00, 0x01, 0xff, 0x02, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x03, 0x10, 0x00, 0x00, 0x00, 0x20};
            JBIG2Segment j = new JBIG2Segment(new MemoryStream(s));
            Assert.AreEqual(j.number, (uint)1);
	    Assert.AreEqual(j.flags, (byte)0xff);
            Assert.AreEqual(j.refCount, (byte)2);
	    CollectionAssert.AreEqual(j.references, new uint[] { 2, 3 });
            Assert.AreEqual(j.page, (byte)0x10);
            Assert.AreEqual(j.length, (uint)0x20);
        }
    }
}
