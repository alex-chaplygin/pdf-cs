using System;
using System.Collections.Generic;
using System.IO;
using PdfCS;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PDFTest
{
    [TestClass]
    public class ParserTest
    {
	public void TestToken(string s, Type t, object o)
	{
	    if (o != null && o.GetType() != t)
		Assert.Fail();
	    MemoryStream m = new MemoryStream();
	    byte[] b = new byte[s.Length];
	    for (int i = 0; i < s.Length; i++)
		b[i] = (byte)s[i];
	    m.Write(b, 0, s.Length);
	    m.Seek(0, SeekOrigin.Begin);
	    Parser p = new Parser(m);
	    p.NextChar();
	    object o2 = p.ReadToken();
	    //Console.WriteLine($"\nТребуется = {Convert.ChangeType(o, t)} Получено = {Convert.ChangeType(o2, t)}");
	    Assert.AreEqual(Convert.ChangeType(o2, t), Convert.ChangeType(o, t));
	}
	
        [TestMethod]
        public void Null()
	{
	    TestToken("null", typeof(object), null);
	}

        [TestMethod]
        public void NullError()
	{
	    try {
		TestToken("  nul ", typeof(object), null);
		Assert.Fail();
	    } catch (Exception e) {
		Assert.AreEqual(e.Message, "Ошибка в null");
	    }
	}

        [TestMethod]
        public void Number()
	{
	    TestToken("  10", typeof(int), 10);
	}

	[TestMethod]
	public void True()
        {
	    TestToken("true", typeof(bool), true);
        }

	[TestMethod]
	public void False()
        {
	    TestToken("false", typeof(bool), false);
	}

	[TestMethod]
	public void TrueError()
	{
            try
            {
		TestToken("tru", typeof(bool), true);
		Assert.Fail();
	    }
	    catch(Exception ex)
            {
		Assert.AreEqual(ex.Message, "Ошибка true Boolean");
            }
	}

	[TestMethod]
	public void FalseError()
	{
	    try
	    {
		TestToken("fals", typeof(bool), false);
		Assert.Fail();
	    }
	    catch (Exception ex)
	    {
		Assert.AreEqual(ex.Message, "Ошибка false Boolean");
	    }
	}

	[TestMethod]
	public void ErrorNumObject()
	{
	    try
	    {
		ReadIndirectObjectTest("K", null);
	    }
	    catch (Exception e)
	    {
		Assert.AreEqual(e.Message, "Неверный номер объекта (objNum)");
	    }
	}

	[TestMethod]
	public void ErrorGenObject()
	{
	    try
	    {
		ReadIndirectObjectTest("10 ,", null);
	    }
	    catch (Exception e)
	    {
		Assert.AreEqual(e.Message, "Неверный номер поколения (genNum)");
	    }
	}

	[TestMethod]
	public void ErrorObject()
	{
	    try
	    {
		ReadIndirectObjectTest("10 0 o", null);
	    }
	    catch (Exception e)
	    {
		Assert.AreEqual(e.Message, "Ожидался параметр obj");
	    }
	}

	[TestMethod]
	public void EndobjObject()
	{
	    ReadIndirectObjectTest("10 0 obj \n 5 \nendobj", 5);
	}

	[TestMethod]
	public void ErrorEndobjObject()
	{
	    try
	    {
		ReadIndirectObjectTest("10 0 obj y", null);
	    }
	    catch (Exception e)
	    {
		Assert.AreEqual(e.Message, "Ожидался параметр endobj");
	    }
	}

	[TestMethod]
	public void ErrorStreamObject()
	{
	    ReadIndirectObjectTest("10 0 obj <5 5>> stream endobj", null);
	}

	public void ReadIndirectObjectTest(string str, object obj)
	{
	    Dictionary<string, object> dict;
	    MemoryStream m = new MemoryStream();
	    byte[] b = new byte[str.Length];
	    for (int i = 0; i < str.Length; i++)
		b[i] = (byte)str[i];
	    m.Write(b, 0, str.Length);
	    m.Seek(0, SeekOrigin.Begin);
	    Parser p = new Parser(m);
	    p.NextChar();
	    object o2 = p.ReadIndirectObject(out dict);
	    Assert.AreEqual(o2, obj);
	}
    }
}
