using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfCS
{
    class PdfGraphics
    {
        /// <summary>
        ///  Стек операндов команд
        /// </summary>
        static Stack<object> operands = new Stack<object>();

        /// <summary>
        /// функция оператора графики
        /// </summary>
        delegate void Operator();

        /// <summary>
        /// Элемент таблицы команд
        /// </summary>
        Dictionary<string, Operator> commands = new Dictionary<string, Operator>
        {
            {"q", new Operator(PushState)},
            {"Q", new Operator(PopState)},
            {"Tf", new Operator(SelectFont)},
        };

        /// <summary>
        /// Отрисовка страницы
        /// </summary>
        /// <param name="content">поток содержимого страницы (команды графики)</param>
        /// <param name="resources">словарь ресурсов страницы</param>
        static void Render(byte[] content, Dictionary<string, object> resources)
        {
        }

        /// <summary>
        /// Сохранение результатов текущего состояния в стек состояний
        /// </summary>
        static void PushState()
        {
        }

        /// <summary>
        /// Установка результатов текущего состояния из стека состояний
        /// </summary>
        static void PopState()
        {
        }

        /// <summary>
        /// Текущий размер шрифта
        /// </summary>
        private static int currentFontSize;

        /// <summary>
        /// команда установки шрифта
        ///<имя шрифта> <размер шрифта> Tf
        ///параметры извлекаются из стека #52
        ///имя шрифта берётся из поля font у словаря ресурсов
        ///значение шрифта - словарь
        ///имя шрифта пропускается, размер шрифта - сохраняется в поле
        ///добавить команду в таблицу команд
        /// </summary>
        static void SelectFont()
        {
            currentFontSize = (int)operands.Pop();
            operands.Pop();
        }
    }
}
