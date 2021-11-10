using System;
using PdfCS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace PDFTest
{
    [TestClass]
    public class RunLengthTest
    {
        private const int endOfData = 128;
        public void DecodeTest(byte[] array, byte[] returnArray)
        {
            byte[] obj = RunLength.Decode(array);
            if (obj.Length != returnArray.Length)
                Assert.Fail();
            for (int i = 0; i < returnArray.Length; i++)
                if (obj[i] != returnArray[i])
                    Assert.Fail();
        }

        [TestMethod]
        public void NoEndData()
        {
            try
            {
                DecodeTest(new byte[2] { 129, 130 }, null);
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, "RunLength - нет конца данных");
            }
        }

        [TestMethod]
        public void EndData()
        {
            DecodeTest(new byte[11] { 254, 2, 0, 0, 2, 3, 254, 1, 0, 255, 128 }, new byte[8] { 2, 2, 2, 0, 3, 254, 1, 255 });
        }
    }
}
