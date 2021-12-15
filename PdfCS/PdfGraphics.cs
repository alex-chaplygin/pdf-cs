using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.ComponentModel.Design;
using System.Drawing.Drawing2D;
using System.Windows.Input;
using System.IO;

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
            public int textFontSize;

	        /// <summary>
            /// имя шрифта
            /// </summary>
            public string textFont;

            /// <summary>
            /// расстояние в немасштабированных единицах пользователя
            /// 
            /// насколько текст поднимается относительно строки
            /// </summary>
            public double textRise;

            /// <summary>
            /// горизонтальное масштабирование
            /// </summary>
            public double horizontalScale;

            /// <summary>
            /// Текущая ширина линии в пользовательских единицах
            /// для перевода в экранную толщину нужно использовать коэффициент a матрицы CTM
            /// ширина 0 соответствует 1 pixel на экране
            /// </summary>
            public double lineWidth;

            /// <summary>
            /// Расстояние между строк
            /// </summary>
            public double leading;

            /// <summary>
            /// Текущая координата по x
            /// </summary>
            public double textX;

            /// <summary>
            /// Текущая координата по y
            /// </summary>
            public double textY;
        }

        /// <summary>
        /// Прямоуголная область
        /// </summary>
        public struct Rectangle
        {
	        /// <summary>
	        ///   левый нижний угол
	        /// </summary>
            public double llx;
            public double lly;

	        /// <summary>
	        ///   правый верхний угол
	        /// </summary>
            public double urx;
            public double ury;

	    public Rectangle(double llx, double lly, double urx, double ury)
            {
                this.llx = llx;
                this.lly = lly;
                this.urx = urx;
                this.ury = ury;
            }
        }

        /// <summary>
        /// Текущее состояние графики
        /// </summary>
        private static State currentState;

        /// <summary>
        /// Стэк состояний
        /// </summary>
        private static Stack<State> states;

        /// <summary>
        /// Прямоуголник страницы
        /// </summary>
        private static Rectangle mediaBox;

        /// <summary>
        /// Текущий путь
        /// </summary>
        private static GraphicsPath currentPath;

        /// <summary>
        /// Первая точка пути
        /// </summary>
        private static PointF pathFirstPoint;

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
            {"Tm", new Operator(SetTextMatrix)},
            {"q",  new Operator(PushState)},
            {"Q",  new Operator(PopState)},
            {"Tf", new Operator(SelectFont)},
	    {"Tj", new Operator(ShowText)},
            {"m", new Operator(BeginPath)},
            {"l", new Operator(AddLine)},
	    {"Td", new Operator(TextMove)},
	    {"TL", new Operator(SetLeading)},
            {"TD", new Operator(TextMoveLeading)},
            {"w", new Operator(SetLineWidth)},
            {"re", new Operator(AddRectangle)},
            {"T*", new Operator(NextLine)},
            {"TJ", new Operator(ShowStrings)},
            {"S", new Operator(StrokePath)},
            {"'",  new Operator(MoveAndShowText)}
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
	        operands = new Stack<object>();
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
	        currentState.textFont = "Arial";
            currentState.textRise = 0;
            currentState.horizontalScale = 1.0;
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
        /// Устанавливает текстовую матрицу
        /// Параметры извлекаются из стека операндов внутри метода.
        /// </summary>
        private static void SetTextMatrix()
        {
            var f = (double)operands.Pop();
            var e = (double)operands.Pop();
            var d = (double)operands.Pop();
            var c = (double)operands.Pop();
            var b = (double)operands.Pop();
            var a = (double)operands.Pop();

            currentState.textMatrix = new Matrix(a, b, c, d, e, f);
        }

        /// <summary>
        /// сохраняем текущее состояние #51 в стеке состояний
        /// </summary>
        static void PushState()
        {
            states.Push(currentState);
        }

        /// <summary>
        /// устанавливаем текущее состояние #51 , извлекая из стека состояний
        /// </summary>
        static void PopState()
        {
            if (states.Count > 0)
                currentState = states.Pop();
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
        private static void SelectFont()
        {
            currentState.textFontSize = (int)operands.Pop();
            operands.Pop();
        }

        /// <summary>
        /// Отрисовка страницы
        /// </summary>
        /// <param name="content">поток содержимого страницы (команды графики)</param>
        /// <param name="resources">словарь ресурсов страницы</param>
        public static void Render(byte[] content, Dictionary<string, object> resources)
        {
	    Parser parser = new Parser(new MemoryStream(content));
            parser.NextChar();
            object temp;
            while (true)
            {
                temp = parser.ReadToken();
                if (temp is char && (char)temp == '\uffff')
                    return;
                if (temp is string && commands.ContainsKey((string)temp))
                    commands[(string)temp]();
                else
                    operands.Push(temp);
            }
        }
	
        /// <summary>
        /// перемещает позицию текста
	    ///
        /// tx ty Td
        /// операнды tx ty берутся из стека операндов #52
        /// создается матрица перемещения Mt #50
        /// из текущего состояния #51 #55 берется текстовая матрица MT и преобразуется
        /// MT = Mt * MT
        /// </summary>
        private static void TextMove()
        {
            var ty = (int)operands.Pop();
            var tx = (int)operands.Pop();
            // Матрица перемещения
            Matrix Mt = new Matrix(1, 0, 0, 1, tx, ty);
            currentState.textMatrix = Mt.Mult(currentState.textMatrix);
        }

	/// <summary>
        /// установка расстояния между строками
        /// leading TL
        /// </summary>
        static void SetLeading()
        {
            currentState.leading = (double)operands.Pop();
        }

        /// <summary>
        /// установить leading как -ty
        ///перемещение позиции текста tx ty
        ///tx ty TD
        /// </summary>
        static void TextMoveLeading()
        {
            currentState.leading = -(int)operands.Peek();
            TextMove();
        }
	
	/// <summary>
        /// выводит строку текста
        /// 
        /// операнд - строка (тип char[])
        /// вывод осуществляется в координаты x = 0, y = 0 пространства текста
        /// формула преобразований:
        /// Tm - текстовая матрица
        /// CTM - матрица трансформации
        /// Вычисляется матрица T(textFontSize* horizontalScale, 0, 0, textFontSize, 0, textRise)
        /// Матрица вывода:
        /// Tr = T* Tm * CTM
        /// координаты вывода в окно вычисляются как умножение вектора x, y на матрицу Tr
        /// выводим текст через объект Graphics
        /// устанавливаем шрифт с именем textFont и размером textFontSize умноженным на коэффициент d матрицы CTM
        /// </summary>
        public static void ShowText()
        {
            char[] array = (char[])operands.Pop();

            Matrix T = new Matrix();// currentState.textFontSize * currentState.horizontalScale, 0, 0, currentState.textFontSize, 0, currentState.textRise);

            Matrix Tr = T.Mult(currentState.textMatrix);
            Tr = Tr.Mult(currentState.CTM);

            double x, y;
            Tr.MultVector(0, 0, out x, out y);

            graphics.DrawString(String.Concat<char>(array), new Font(currentState.textFont,
                (int)Math.Abs(currentState.textFontSize * currentState.CTM.GetValues()[3])),
                new SolidBrush(Color.Black), (int)x, (int)y);
        }

        /// <summary>
	/// (строка) '
        /// Параметр - string. Берется из стека операндов.
        /// Перемещается на следующую строчку #60
        /// Выводит строку #57
        /// </summary>
        private static void MoveAndShowText()
        {
            NextLine();
            ShowText();
        }

        /// <summary>
        /// начало пути
        ///
        /// x y m
        /// начинает новый путь с точки x, y
        /// предыдущий путь не сохраняется
        /// </summary>
        private static void BeginPath()
        {
            var y = (int)operands.Pop();
            var x = (int)operands.Pop();
            double ox;
            double oy;

            currentState.CTM.MultVector(x, y, out ox, out oy);
            pathFirstPoint = new PointF((float)ox, (float)oy);
            currentPath = new GraphicsPath();
        }

	    /// <summary>
	    /// добавление прямой линии
	    ///
	    /// x y l
	    /// добавляет сегмент прямой линии в текущий путь
	    /// </summary>
        private static void AddLine()
        {
            var y = (int)operands.Pop();
            var x = (int)operands.Pop();
            double ox;
            double oy;

            currentState.CTM.MultVector(x, y, out ox, out oy);

            PointF lastPoint;
            try
            {
                lastPoint = currentPath.GetLastPoint();
            } catch (Exception) {
                lastPoint = pathFirstPoint;
            }

            currentPath.AddLine(lastPoint, new PointF((float)ox, (float)oy));
        }

        /// <summary>
        /// Устанавливает ширину линии для текущего состояния
        /// lineWidth w
        /// </summary>
        private static void SetLineWidth()
        {
            currentState.lineWidth = (double)operands.Pop();
        }

        /// <summary>
        /// Добавляет прямоугольник в текущий путь
        /// x y width height re
        /// </summary>
        private static void AddRectangle()
        {
            var height = (int)operands.Pop();
            var width = (int)operands.Pop();
            var y = (int)operands.Pop();
            var x = (int)operands.Pop();

            operands.Push(x);
            operands.Push(y);
            BeginPath();
            operands.Push(x + width);
            operands.Push(y);
            AddLine();
            operands.Push(x + width);
            operands.Push(y + height);
            AddLine();
            operands.Push(x);
            operands.Push(y + height);
            AddLine();
            ClosePath();
        }


        private static void ClosePath()
        {

        }

        /// <summary>
        /// Добавляет обводку к текущему пути
        /// </summary>
        private static void StrokePath()
        {
            graphics.DrawPath(new Pen(Color.Black) { Width = (float)(currentState.lineWidth * currentState.CTM.GetValues()[0]) }, currentPath);
        }

        /// <summary>
        /// Перемещает позицию вывода текста на следующую строку.
	///
	/// T*
        /// В структуре состояния параметр называется double leading.
        /// Вызвать команду 0, -leading, Td #56
        /// </summary>
        private static void NextLine()
        {
            operands.Push((int)0);
            operands.Push((int)-currentState.leading);
            TextMove();
	}

        /// <summary>
        /// Отображает одну или более строк.
        /// Параметр массив, каждый элемент массива строка или число.
        /// Если элемент строка, то отображается строка используя #57
        /// Если число, то позиция текста смещается на указанное число x/1000, это число должно быть вычтено из текущей горизонтальной координаты.
        /// </summary>
        private static void ShowStrings()
        {
            foreach (object x in operands)
            {
                if (x is char[])
                {
                    operands.Push(x);
                    ShowText();
                }
                if (x is double)
                    currentState.textX += (double)x/1000;
            }
        }
    }
}
