using System;
using PdfCS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace PDFTest
{
    [TestClass]
    public class NameTreeTest
    {
        public void LoadCache()
        {        
            Dictionary<string, object> rt = new Dictionary<string, object>();
            Dictionary<string, object> kid2 = new Dictionary<string, object>();
            Dictionary<string, object> kid3 = new Dictionary<string, object>();
        
            Tuple<int, int>[] rtkds =
            {
                new Tuple<int, int> (2, 0),
                new Tuple<int, int> (3, 0)
            };

            Tuple<int, int>[] kid2kds =
            {
                new Tuple<int, int> (4, 0),
                new Tuple<int, int> (5, 0)
            };

            Tuple<int, int>[] kid3kds =
            {
                new Tuple<int, int> (6, 0),
                new Tuple<int, int> (7, 0)
            };

            string[] kid2lts = { "Actinium", "Barium" };
            string[] kid3lts = { "Berkelium", "Calcium" };

            rt.Add("Kids", rtkds);
            kid2.Add("Kids", kid2kds);
            kid2.Add("Limits", kid2lts);
            kid3.Add("Kids", kid3kds);
            kid3.Add("Limits", kid3lts);

            Dictionary<string, object> names4 = new Dictionary<string, object>();
            Dictionary<string, object> names5 = new Dictionary<string, object>();
            Dictionary<string, object> names6 = new Dictionary<string, object>();
            Dictionary<string, object> names7 = new Dictionary<string, object>();

            string[] nm4lts = { "Actinium", "Antimony" };
            string[] nm5lts = { "Argon", "Barium" };
            string[] nm6lts = { "Berkelium", "Bohrium" };
            string[] nm7lts = { "Boron", "Calcium" };

            object[] nms4names =
            {
                "Actinium", new Tuple<int, int> (8, 0),
                "Aluminum", new Tuple<int, int> (9, 0),
                "Amerecium", new Tuple<int, int> (10, 0),
                "Antimony", new Tuple<int, int> (11, 0)
            };

            object[] nms5names =
            {
                "Argon", new Tuple<int, int> (12, 0),
                "Arsenic", new Tuple<int, int> (13, 0),
                "Astatine", new Tuple<int, int> (14, 0),
                "Barium", new Tuple<int, int> (15, 0)
            };

            object[] nms6names =
            {
                "Berkelium", new Tuple<int, int> (16, 0),
                "Beryllium", new Tuple<int, int> (17, 0),
                "Bismuth", new Tuple<int, int> (18, 0),
                "Bohrium", new Tuple<int, int> (19, 0)
            };

            object[] nms7names =
            {
                "Boron", new Tuple<int, int> (20, 0),
                "Bromine", new Tuple<int, int> (21, 0),
                "Cadmium", new Tuple<int, int> (22, 0),
                "Calcium", new Tuple<int, int> (23, 0)
            };

            names4.Add("Names", nms4names);
            names5.Add("Names", nms5names);
            names6.Add("Names", nms6names);
            names7.Add("Names", nms7names);

            names4.Add("Limits", nm4lts);
            names5.Add("Limits", nm5lts);
            names6.Add("Limits", nm6lts);
            names7.Add("Limits", nm7lts);

            PDFFile.objectCache.Add(1, rt);
            PDFFile.objectCache.Add(2, kid2);
            PDFFile.objectCache.Add(3, kid3);
            PDFFile.objectCache.Add(4, names4);
            PDFFile.objectCache.Add(5, names5);
            PDFFile.objectCache.Add(6, names6);
            PDFFile.objectCache.Add(7, names7);
            PDFFile.objectCache.Add(8, 89);
            PDFFile.objectCache.Add(9, 13);
            PDFFile.objectCache.Add(10, 95);
            PDFFile.objectCache.Add(11, 51);
            PDFFile.objectCache.Add(12, 18);
            PDFFile.objectCache.Add(13, 33);
            PDFFile.objectCache.Add(14, 85);
            PDFFile.objectCache.Add(15, 56);
            PDFFile.objectCache.Add(16, 97);
            PDFFile.objectCache.Add(17, 4);
            PDFFile.objectCache.Add(18, 83);
            PDFFile.objectCache.Add(19, 107);
            PDFFile.objectCache.Add(20, 5);
            PDFFile.objectCache.Add(21, 35);
            PDFFile.objectCache.Add(22, 48);
            PDFFile.objectCache.Add(23, 20);

        }

        public object SearchTest(char[] key)
        {
	    Dictionary<string, object> d;
            NameTree n = new NameTree((Dictionary<string, object>)PDFFile.LoadLink(new Tuple<int, int> (1, 0), out d));
            return n.Search(key);
        }

        [TestMethod]
        public void NameTreeSearchTest1()
        {
            LoadCache();
            char[] mas = "Bohrium".ToCharArray();
            object to = SearchTest(mas);
            Assert.AreEqual((int)to, 107);
        }

        [TestMethod]
        public void NameTreeSearchTest2()
        {
            char[] mas = "Aluminum".ToCharArray();
            object to = SearchTest(mas);
            Assert.AreEqual((int)to, 13);
        }

        [TestMethod]
        public void NameTreeSearchTest3()
        {
            char[] mas = "Actinium".ToCharArray();
            object to = SearchTest(mas);
            Assert.AreEqual((int)to, 89);
        }

        [TestMethod]
        public void NameTreeSearchTest4()
        {
            char[] mas = "Cesium".ToCharArray();
            object to = SearchTest(mas);
            Assert.AreEqual(to, null);
        }

        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
