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
            PDFFile.Open(File.OpenRead(args[0]));
            Dictionary<string, object> dict;
            for (int i = 0; i < PDFFile.xrefTable.Length; i++)
            {
                if (PDFFile.xrefTable[i].offset == 0)
                    continue;
                var cur = PDFFile.GetObject(i, out dict);
                Console.WriteLine($"Объект {i}: {cur.ToString()}");
            }
        }
    }
}
