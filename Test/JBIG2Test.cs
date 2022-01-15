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
	    byte[] s = new byte[] { 0x00, 0x00, 0x00, 0x20, 0x86, 0x6B, 0x02, 0x1E, 0x05, 0x04, 0x00, 0x00, 0x00, 0x20};
            JBIG2Segment j = new JBIG2Segment(new MemoryStream(s));
            Assert.AreEqual(j.number, (uint)0x20);
	    Assert.AreEqual(j.flags, (byte)0x86);
            Assert.AreEqual(j.type, 6);
            Assert.AreEqual(j.refCount, (byte)0x6B);
	    CollectionAssert.AreEqual(j.references, new int[] { 2, 0x1E, 0x05 });
            Assert.AreEqual(j.page, (byte)0x04);
            Assert.AreEqual(j.length, (uint)0x20);
        }
    }
}
