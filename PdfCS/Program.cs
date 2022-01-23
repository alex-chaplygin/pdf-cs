using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PdfCS
{
    class Program
    {
        static void Main(string[] args)
        {
	    Dictionary<string, object> dict;
	    PDFFile.Open(File.OpenRead(args[0]));
	    for (int i = 0; i < PDFFile.xrefTable.Length; i++)
            {
                if (PDFFile.xrefTable[i].offset == 0)
                    continue;
                var cur = PDFFile.GetObject(i, out dict);
		Console.Write($"Объект {i}");
		if (cur is byte[])
		    DumpStream((byte[])cur);
		else
		    Console.WriteLine($" : {cur.ToString()}");
            }
	    /*
	    try
	    {
		PDFFile.Open(File.OpenRead(args[0]));
		for (int i = 1; i < 4; i++)
		{
                    Dictionary<string, object> dictionary = PDFFile.GetPage(i);
                    foreach (var pair in dictionary)
                        Console.WriteLine($"{pair.Key} {pair.Value}");
                }
	    }
	    catch (Exception e)
	    {
		Console.WriteLine(e.Message);
	    }*/
        }

	static void DumpStream(byte[] s)
	{
	    Console.WriteLine();
	    for (int i = 0; i < s.Length; i++)
		Console.Write((char)s[i]);
	    Console.WriteLine();
	}
	
	static void Print(byte[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
                Console.Write((char)arr[i]);
	    Console.WriteLine();
        }
    }
}
