using System;
using System.Collections.Generic;


public class PdfFont
{
    /// <summary>
    ///тип шрифта
    ///</summary>
    string subType;

    /// <summary>
    ///имя шрифта
    ///</summary>
    string baseFont;

    /// <summary>
    ///код первого символа в массиве Widths
    ///</summary>
    int firstChar;

    /// <summary>
    /// код последнего символа в массиве Widths
    ///</summary>
    int lastChar;

    /// <summary>
    /// массив ширин фигур в шрифте
    ///</summary>
    int[] widths;

    /// <summary>
    ///словарь, хранящий метрики шрифта
    ///</summary>
    Dictionary<string, object> fontDescriptor;

    /// <summary>
    ///encoding - имя кодировки
    ///</summary>
    string encoding;

    /// <summary>
    ///поток соответствия кодов символов кодам Unicode
    ///</summary>
    ///
    byte[] toUnicode;

    /// <summary>
    ///   считываем данные шрифта из словаря 
    /// </summary>
    /// <param name="dic">словарь шрифта</param>
    public PdfFont(Dictionary<string, object> dic)
    {
        if (dic["Type"].ToString() != "Font")
            throw new Exception("тип != font");

        subType = dic["SubType"].ToString();
        baseFont = dic["BaseFont"].ToString();
        firstChar = (int)dic["FirstChar"];
        lastChar = (int)dic["LastChar"];
        widths = (int[])dic["Widths"];
        fontDescriptor = (Dictionary<string, object>)dic["FontDescriptor"];
        encoding = dic["Encoding"].ToString();
        toUnicode = (byte[])dic["ToUnicode"];
    }
}
