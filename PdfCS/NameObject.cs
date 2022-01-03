using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfCS
{
    /// <summary>
    ///   Объект - имя
    /// </summary>
    public class NameObject
    {
        private string name;
	
        public NameObject(string name)
	{
                this.name = name;
	}
	
        public override bool Equals(object obj)
        {
	    NameObject o = (NameObject)obj;
            return name == o.name;
        }
    }
}
