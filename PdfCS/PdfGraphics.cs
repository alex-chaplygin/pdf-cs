using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

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
        /// стек операндов команд
        /// </summary>
        private Stack<object> operands;

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
        };

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
    }
}
