using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PdfCS
{
    class Parser
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

        public bool isWhiteSpace(char c)
        {
            string whiteSpaces = "\x00\t\r\n \x0c";

            return whiteSpaces.IndexOf(c) != -1;
        }

        public bool isDelimeter(char c)
        {
            string delimeters = "()<>{}[]/%";

            return delimeters.IndexOf(c) != -1;
        }

        public void SkipComment()
        {
            while (NextChar() != '\n') ;
        }

        public void SkipWhitespace()
        {
            while (lastChar == '%' || isWhiteSpace(lastChar))
            {
                if (lastChar == '%') SkipComment();
                NextChar();
            }
        }

	public object ReadNull()
        {
            string f = "";
            for (int i = 0; i < 3; i++)
                f += NextChar();
            
            if ( f!= "ull")
                throw new  Exception("Ошибка в null");
            return null;
        }
    }
}
