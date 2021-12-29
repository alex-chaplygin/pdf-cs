using System;
using System.Collections.Generic;


public class PdfFont
{
    /// <summary>
    ///тип шрифта
    ///</summary>
    protected string subType;

    /// <summary>
    ///имя шрифта
    ///</summary>
    protected string baseFont;

    /// <summary>
    ///код первого символа в массиве Widths
    ///</summary>
    protected int firstChar;

    /// <summary>
    /// код последнего символа в массиве Widths
    ///</summary>
    protected int lastChar;

    /// <summary>
    /// массив ширин фигур в шрифте
    ///</summary>
    protected int[] widths;

    /// <summary>
    ///словарь, хранящий метрики шрифта
    ///</summary>
    protected Dictionary<string, object> fontDescriptor;

    /// <summary>
    ///encoding - имя кодировки
    ///</summary>
    protected string encoding;

    /// <summary>
    ///поток соответствия кодов символов кодам Unicode
    ///</summary>
    protected byte[] toUnicode;

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
