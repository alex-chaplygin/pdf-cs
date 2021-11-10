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
	    if (t == typeof(byte[]))
		CollectionAssert.AreEqual((System.Collections.ICollection)Convert.ChangeType(o2, t),
					  (System.Collections.ICollection)Convert.ChangeType(o, t));
	    else
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

	[TestMethod]
	public void ReadNumberInt1()
	{
		TestToken("43445", typeof(int), 43445);
	}

	[TestMethod]
	public void ReadNumberInt2()
	{
		TestToken("+17", typeof(int), +17);
	}

	[TestMethod]
	public void ReadNumberInt3()
	{
		TestToken("-98", typeof(int), -98);
	}

	[TestMethod]
	public void ReadNumberInt4()
	{
		TestToken("0", typeof(int), 0);
	}

	[TestMethod]
	public void ReadNumberDouble1()
	{
		TestToken("34.5", typeof(double), 34.5);
	}

	[TestMethod]
	public void ReadNumberDouble2()
	{
		TestToken("-3.62", typeof(double), -3.62);
	}

	[TestMethod]
	public void ReadNumberDouble3()
	{
		TestToken("+123.6", typeof(double), +123.6);
	}

	[TestMethod]
	public void ReadNumberDouble4()
	{
		TestToken("4.", typeof(double), 4.0);
	}

	[TestMethod]
	public void ReadNumberDouble5()
	{
		TestToken("-.002", typeof(double), -.002);
	}

	[TestMethod]
	public void ReadNumberDouble6()
	{
		TestToken("0.0", typeof(double), 0.0);
	}

	[TestMethod]
	public void ReadNumberDouble7()
	{
		TestToken(".023", typeof(double), .023);
	}

	[TestMethod]
	public void ReadNumberIntError()
	{
		try
		{
			TestToken("++01", typeof(int), 01);
			Assert.Fail();
		}
		catch (Exception ex)
		{
			Assert.AreEqual(ex.Message, "Ошибка в целом числе");
		}
	}

	[TestMethod]
	public void ReadNumberDoubleError()
	{
		try
		{
			TestToken("..2", typeof(double), .2);
			Assert.Fail();
		}
		catch (Exception ex)
		{
			Assert.AreEqual(ex.Message, "Ошибка в вещественном числе");
		}
	}

	[TestMethod]
	public void ReadHexTest1()
	{
	    TestToken("\n%rewrew\n<901FA3>", typeof(byte[]), new byte[] {0x90, 0x1F, 0xA3});
	}

	[TestMethod]
	public void ReadHexTest2()
	{
	    byte[] bytemas = new byte[] { 0x90, 0x1f, 0xa0 };
	    TestToken("<901FA>", typeof(byte[]), bytemas);
	}

	[TestMethod]
	public void ReadNameObjectTest1()
    {
		TestToken("/Name1", typeof(string), "Name1");
    }
	
	[TestMethod]
	public void ReadNameObjectTest2()
	{
		TestToken("/ASomewhatLongerName", typeof(string), "ASomewhatLongerName");
	}

	[TestMethod]
	public void ReadNameObjectTest3()
	{
		TestToken("/A;Name_With-VariousCharacters?", typeof(string), "A;Name_With-VariousCharacters?");
	}

	[TestMethod]
	public void ReadNameObjectTest4()
	{
		TestToken("/1.2", typeof(string), "1.2");
	}

	[TestMethod]
	public void ReadNameObjectTest5()
	{
		TestToken("/$$", typeof(string), "$$");
	}

	[TestMethod]
	public void ReadNameObjectTest6()
	{
		TestToken("/@pattern", typeof(string), "@pattern");
	}

	[TestMethod]
	public void ReadNameObjectTest7()
	{
		TestToken("/.notdef", typeof(string), ".notdef");
	}

	[TestMethod]
	public void ReadNameObjectTest8()
	{
		TestToken("/Lime#20Green", typeof(string), "Lime Green");
	}

	[TestMethod]
	public void ReadNameObjectTest9()
	{
		TestToken("/paired#28#29parentheses", typeof(string), "paired()parentheses");
	}

	[TestMethod]
	public void ReadNameObjectTest10()
	{
		TestToken("/The_Key_of_F#23_Minor", typeof(string), "The_Key_of_F#_Minor");
	}

	[TestMethod]
	public void ReadNameObjectTest11()
	{
	    TestToken("/A#42", typeof(string), "AB");
	}

	[TestMethod]
	public void ReadNameObjectNullErrorTest()
	{
	    try
	    {
		TestToken("/\x00", typeof(string), "");
		Assert.Fail();
	    }
	    catch(Exception ex)
            {
		Assert.AreEqual(ex.Message, "В имени содержится неверный символ '0'");
	    }
	}
    }
}
