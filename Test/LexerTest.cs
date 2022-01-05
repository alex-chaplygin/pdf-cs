﻿using System;
using System.Collections.Generic;
using System.IO;
using PdfCS;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PDFTest
{
    [TestClass]
    public class LexerTest
    {
		private object ReadToken(string s)
		{
			MemoryStream m = new MemoryStream();
			byte[] b = new byte[s.Length];
			for (int i = 0; i < s.Length; i++)
				b[i] = (byte)s[i];
			m.Write(b, 0, s.Length);
			m.Seek(0, SeekOrigin.Begin);
			Lexer l = new Lexer(m);
			l.NextChar();
			return l.ReadToken();
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
		public void Number()
		{
			TestToken("  10", typeof(int), 10);
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
			TestToken("\n%rewrew\n<901FA3>", typeof(byte[]), new byte[] { 0x90, 0x1F, 0xA3 });
		}

		[TestMethod]
		public void ReadHexTest2()
		{
			byte[] bytemas = new byte[] { 0x90, 0x1f, 0xa0 };
			TestToken("<901FA>", typeof(byte[]), bytemas);
		}
			
		[TestMethod]
		public void ReadArrayTest1()
		{
			TestToken("[549", typeof(char), '[');				
		}

		[TestMethod]
		public void ReadArrayTest2()
		{
			TestToken("]549", typeof(char), ']');
		}

		[TestMethod]
		public void ReadDictionaryTest()
		{
			TestToken("<<", typeof(string), "<<");
		}

		[TestMethod]
		public void ReadDictionaryTest2()
		{
			TestToken(">>", typeof(string), ">>");
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
			TestToken("(\\5\\05\\0010\\010)", typeof(char[]), ("\u0005\u0005\u0001" + "0" + "\u0008").ToCharArray());
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

		[TestMethod]
		public void ReadIdTest()
		{
		    TestToken("     obj", typeof(string), "obj");
		    TestToken("     stream", typeof(string), "stream");
		    TestToken("     Tj", typeof(string), "Tj");
		}

		[TestMethod]
		public void ReadNameObjectTest1()
		{
			TestToken("/Name1", typeof(NameObject), new NameObject("Name1"));
		}

		[TestMethod]
		public void ReadNameObjectTest2()
		{
			TestToken("/.notdef", typeof(NameObject), new NameObject(".notdef"));
		}

		[TestMethod]
		public void ReadNameObjectTest3()
		{
			TestToken("/ASomewhatLongerName", typeof(NameObject), new NameObject("ASomewhatLongerName"));
		}

	[TestMethod]
		public void ReadNameObjectTest4()
		{
			TestToken("/A;Name_With-VariousCharacters?", typeof(NameObject), new NameObject("A;Name_With-VariousCharacters?"));
		}

	[TestMethod]
		public void ReadNameObjectTest5()
		{
			TestToken("/1.2", typeof(NameObject), new NameObject("1.2"));
		}

	[TestMethod]
		public void ReadNameObjectTest6()
		{
			TestToken("/$$", typeof(NameObject), new NameObject("$$"));
		}

	[TestMethod]
		public void ReadNameObjectTest7()
		{
			TestToken("/@pattern", typeof(NameObject), new NameObject("@pattern"));
		}

	[TestMethod]
		public void ReadNameObjectTest8()
		{
			TestToken("/lime#20Green", typeof(NameObject), new NameObject("lime Green"));
		}

	[TestMethod]
		public void ReadNameObjectTest9()
		{
			TestToken("/paired#28#29parentheses", typeof(NameObject), new NameObject("paired()parentheses"));
		}

	[TestMethod]
		public void ReadNameObjectTest10()
		{
			TestToken("/The_Key_of_F#23_Minor", typeof(NameObject), new NameObject("The_Key_of_F#_Minor"));
		}

	[TestMethod]
		public void ReadNameObjectTest11()
		{
			TestToken("/A#42", typeof(NameObject), new NameObject("AB"));
		}
	}
}
