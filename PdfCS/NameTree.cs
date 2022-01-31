using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfCS
{
    /// <summary>
    ///тип данных дерево имен, ключи дерева представляют собой строки
    ///все ключи упорядочены, значения - любые типы: ссылки для словарей, массивов и строк
    ///или прямые типы(без ссылок): null, числа, bool и имена
    ///эта структура данных предназначена для эффективного поиска по ключу без необходимости
    ///полного чтения данных.
    ///дерево состоит из узлов(словарь). узлы бывают 3-х типов: корневой, промежуточный и лист.
    ///Корневой узел содержит или Kids или Names(в случае одного узла в дереве).
    ///Промежуточные узлы содержат поля Kids и Limits
    ///Листы содержат только Limits или Names
    ///Kids - массив ссылок на узлы или листья
    ///Names - массив[ключ1 значение1 ключ2 значение2...]
    ///ключ - строка(char[]), значение - объект(object)
    ///ключи отсортированы
    ///Limits - массив из двух строк(char[]) - наименьший и наибольший ключ
    ///Все дерево отсортировано: меньшие ключи в поддеревьях всегда раньше больших, порядок - лексический
    ///сравнение ключей должно происходить символ в символ, ключи не пересекаются в разных узлах
    /// </summary>
    public class NameTree
    {
        /// <summary>
        /// Словарь корня дерева
        /// </summary>
        Dictionary<string, object> root;

        /// <summary>
        /// Создаёт дерево имён
        /// </summary>
        /// <param name="dic">словарь корня дерева</param>
        public NameTree(Dictionary<string, object> dic)
        {
            root = dic;
        }

        /// <summary>
        /// Поиск объекта в дереве
        /// </summary>
        /// <param name="key">ключ</param>
        /// <returns>возвращает значение соответствующее ключу</returns>
        public object Search(char[] key)
        {
	    Dictionary<string, object> d;
            object so = null;
            if (root.ContainsKey("Names"))
            {
                object[] nms = (object[])root["Names"];
                for (int i = 0; i < key.Length; i++)
                {
                    for (int j = 0; j < nms.Length/2; j++)
                    if (key.SequenceEqual(((string)nms[j * 2]).ToCharArray()))
                        so = PDFFile.LoadLink(nms[(j * 2) + 1], out d);
                }
                    return so;
                
            }
            else
            {
                Dictionary<string, object> tmpNode = null;
                object[] tmpKds = (object[])root["Kids"];
                
                foreach (object i in tmpKds) 
                {
                    tmpNode = (Dictionary<string, object>)PDFFile.LoadLink(i, out d);
                    string[] tmpLmt = (string[])tmpNode["Limits"];
                    if (String.Compare((new String(key)), tmpLmt[0]) >= 0 && String.Compare((new String(key)), tmpLmt[1]) <= 0)
                        so = SearchRec(key, tmpNode);
                }
            }
            return so;
        }

        /// <summary>
        /// Выполняет поиск
        /// </summary>
        /// <param name="key">ключ</param>
        /// <param name="node">текущий узел</param>
        /// <returns>возвращает значение соответствующее ключу</returns>
        private object SearchRec(char[] key, Dictionary<string, object> node)
        {
	    Dictionary<string, object> d;
            object so = null;

            if (node.ContainsKey("Names"))
            {
                object[] nms = (object[])node["Names"];
                for (int i = 0; i < key.Length; i++)
                {
                    for (int j = 0; j < nms.Length / 2; j++)
                        if (key.SequenceEqual(((string)nms[j * 2]).ToCharArray()))
                            so = PDFFile.LoadLink(nms[(j * 2) + 1], out d);
                }
                    return so;
                
            }
            else
            {
                Dictionary<string, object> node2 = null;
                
                object[] tmpKds = (object[])node["Kids"];

                foreach (object i in tmpKds)
                {
                    node2 = (Dictionary<string, object>)PDFFile.LoadLink(i, out d);
                    string[] tmpLmt = (string[])node2["Limits"];
                    if (String.Compare((new String(key)), tmpLmt[0]) >= 0 && String.Compare((new String(key)), tmpLmt[1]) <= 0)
                        so = SearchRec(key, node2);
                }
            }
            return so;
        }
    }
}
