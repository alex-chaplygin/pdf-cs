using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PdfCS
{
    public class Parser
    {
        private Stream stream;
        private char lastChar;

        public Parser(Stream s)
        {
            stream = s;
        }

        public char NextChar()
        {
            lastChar = (char)stream.ReadByte();
            return lastChar;
        }

        public static bool IsWhitespace(char c)
        {
            string w = "\x00\t\r\n \x0c";

            return w.IndexOf(c) != -1;
        }

        public static bool IsDelimeter(char c)
        {
            string d = "()<>{}[]/%";

            return d.IndexOf(c) != -1;
        }

        public void SkipComment()
        {
            while (NextChar() != '\n') ;
        }

        public void SkipWhitespace()
        {
            while (lastChar == '%' || Parser.IsWhitespace(lastChar))
	    {
                if (lastChar == '%')
		    SkipComment();
                NextChar();
            }
        }

	public object ReadNull()
        {
            string f = "";

            for (int i = 0; i < 3; i++)
                f += NextChar();
            if (f != "ull")
                throw new  Exception("Ошибка в null");
            return null;
        }

	public bool ReadBoolean()
        {
	    // Случай true
            if (lastChar == 't')
            {
                string _true = "true";
                for (int i = 1; i < 5; i++)
                    if (lastChar == _true[i - 1])
                        NextChar();
                    else
                        throw new Exception("Ошибка Boolean");
                return true;
            }
            else if (lastChar == 'f')
            {
                string _false = "false";
                for (int i = 1; i < 6; i++)
                    if (lastChar == _false[i - 1])
			NextChar();
                    else
                        throw new Exception("Ошибка Boolean");
                return false;
            }
            else
		throw new Exception("Ошибка Boolean");
        }

	public void SkipEndOfLine()
        {
            if (lastChar == '\n')
                NextChar();
            else if (lastChar == '\r')
            {
                if (NextChar() != '\n')
                    throw new Exception("Ошибка при переводе строки");
                NextChar();
            }
        }

	public string ReadKeyword()
	{
	    string res = lastChar.ToString();
	    while (lastChar != '\uffff' && !Parser.IsWhitespace(lastChar) &&
		   !Parser.IsDelimeter(lastChar))
		res += NextChar();
	    return res.Substring(0, res.Length - 1);
	}
    }
}
