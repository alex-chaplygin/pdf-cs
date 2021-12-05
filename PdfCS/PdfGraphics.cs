using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.ComponentModel.Design;
using System.Drawing.Drawing2D;
using System.Windows.Input;

namespace PdfCS
{
    /// <summary>
    /// Класс вывода содержимого страницы
    /// </summary>
    public class PdfGraphics
    {
        /// <summary>
        /// Объект из формы, в который выводится графика
        /// </summary>
        private static Graphics graphics;

        /// <summary>
        /// Cостояние графики
        /// </summary>
        private struct State
        {
            /// <summary>
            /// Текущяя матрица трансформации
            /// </summary>
            public Matrix CTM;

            /// <summary>
            /// Текстовая матрица
            /// </summary>
            public Matrix textMatrix;

            /// <summary>
            /// Если true - текстовый объект создается
            /// </summary>
            public bool beginText;

            /// <summary>
            /// Текущий размер шрифта
            /// </summary>
            public int currentFontSize;
        }

        /// <summary>
        /// Прямоуголная область
        /// </summary>
        public struct Rectangle
        {
            public double llx;
            public double lly;
            public double urx;
            public double ury;
        }

        /// <summary>
        /// Текущее состояние графики
        /// </summary>
        private static State currentState;

        /// <summary>
        /// Прямоуголник страницы
        /// </summary>
        private static Rectangle mediaBox;

        /// <summary>
        /// функция оператора графики
        /// </summary>
        private delegate void Operator();

        /// <summary>
        /// таблица команд
        /// </summary>
        private static Dictionary<string, Operator> commands = new Dictionary<string, Operator>
        {
            {"BT", new Operator(BeginText)},
            {"ET", new Operator(EndText)},
            {"cm", new Operator(SetMatrix)},
            {"q", new Operator(PushState)},
            {"Q", new Operator(PopState)},
            {"Tf", new Operator(SelectFont)},
        };

        /// <summary>
        /// стек операндов команд
        /// </summary>
        private static Stack<object> operands;

        /// <summary>
        /// Инициализация графики
        /// </summary>
        /// <param name="g">Объект из формы, в который выводится графика</param>
        /// <param name="r">Система координат страницы</param>
        public static void Init(Graphics g, Rectangle r)
        {
            currentState.beginText = false;
            graphics = g;
            mediaBox = r;
        }

        /// <summary>
        /// Устанавливает матрицу CTM
        /// </summary>
        /// <param name="width">Ширина элемента вывода</param>
        /// <param name="height">Высота элемента вывода</param>
        public static void SetSize(int width, int height)
        {
            currentState.CTM = new Matrix(width / mediaBox.urx, 0, 0, -height / mediaBox.ury, 0, height);
        }

        /// <summary>
        /// Создание нового текстового объекта
        /// Если объект уже создан - возвращаем исключение
        /// В текущем состоянии создаём единичную текстовую матрицу
        /// </summary>
        private static void BeginText()
        {
            if (currentState.beginText)
                throw new Exception("Текстовый объект уже создан");
            currentState.textMatrix = new Matrix();
            currentState.beginText = true;
        }

        /// <summary>
        /// Завершение текстового объекта
        /// В текущем состоянии завершаем текстовый объект
        /// </summary>
        private static void EndText()
        {
            currentState.beginText = false;
	    }
	
        /// <summary>
        /// Модифицирует текущую матрицу трансформаций в текущем состоянии #51
        /// 
        /// новая матрица #50 создается из 6 операндов a, b, c, d, e, f
        /// они извлекаются из стека #52 (операнды в стеке операндов идут в обратном порядке!)
        /// новая матрица трансформации: CTM = M * CTM #50
        /// 
        /// добавь свой метод в таблицу операторов #52, имя метода - "cm"
        /// </summary>
        public static void SetMatrix()
        {
            var f = (double)operands.Pop();
            var e = (double)operands.Pop();
            var d = (double)operands.Pop();
            var c = (double)operands.Pop();
            var b = (double)operands.Pop();
            var a = (double)operands.Pop();

            Matrix matrix = new Matrix(a, b, c, d, e, f);

            currentState.CTM = matrix.Mult(currentState.CTM);
        }

        /// <summary>
        /// сохраняем текущее состояние #51 в стеке состояний
        /// </summary>
        static void PushState()
        {

        }

        /// <summary>
        /// устанавливаем текущее состояние #51 , извлекая из стека состояний
        /// </summary>
        static void PopState()
        {


        }

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
            currentState.currentFontSize = (int)operands.Pop();
            operands.Pop();
        }

        /// <summary>
        /// Отрисовка страницы
        /// </summary>
        /// <param name="content">поток содержимого страницы (команды графики)</param>
        /// <param name="resources">словарь ресурсов страницы</param>
        static void Render(byte[] content, Dictionary<string, object> resources)
        {
        }
        /// <summary>
        ///перемещает позицию текста

        /// tx ty Td
        /// операнды tx ty берутся из стека операндов #52
        /// создается матрица перемещения Mt #50
        /// из текущего состояния #51 #55 берется текстовая матрица MT и преобразуется
        /// MT = Mt * MT
        /// </summary>
        private static void TextMove()
        {
            var ty = (double)operands.Pop();
            var tx = (double)operands.Pop();
            // Матрица перемещения
            Matrix Mt = new Matrix(1, 0, 0, 1, tx, ty);
            currentState.textMatrix = Mt.Mult(currentState.textMatrix);
        }
    }
}
