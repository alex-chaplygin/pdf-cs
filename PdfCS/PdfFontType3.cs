using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfCS
{
    /// <summary>
    /// Шрифт Type3
    /// </summary>
    public class PdfFontType3 : PdfFont
    {
        /// <summary>
        /// Прямоугольник опоясывающий максимальный по размерам символ в шрифте
        /// </summary>
        Rectangle fontBBox;
	
        /// <summary>
        /// Матрица преобразования из системы координат фигуры символа в пространство текста
        /// </summary>
        Matrix fontMatrix;
	
        /// <summary>
        /// Словарь ссылок на программы рисования символов шрифта
        /// </summary>
        Dictionary<string, object> charProcs;

	/// <summary>
        /// Cловарь ресурсов для отрисовки символов шрифта
        /// </summary>
        Dictionary<string, object> resources;

        public PdfFontType3(Dictionary<string, object> dic) : base(dic)
        {
            object[] box = (object[])dic["FontBBox"];
            object[] matrix = (object[])dic["FontMatrix"];

            subType = dic["Substype"].ToString();
            fontBBox = new Rectangle(Convert.ToDouble(box[0]), Convert.ToDouble(box[1]), 
                Convert.ToDouble(box[2]), Convert.ToDouble(box[3]));
            fontMatrix = new Matrix(Convert.ToDouble(matrix[0]), Convert.ToDouble(matrix[1]), 
                Convert.ToDouble(matrix[2]), Convert.ToDouble(matrix[3]), 
                Convert.ToDouble(matrix[4]), Convert.ToDouble(matrix[5]));

            if (dic.ContainsKey("CharProcs"))
                charProcs = (Dictionary<string, object>)dic["CharProcs"];
            if (dic.ContainsKey("Resources"))
                resources = (Dictionary<string, object>)dic["Resources"];
        }
    }
}
