using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PdfCS
{
    class ASCIIHex
    {
        public static byte[] Decode(byte[] stream)
        {
            string s = "";
            List<byte> bt = new List<byte>();
            for (int i = 0; i < stream.Length; i++)
            {
                char c = (char)stream[i];
                if (c == '>')
                    break;
                else if (Parser.IsWhitespace(c))
                    continue;
                else if (Char.IsDigit(c) || c >= 'a' && c <= 'f' || c >= 'A' && c <= 'F')
                    s += c;
                else
                    throw new Exception($"Неверный символ ASCIIHex: {c}");
                if (s.Length == 2)
                {
                    bt.Add(Convert.ToByte(s, 16));
                    s = "";
                }
            }
            if (s != "")
                bt.Add(Convert.ToByte(s, 16));
            return bt.ToArray();
        }
    }
}
