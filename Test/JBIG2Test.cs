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
            Assert.AreEqual(j.refCount, (uint)0x6B);
	    CollectionAssert.AreEqual(j.references, new int[] { 2, 0x1E, 0x05 });
            Assert.AreEqual(j.page, (uint)0x04);
            Assert.AreEqual(j.length, (uint)0x20);
        }

	[TestMethod]
        public void SegmentTest2()
        {
            byte[] s = new byte[] { 0x00, 0x00, 0x02, 0x34, 0x40, 0x00, 0x00, 0x00, 0x09, 0x02, 0xfd, 0x01, 0x00, 0x02, 0x01, 0x02, 0x02, 0x02, 0x03, 0x02, 0x00, 0x00, 0x04, 0x01, 0x00, 0x00, 0x00, 0x20 };
            JBIG2Segment j = new JBIG2Segment(new MemoryStream(s));
            Assert.AreEqual(j.number, (uint)564);
            Assert.AreEqual(j.flags, (byte)0x40);
            Assert.AreEqual(j.type, 0);
            Assert.AreEqual(j.refCount, (uint)9);
            CollectionAssert.AreEqual(j.references, new int[] { 1, 0, 2, 1, 2, 2, 2, 3, 2 });
            Assert.AreEqual(j.page, (uint)1025);
            Assert.AreEqual(j.length, (uint)0x20);
        }

	public static string Squeeze(string str)
        {            
            int index;
            string c1 = ""; //строка с повторениями
            string c2; //текущий символ
            int k = 1; //кол-во повторений символа
	    char[] letters = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
            str = str.Replace("~", "~~");
            for (int i = 0; i < str.Length - 1; i++)
            {
                c2 = str[i].ToString();
                if (str[i] == str[i + 1] && str[i] != '~')
                {
                    k++;
                    c1 += str[i];
                }
                else if (k >= 4)
                {
                    c1 += str[i];
                    str = str.Replace(c1, "~" + c2 + letters[k - 4]);
                    index = i;
                    k = 1;
                    c1 = "";
                    i = 0;
                    continue;
                }                
            }           
            return str;
        }

	public void Test(string input, string output)
        {
            string o2 = Squeeze(input);

            //Equals(o2, output);
            //string.Equals(o2, output);
            string.Compare(o2, output);
            //Assert.AreEqual(o2, output);
        }

        [TestMethod]
        public void Test1()
        {
            Test("abc1111~44444qqeee", "abc~1a~~~4bqqeee");
        }
        [TestMethod]
        public void Test2()
        {
//          Test("dg333baggggshu888888n", "dg333ba~gashu~8cn");
        }
        [TestMethod]
        public void Test3()
        {
            Test("000000000", "~0g");
        }
        public void Test4()
        {
            Test("~~aaaa", "~~~~~aa");
        }
    }
}
