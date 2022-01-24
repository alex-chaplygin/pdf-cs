using Microsoft.VisualStudio.TestTools.UnitTesting;
using PdfCS;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PDFTest
{
    [TestClass]
    public class FontEncodingTest
    {
        [TestMethod]
        public void Differences()
        {
            string s = @"
                    << /Type /Encoding
                    /Differences
                    [
                    39 /quotesingle
                    96 /grave
                    128 /Adieresis /Aring /Ccedilla /Eacute /Ntilde /Odieresis /Udieresis
                    /aacute /agrave /acircumflex /adieresis /atilde /aring /ccedilla
                    /eacute /egrave /ecircumflex /edieresis /iacute /igrave /icircumflex
                    /idieresis /ntilde /oacute /ograve /ocircumflex /odieresis /otilde
                    /uacute /ugrave /ucircumflex /udieresis /dagger /degree /cent
                    /sterling /section /bullet /paragraph /germandbls /registered
                    /copyright /trademark /acute /dieresis
                    ]
                    >>
                    ";

            Parser parser = new Parser(new MemoryStream(Encoding.UTF8.GetBytes(s)));
            var dic = (Dictionary<string, object>)parser.ReadToken();
            FontEncoding fontEncoding = new FontEncoding(dic, 200);
            Assert.AreEqual("quotesingle", fontEncoding.encoding[39]);
            Assert.AreEqual("grave", fontEncoding.encoding[96]);
            Assert.AreEqual("Adieresis", fontEncoding.encoding[128]);
            Assert.AreEqual("Aring", fontEncoding.encoding[129]);
            Assert.AreEqual("dieresis", fontEncoding.encoding[172]);
        }

        [TestMethod]
        public void FillEncodingStandardEncoding()
        {
            string s = @"
                << /Type /Encoding
                /BaseEncoding /StandardEncoding
                /Differences
                [
                39 /quotesingle
                96 /grave
                128 /Adieresis /Aring /Ccedilla /Eacute /Ntilde /Odieresis /Udieresis
                /aacute /agrave /acircumflex /adieresis /atilde /aring /ccedilla
                /eacute /egrave /ecircumflex /edieresis /iacute /igrave /icircumflex
                /idieresis /ntilde /oacute /ograve /ocircumflex /odieresis /otilde
                /uacute /ugrave /ucircumflex /udieresis /dagger /degree /cent
                /sterling /section /bullet /paragraph /germandbls /registered
                /copyright /trademark /acute /dieresis
                ]
                >>
                ";

            Parser parser = new Parser(new MemoryStream(Encoding.UTF8.GetBytes(s)));
            var dic = (Dictionary<string, object>)parser.ReadToken();

            FontEncoding fontEncoding = new FontEncoding(dic, 350);

            Assert.AreEqual("A", fontEncoding.encoding[0101]);
            Assert.AreEqual("AE", fontEncoding.encoding[0341]);
        }

        [TestMethod]
        public void FillEncodingMacRomanEncoding()
        {
            string s = @"
                << /Type /Encoding
                /BaseEncoding /MacRomanEncoding
                /Differences
                [
                39 /quotesingle
                96 /grave
                128 /Adieresis /Aring /Ccedilla /Eacute /Ntilde /Odieresis /Udieresis
                /aacute /agrave /acircumflex /adieresis /atilde /aring /ccedilla
                /eacute /egrave /ecircumflex /edieresis /iacute /igrave /icircumflex
                /idieresis /ntilde /oacute /ograve /ocircumflex /odieresis /otilde
                /uacute /ugrave /ucircumflex /udieresis /dagger /degree /cent
                /sterling /section /bullet /paragraph /germandbls /registered
                /copyright /trademark /acute /dieresis
                ]
                >>
                ";

            Parser parser = new Parser(new MemoryStream(Encoding.UTF8.GetBytes(s)));
            var dic = (Dictionary<string, object>)parser.ReadToken();

            FontEncoding fontEncoding = new FontEncoding(dic, 315);


            Assert.AreEqual("Agrave", fontEncoding.encoding[0313]);
            Assert.AreEqual("AE", fontEncoding.encoding[0256]);
        }

        [TestMethod]
        public void FillEncodingWinAnsiEncoding()
        {
            string s = @"
                << /Type /Encoding
                /BaseEncoding /WinAnsiEncoding
                /Differences
                [
                39 /quotesingle
                96 /grave
                128 /Adieresis /Aring /Ccedilla /Eacute /Ntilde /Odieresis /Udieresis
                /aacute /agrave /acircumflex /adieresis /atilde /aring /ccedilla
                /eacute /egrave /ecircumflex /edieresis /iacute /igrave /icircumflex
                /idieresis /ntilde /oacute /ograve /ocircumflex /odieresis /otilde
                /uacute /ugrave /ucircumflex /udieresis /dagger /degree /cent
                /sterling /section /bullet /paragraph /germandbls /registered
                /copyright /trademark /acute /dieresis
                ]
                >>
                ";

            Parser parser = new Parser(new MemoryStream(Encoding.UTF8.GetBytes(s)));
            var dic = (Dictionary<string, object>)parser.ReadToken();

            FontEncoding fontEncoding = new FontEncoding(dic, 350);

            Assert.AreEqual("A", fontEncoding.encoding[0101]);
            Assert.AreEqual("AE", fontEncoding.encoding[0306]);
        }

        [TestMethod]
        public void FillEncodingPDFDocEncoding()
        {
            string s = @"
                << /Type /Encoding
                /BaseEncoding /PDFDocEncoding
                /Differences
                [
                39 /quotesingle
                96 /grave
                128 /Adieresis /Aring /Ccedilla /Eacute /Ntilde /Odieresis /Udieresis
                /aacute /agrave /acircumflex /adieresis /atilde /aring /ccedilla
                /eacute /egrave /ecircumflex /edieresis /iacute /igrave /icircumflex
                /idieresis /ntilde /oacute /ograve /ocircumflex /odieresis /otilde
                /uacute /ugrave /ucircumflex /udieresis /dagger /degree /cent
                /sterling /section /bullet /paragraph /germandbls /registered
                /copyright /trademark /acute /dieresis
                ]
                >>
                ";

            Parser parser = new Parser(new MemoryStream(Encoding.UTF8.GetBytes(s)));
            var dic = (Dictionary<string, object>)parser.ReadToken();

            FontEncoding fontEncoding = new FontEncoding(dic, 350);

            Assert.AreEqual("A", fontEncoding.encoding[0101]);
            Assert.AreEqual("AE", fontEncoding.encoding[0306]);
            Assert.AreEqual("grave", fontEncoding.encoding[96]);
        }
    }
}
