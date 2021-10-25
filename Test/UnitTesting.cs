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

	public static void IsEqual(object o, object o2)
	{
	    if (o is int)
	    {
		if ((int)o != (int)o2)
		    throw new Exception("Fail");
	    }
	    else 
		if (o != o2)
		    throw new Exception("Fail");
	}
    }
}
