using System;

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

	public static void AreEqual(object o, object o2)
	{
	    if (o is int)
	    {
		if ((int)o != (int)o2)
		    Fail();
	    }
	    else if (o is bool)
	    {
		if ((bool)o != (bool)o2)
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
	    else 
		if (o != o2)
		    Fail();
	}
    }
}
