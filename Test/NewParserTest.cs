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
	private object ReadToken(string s)
	{
	    MemoryStream m = new MemoryStream();
	    byte[] b = new byte[s.Length];
	    for (int i = 0; i < s.Length; i++)
		b[i] = (byte)s[i];
	    m.Write(b, 0, s.Length);
	    m.Seek(0, SeekOrigin.Begin);
	    Parser p = new Parser(m);
	    p.NextChar();
	    return p.ReadToken();
	}
	
	public void TestToken(string s, Type t, object o)
	{
	    if (o != null && o.GetType() != t)
		Assert.Fail();
	    object o2 = ReadToken(s);
	    //Console.WriteLine($"\nТребуется = {Convert.ChangeType(o, t)} Получено = {Convert.ChangeType(o2, t)}");
	    if (t == typeof(byte[]) || t == typeof(char[]))
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
	public void TestStreamObject()
	{
	    ReadIndirectObjectTest("11 0 obj <</Length 5>> stream\n23145\nendstream endobj", 
				   new byte[] {(byte)'2', (byte)'3', (byte)'1', (byte)'4', (byte)'5' });
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
	    if (obj is byte[])
		CollectionAssert.AreEqual((byte[])o2, (byte[])obj);
	    else
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

	[TestMethod]
	public void ReadArrayTest()
	{
	    object[] m = (object[])ReadToken("[549 3.14 false(Ralph) /SomeName [1]]");
	    Assert.AreEqual((int)m[0], 549);
	    Assert.AreEqual((double)m[1], (double)3.14);
	    Assert.AreEqual((bool)m[2], false);
	    CollectionAssert.AreEqual((char[])m[3], new char[] {'R', 'a', 'l', 'p', 'h'});
	    Assert.AreEqual((NameObject)m[4], new NameObject("SomeName"));
	    Assert.AreEqual((int)((object[])m[5])[0], 1);
	}

	[TestMethod]
	public void ReadArrayErrorTest()
	{
	    try
	    {
		object[] m = (object[])ReadToken("[549");
		Assert.AreEqual((int)m[0], 549);
		Assert.Fail();
	    }
	    catch(Exception ex)
            {
		Assert.AreEqual(ex.Message, "ReadArray завершился без закрывающей скобки");
	    }
	}

	[TestMethod]
	public void LinkTest()
	{
	    Tuple<int, int> t = (Tuple<int, int>)ReadToken("1 0 R");
	    Assert.AreEqual(t.Item1, 1);
	    Assert.AreEqual(t.Item2, 0);
	}	

	[TestMethod]
	public void LinkTest2()
	{
	    int t = (int)ReadToken("1 0 1 R");
	    Assert.AreEqual(t, 1);
	}	

	[TestMethod]
	public void LinkTest3()
	{
	    int t = (int)ReadToken("0 6 0003 65535 f 00017 000");
	    Assert.AreEqual(t, 0);
	}	

		[TestMethod]
		public void ReadDictionaryTest()
		{
			Dictionary<string, object> dict = (Dictionary<string, object>)ReadToken("<<" +
			"/Type /Example" +
			"/Subtype /DictionaryExample" +
			"/Version 0.01" +
			"/IntegerItem 12" +
			"/StringItem (a string)" +
			"/Subdictionary" +
			"<<" +
			"/Item1 0.4" +
			"/Item2 true" +
			"/LastItem (not!)" +
			"/VeryLastItem (OK)" + ">>" + ">>");
			Assert.AreEqual((NameObject)dict["Type"], new NameObject("Example"));
			Assert.AreEqual((NameObject)dict["Subtype"], new NameObject("DictionaryExample"));
			Assert.AreEqual((double)dict["Version"], 0.01);
			Assert.AreEqual((int)dict["IntegerItem"], 12);
			CollectionAssert.AreEqual((char[])dict["StringItem"], "a string".ToCharArray());
			Assert.AreEqual((double)((Dictionary<string, object>)dict["Subdictionary"])["Item1"], 0.4);
			Assert.AreEqual((bool)((Dictionary<string, object>)dict["Subdictionary"])["Item2"], true);
			CollectionAssert.AreEqual((char[])((Dictionary<string, object>)dict["Subdictionary"])["LastItem"], "not!".ToCharArray());
			CollectionAssert.AreEqual((char[])((Dictionary<string, object>)dict["Subdictionary"])["VeryLastItem"], "OK".ToCharArray());
		}
	
        [TestMethod]
        public void ReadDictionaryErrorTest()
        {
            try
            {
                Dictionary<string, object> dict = (Dictionary<string, object>)ReadToken("<</Type /Example");
                Assert.AreEqual((string)dict["Type"], "Example");
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.Message, "ReadDictionary завершился без закрывающих скобок");
            }
        }
	
	[TestMethod]
        public void ReadStringTest1()
        {
            TestToken("(string)", typeof(char[]), "string".ToCharArray());
        }

        [TestMethod]
        public void ReadStringTest2()
        {
            TestToken("(string\\n)", typeof(char[]), "string\n".ToCharArray());
        }

        [TestMethod]
        public void ReadStringTest3()
        {
            TestToken("(Strings may contain balanced parentheses ( ) and special characters ( * ! & } ^ % and so on ).)", typeof(char[]),
                "Strings may contain balanced parentheses ( ) and special characters ( * ! & } ^ % and so on ).".ToCharArray());
        }

        [TestMethod]
        public void ReadStringTest4()
        {
            TestToken("()", typeof(char[]), "".ToCharArray());
        }

        [TestMethod]
        public void ReadStringTest5()
        {
            TestToken("(This string contains \\005two octal characters\\0109.)", typeof(char[]), ("This string contains \u0005two octal characters\u0008" + "9.").ToCharArray());
        }

        [TestMethod]
        public void ReadStringTest6()
        {
            TestToken("(\\5\\05\\0010\\010)", typeof(char[]), ("\u0005\u0005\u0001"+"0"+"\u0008").ToCharArray());
        }

        [TestMethod]
        public void ReadStringTest7()
        {
            TestToken("(aaa\\" + "\nbbb)", typeof(char[]), ("aaabbb").ToCharArray());
        }

        [TestMethod]
        public void ReadStringTest8()
        {
            TestToken("(aaa\r\nbbb)", typeof(char[]), ("aaa\nbbb").ToCharArray());
        }
    }
}
