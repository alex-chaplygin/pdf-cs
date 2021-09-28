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
	    Print(ASCII85.Decode(File.ReadAllBytes(args[0])));
        }

	static void Print(byte[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
                Console.Write((char)arr[i]);
	    Console.WriteLine();
        }
    }
}
