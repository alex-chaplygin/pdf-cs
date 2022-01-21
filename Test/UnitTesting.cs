using System;
using PdfCS;

namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    public class TestClass : Attribute
    {
    }

    public class TestMethod : Attribute
    {
    }

    public class Assert
    {
	public static void Fail()
	{
	    throw new Exception("Fail");
	}

	public static void IsTrue(bool c)
	{
	    if (!c)
		Fail();
	}
	
	public static void AreEqual(object o, object o2)
	{
	    if (o is int)
	    {
		if ((int)o != (int)o2)
		{
		    Console.WriteLine($"\n {(int)o} != {(int)o2}");
		    Fail();
		}
	    }
	    else if (o is uint)
	    {
		if ((uint)o != (uint)o2)
		{
		    Console.WriteLine($"\n {(uint)o} != {(uint)o2}");
		    Fail();
		}
	    }
	    else if (o is bool)
	    {
		if ((bool)o != (bool)o2)
		    Fail();
	    }
	    else if (o is char)
	    {
		if ((char)o != (char)o2)
		    Fail();
	    }
	    else if (o is byte)
	    {
		if ((byte)o != (byte)o2)
		    Fail();
	    }
	    else if (o is string)
	    {
		if ((string)o != (string)o2)
		    Fail();
	    }
	    else if (o is double)
	    {
		if ((double)o != (double)o2)
		    Fail();
	    }
	    else if (o is NameObject)
	    {
		if (!o.Equals(o2))
		{
		    Console.WriteLine($"NameObject {o.ToString()} {o2.ToString()}");
		    Fail();
		}
	    }
	    else 
		if (o != o2)
		    Fail();
	}
    }

    public class CollectionAssert
    {
	public static void Fail(string msg)
	{
	    throw new Exception("Fail " + msg);
	}

	public static void AreEqual(object o, object o2)
	{
	    if (o is byte[])
	    {
		byte[] oa = (byte[])o;
		byte[] oa2 = (byte[])o2;
		if (oa.Length != oa2.Length)
		    Fail("length");
		for (int i = 0; i < oa.Length; i++)
		    if (oa[i] != oa2[i])
			Fail("i = " + i + " a1 = " + oa[i] + " a2 = " + oa2[i]);
	    }
	    else if (o is char[])
	    {
		char[] oa = (char[])o;
		char[] oa2 = (char[])o2;
		if (oa.Length != oa2.Length)
		    Fail("length");
		for (int i = 0; i < oa.Length; i++)
		    if (oa[i] != oa2[i])
			Fail("i = " + i + " a1 = " + oa[i] + " a2 = " + oa2[i]);
	    }
	}
    }
}
