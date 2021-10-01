﻿using System;
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
                        throw new Exception("Ошибка true Boolean");
                return true;
            }
            else if (lastChar == 'f')
            {
                string _false = "false";
                for (int i = 1; i < 6; i++)
                    if (lastChar == _false[i - 1])
			NextChar();
                    else
                        throw new Exception("Ошибка false Boolean");
                return false;
            }
            else
		throw new Exception("Неверный Boolean");
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

	public object ReadNumber()
        {
            string str = "";
            int resi;
            double resd;
            
            while (lastChar != '\uffff' && 
                (lastChar == '+' || lastChar == '-' || lastChar == '.' || Char.IsDigit(lastChar)))
            {
                str += lastChar;
                NextChar();
            }
            str = str.Replace('.', ',');
            if (str.IndexOf(",") < 0)
                if (int.TryParse(str, out resi))
                    return resi;
                else
                    throw new Exception("Ошибка в числе: " + str);
            else if (double.TryParse(str, out resd))
                return resd;
            else
                throw new Exception("Ошибка в числе: " + str);
        }

	public string ReadNameObject()
        {
            string res = NextChar().ToString();
            while (lastChar != '\uffff')
            {
                if (lastChar == '\x00')
                    throw new Exception("В имени содержится неверный символ '0'");
                else if (Parser.IsWhitespace(lastChar) || Parser.IsDelimeter(lastChar))
                    break;
                res += NextChar();
            }
            return res.Substring(0, res.Length - 1); ;
        }

	public byte[] ReadHex()
	{
	    List<byte> bytes = new List<byte>();
	    string s = "";

	    while (lastChar != '\uffff')
	    {
		if (NextChar() == '>')
		    break;
		else if (Char.IsDigit(lastChar) || lastChar >= 'a' && lastChar <= 'f' ||
		    lastChar >= 'A' && lastChar <= 'F')
		    s += lastChar;
		else
		    throw new Exception("Неверный символ в ReadHex: " + lastChar);
		if (s.Length == 2)
		{
		    bytes.Add(Convert.ToByte(s, 16));
		    s = "";
		}
	    }
	    if (lastChar == '\uffff')
		throw new Exception("Нет скобки в ReadHex");
	    if (s != "")
		bytes.Add(Convert.ToByte(s + "0", 16));
	    return bytes.ToArray();
	}
    }
}
