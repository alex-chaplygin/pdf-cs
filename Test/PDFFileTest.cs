using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using PdfCS;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PDFTest
{
    [TestClass]
    public class PDFFileTest
    {
        void Test(string s, PDFFile.XRefEntry[] table)
        {
            MemoryStream m = new MemoryStream();
            byte[] b = new byte[s.Length];
            for (int i = 0; i < s.Length; i++)
                b[i] = (byte)s[i];
            m.Write(b, 0, b.Length);
            m.Seek(0, SeekOrigin.Begin);
            PDFFile.stream = m;      
            PDFFile.parser = new Parser(m);
            PDFFile.ReadCrossReferenceTable();
            CollectionAssert.AreEqual(table, PDFFile.xrefTable);
        }

        [TestMethod]
        public void ReadCrossReferenceTableTest()
        {
            string s = "xref\n" +
                        "0 6\n" +
            "0000000003 65535 f\n" +
            "0000000017 00000 n\n" +
            "0000000081 00000 n\n" +
            "0000000000 00007 f\n" +
            "0000000331 00000 n\n" +
            "0000000409 00000 n\n" +
		"trailer \n" +
            "<< /Size 22 \n" +
            "/Root 2 0 R \n" +
            "/Info 1 0 R \n" +
            "/ID[ <81b14aafa313db63dbd6f981e49f94f4> \n" +
            "<81b14aafa313db63dbd6f981e49f94f4> ] \n" +
            ">> \n";
            PDFFile.XRefEntry[] table = new PDFFile.XRefEntry[]
            {
                new PDFFile.XRefEntry(0, 0, true),
                new PDFFile.XRefEntry(17, 0, false),
                new PDFFile.XRefEntry(81, 0, false),
                new PDFFile.XRefEntry(0, 0, true),
                new PDFFile.XRefEntry(331, 0, false),
                new PDFFile.XRefEntry(409, 0, false),
            };
            Test(s, table);
	    Assert.AreEqual(PDFFile.root.Item1, 2);
            Assert.AreEqual(PDFFile.info.Item1, 1);
        }
	
        [TestMethod]
        public void ReadCrossReferenceTableTest2()
        {
            string s = "xref\n" +
            "0 1\n" +
            "0000000000 65535 f\n" +
            "3 1\n" +
            "0000025325 00000 n\n" +
            "23 2\n" +
            "0000025518 00002 n\n" +
            "0000025635 00000 n\n" +
            "30 1\n" +
            "0000025777 00000 n\n" +
		"trailer \n" +
            "<< /Size 22 \n" +
            "/Root 2 0 R \n" +
            "/Info 1 0 R \n" +
            "/ID[ <81b14aafa313db63dbd6f981e49f94f4> \n" +
            "<81b14aafa313db63dbd6f981e49f94f4> ] \n" +
            ">> \n";          
            PDFFile.XRefEntry[] table = new PDFFile.XRefEntry[]
             {
                new PDFFile.XRefEntry(0, 0, true),
                new PDFFile.XRefEntry(),
                new PDFFile.XRefEntry(),
                new PDFFile.XRefEntry(25325, 0, false),
                new PDFFile.XRefEntry(),
                new PDFFile.XRefEntry(),
                new PDFFile.XRefEntry(),
                new PDFFile.XRefEntry(),
                new PDFFile.XRefEntry(),
                new PDFFile.XRefEntry(),
                new PDFFile.XRefEntry(),
                new PDFFile.XRefEntry(),
                new PDFFile.XRefEntry(),
                new PDFFile.XRefEntry(),
                new PDFFile.XRefEntry(),
                new PDFFile.XRefEntry(),
                new PDFFile.XRefEntry(),
                new PDFFile.XRefEntry(),
                new PDFFile.XRefEntry(),
                new PDFFile.XRefEntry(),
                new PDFFile.XRefEntry(),
                new PDFFile.XRefEntry(),
                new PDFFile.XRefEntry(),
                new PDFFile.XRefEntry(25518, 2, false),
                new PDFFile.XRefEntry(25635, 0, false),
                new PDFFile.XRefEntry(),
                new PDFFile.XRefEntry(),
                new PDFFile.XRefEntry(),
                new PDFFile.XRefEntry(),
                new PDFFile.XRefEntry(),
                new PDFFile.XRefEntry(25777, 0, false)
             };
            Test(s, table); 
        }

	[TestMethod]
        public void OpenTest()
        {
            string path = "pdfs/test2.pdf";
            Assert.IsTrue(File.Exists(path));

            FileStream fileStream = File.OpenRead(path);
            PDFFile.Open(fileStream);
            Assert.IsTrue(PDFFile.xrefTable.Length > 0);
        }

	[TestMethod]
        public void LoadXRefStreamTest()
        {
            string s = "20 0 obj\n" +
            "<</Type/XRef/Size 3/W [1 2 1] /Length 37 /Filter/ASCIIHexDecode>>\n" +
            "stream\n" +
            "00 1111 22    01 1000 00   02 2000 02\n" +
            "endstream\nendobj\n";
            MemoryStream m = new MemoryStream(s.Select<char, byte>(x => (byte)x).ToArray());
            PDFFile.parser = new Parser(m);
            PDFFile.parser.NextChar();
            PDFFile.LoadXRefStream();

            PDFFile.XRefEntry[] table = new PDFFile.XRefEntry[]
            {
                new PDFFile.XRefEntry(0, 0x22, true),
                new PDFFile.XRefEntry(0x10, 0, false),
                new PDFFile.XRefEntry(true, 0x20, 2),
            };
            CollectionAssert.AreEqual(table, PDFFile.xrefTable);
        }
    }
}
