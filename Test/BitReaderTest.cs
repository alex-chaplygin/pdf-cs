using System;
using PdfCS;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PDFTest
{
    [TestClass]
    public class BitReaderTest
    {
        public void TestBitsLS(byte[] data, int[] num, int[] res)
        {
            int r2;
            BitReader bit = new BitReader(data);
            for (int i = 0; i < num.Length; i++)
            {
                r2 = bit.GetBitsLS(num[i]);
                if (r2 != res[i])
                    Assert.Fail();
            }
        }

        public void TestBitsMS(byte[] data, int[] num, int[] res)
        {
            int r2;
	    BitReader bit = new BitReader(data);
            for (int i = 0; i < num.Length; i++)
            {
                r2 = bit.GetBitsMS(num[i]);
                if (r2 != res[i])
                    Assert.Fail();
            }
        }

        [TestMethod]
        public void BitsLS()
        {
            TestBitsLS(new byte[1] { 0x95 }, new int[1] { 5 }, new int[1] { 0x15 });
        }

        [TestMethod]
        public void BitsLSError()
        {
            try
            {
                TestBitsLS(new byte[1] { 0x5 }, new int[1] { 9 }, new int[1] { 0x1 });
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.Message, "Выход за пределы массива -> BitReader, GetBit");
            }
        }

        [TestMethod]
        public void BitsMS()
        {
            TestBitsMS(new byte[1] { 0x80 }, new int[1] { 2 }, new int[1] { 0x02 });
        }

        [TestMethod]
        public void BitsMSError()
        {
            try
            {
                TestBitsMS(new byte[1] { 0x1 }, new int[1] { 9 }, new int[1] { 0x1 });
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.Message, "Выход за пределы массива -> BitReader, GetBit");
            }
        }

        [TestMethod]
        public void BitsMSMeet()
        {
            TestBitsMS(new byte[2] { 0x81, 0xF }, new int[2] { 6, 8 }, new int[2] { 0x20, 0x43 });
        }

        [TestMethod]
        public void BitsLSMeet()
        {
            TestBitsLS(new byte[2] { 0xBF, 0x96 }, new int[2] { 5, 5 }, new int[2] { 0x1F, 0x15 });
        }
    }
}
