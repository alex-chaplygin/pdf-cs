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
            public double textFontSize;

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

            /// <summary>
            /// Цвет заполнения пути
            /// </summary>
            public Color fillColor;

            /// <summary>
            /// Цвет обводки пути
            /// </summary>
            public Color strokeColor;

	    /// <summary>
            /// Регион отсечения
            /// </summary>
            public Region clippingRegion;

            /// <summary>
            /// Текущий путь
            /// </summary>
            public GraphicsPath currentPath;

            /// <summary>
            /// Первая точка пути
            /// </summary>
            public PointF pathFirstPoint;

            /// <summary>
            /// размещение следующего символа, относительно предыдущего по горизонтали
            /// </summary>
            public double displacementX;

            /// <summary>
            /// размещение следующего символа, относительно предыдущего по вертикали
            /// </summary>
            public double displacementY;
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
            {"h", new Operator(ClosePath)},
            {"l", new Operator(AddLine)},
            {"Td", new Operator(TextMove)},
            {"TL", new Operator(SetLeading)},
            {"TD", new Operator(TextMoveLeading)},
            {"w", new Operator(SetLineWidth)},
            {"re", new Operator(AddRectangle)},
            {"T*", new Operator(NextLine)},
            {"TJ", new Operator(ShowStrings)},
            {"S", new Operator(StrokePath)},
            {"SC", new Operator(SetStrokeColor)},
            {"sc", new Operator(SetFillColor)},
            {"f", new Operator(FillPath)},
            {"F", new Operator(FillPath)},
        {"f*", new Operator(FillPathEven)},
        {"B", new Operator(FillAndThenStrokePathNonZero)},
            {"B*", new Operator(FillAndThenStrokePathEvenOddRule)},
            {"b", new Operator(CloseFillAndThenStrokePathNonZero)},
            {"b*", new Operator(CloseFillAndThenStrokePathEvenOddRule)},
            {"'",  new Operator(MoveAndShowText)},
        {"c", new Operator(AddCurve3)},
            {"v", new Operator(AddCurve2)},
            {"y", new Operator(AddCurve1)},
	    {"n", new Operator(() => {}) },
	    {"W", new Operator(SetClipPath) },
            {"W*", new Operator(SetClipPath2) },
            {"d0", new Operator(SetDisplacement)},
            {"d1", new Operator(SetDisplacementBoundingBox) },
            {"Do", new Operator(PaintObject) }
        };

        /// <summary>
        /// стек операндов команд
        /// </summary>
        private static Stack<object> operands;

        /// <summary>
        /// Словарь ресурсов
        /// </summary>
        private static Dictionary<string, object> resources;

        /// <summary>
        /// Инициализация графики
        /// </summary>
        /// <param name="g">Объект из формы, в который выводится графика</param>
        /// <param name="r">Система координат страницы</param>
        public static void Init(Graphics g, Rectangle r)
        {
            currentState.beginText = false;
            currentState.fillColor = Color.Black;
            currentState.strokeColor = Color.Black;
            graphics = g;
            mediaBox = r;
            operands = new Stack<object>();
            states = new Stack<State>();
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
            double f = ReadNumber();
            double e = ReadNumber();
            double d = ReadNumber();
            double c = ReadNumber();
            double b = ReadNumber();
            double a = ReadNumber();

            Matrix matrix = new Matrix(a, b, c, d, e, f);

            currentState.CTM = matrix.Mult(currentState.CTM);
        }

        /// <summary>
        /// Устанавливает текстовую матрицу
        /// Параметры извлекаются из стека операндов внутри метода.
        /// </summary>
        private static void SetTextMatrix()
        {
            double f = ReadNumber();
            double e = ReadNumber();
            double d = ReadNumber();
            double c = ReadNumber();
            double b = ReadNumber();
            double a = ReadNumber();

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
            currentState.textFontSize = ReadNumber();
            operands.Pop();
        }

        /// <summary>
        /// Отрисовка страницы
        /// </summary>
        /// <param name="content">поток содержимого страницы (команды графики)</param>
        /// <param name="resources">словарь ресурсов страницы</param>
        public static void Render(byte[] content, Dictionary<string, object> resource)
        {
            Parser parser = new Parser(new MemoryStream(content));
            parser.NextChar();
            object temp;
            resources = resource;
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
            double ty = ReadNumber();
            double tx = ReadNumber();
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
            currentState.leading = ReadNumber();
        }

        /// <summary>
        /// установить leading как -ty
        ///перемещение позиции текста tx ty
        ///tx ty TD
        /// </summary>
        static void TextMoveLeading()
        {
            double num = ReadNumber();
            currentState.leading = -num;
            operands.Push(num);
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
            (float)Math.Abs(currentState.textFontSize * currentState.CTM.GetValues()[3])),
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
            double y = ReadNumber();
            double x = ReadNumber();
            double ox;
            double oy;
            currentState.CTM.MultVector(x, y, out ox, out oy);

            currentState.pathFirstPoint = new PointF((float)ox, (float)oy);
            currentState.currentPath = new GraphicsPath();
        }

        /// <summary>
        /// добавление прямой линии
        ///
        /// x y l
        /// добавляет сегмент прямой линии в текущий путь
        /// </summary>
        private static void AddLine()
        {
            double y = ReadNumber();
            double x = ReadNumber();
            double ox;
            double oy;
            currentState.CTM.MultVector(x, y, out ox, out oy);

            PointF lastPoint;
            try
            {
                lastPoint = currentState.currentPath.GetLastPoint();
            }
            catch (Exception)
            {
                lastPoint = currentState.pathFirstPoint;
            }
            if (currentState.currentPath == null)
                currentState.currentPath = new GraphicsPath();
            currentState.currentPath.AddLine(lastPoint, new PointF((float)ox, (float)oy));
        }

        /// <summary>
        /// Устанавливает ширину линии для текущего состояния
        /// lineWidth w
        /// </summary>
        private static void SetLineWidth()
        {
            currentState.lineWidth = ReadNumber();
        }

        /// <summary>
        /// Добавляет прямоугольник в текущий путь
        /// x y width height re
        /// </summary>
        private static void AddRectangle()
        {
            double height = ReadNumber();
            double width = ReadNumber();
            double y = ReadNumber();
            double x = ReadNumber();

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

        /// <summary>
        /// Закрывает текущий путь #66
        /// 
        /// добавляет прямую линию из текущей (последней точки) в начальную точку пути.
        /// если последний сегмент пути уже замкнул путь, то ничего не делает.
        /// </summary>
        private static void ClosePath()
        {
            PointF lastPoint;
            try
            {
                lastPoint = currentState.currentPath.GetLastPoint();
            }
            catch (Exception)
            {
                lastPoint = currentState.pathFirstPoint;
            }
            if (currentState.currentPath == null)
                currentState.currentPath = new GraphicsPath();

            if (lastPoint != currentState.pathFirstPoint)
                currentState.currentPath.AddLine(lastPoint, currentState.pathFirstPoint);
        }

        private static void SetStrokeColor()
        {
            currentState.strokeColor = ReadRGBColor();
        }

        private static void SetFillColor()
        {
            currentState.fillColor = ReadRGBColor();
        }

        /// <summary>
        /// f
        /// F
        /// Заполнение текущего пути с обходом контура non zero winding
        /// </summary>
        private static void FillPath()
        {
            ClosePath();
            currentState.currentPath.FillMode = FillMode.Winding;
            graphics.FillPath(new SolidBrush(currentState.fillColor), currentState.currentPath);
        }

        /// <summary>
        /// f*
        /// Заполнение текущего пути с обходом контура even-odd
        /// </summary>
        private static void FillPathEven()
        {
            ClosePath();
            currentState.currentPath.FillMode = FillMode.Alternate;
            graphics.FillPath(new SolidBrush(currentState.fillColor), currentState.currentPath);
        }

        /// <summary>
        /// Cначала заполняет контур с правилом не нулевого контура, потом обводит.
        /// 
        /// Operator - B
        /// </summary>
        private static void FillAndThenStrokePathNonZero()
        {
            currentState.currentPath.FillMode = FillMode.Winding;
            graphics.FillPath(new SolidBrush(currentState.fillColor), currentState.currentPath);

            graphics.DrawPath(new Pen(currentState.strokeColor) {
                Width = (float)(currentState.lineWidth * currentState.CTM.GetValues()[0]) }, currentState.currentPath);
        }

        /// <summary>
        /// Тоже что и последовательность h B.
        /// 
        /// Operator - b
        /// </summary>
        private static void CloseFillAndThenStrokePathNonZero()
        {
            ClosePath();

            FillAndThenStrokePathNonZero();
        }

        /// <summary>
        /// Сначала заполняет контур с правилом четный-нечетный, потом обводит.
        /// 
        /// Operator - B*
        /// </summary>
        private static void FillAndThenStrokePathEvenOddRule()
        {
            currentState.currentPath.FillMode = FillMode.Alternate;
            graphics.FillPath(new SolidBrush(currentState.fillColor), currentState.currentPath);

            graphics.DrawPath(new Pen(currentState.strokeColor) {
                Width = (float)(currentState.lineWidth * currentState.CTM.GetValues()[0]) }, currentState.currentPath);
        }

        /// <summary>
        /// Тоже что и последовательность h B*.
        /// 
        /// Operator - b*
        /// </summary>
        private static void CloseFillAndThenStrokePathEvenOddRule()
        {
            ClosePath();
            FillAndThenStrokePathEvenOddRule();
        }

        /// <summary>
        /// S
        /// Добавляет обводку к текущему пути
        /// </summary>
        private static void StrokePath()
        {
            graphics.DrawPath(new Pen(currentState.strokeColor) {
		    Width = (float)(currentState.lineWidth * currentState.CTM.GetValues()[0])
		}, currentState.currentPath);
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
            operands.Push(0);
            operands.Push(-currentState.leading);
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
                    currentState.textX += (double)x / 1000;
            }
        }

        /// <summary>
        /// Cчитывает число (целое или вещественное) из стека операндов и преобразует его в double
        /// </summary>
        /// <returns> Вещественное число из стека операндов </returns> 
        private static double ReadNumber()
        {
            return Convert.ToDouble(operands.Pop());
        }

        /// <summary>
        /// Cчитывает цвет из стека операндов
        /// </summary>
        private static Color ReadRGBColor()
        {
            int b = (int)operands.Pop();
            int g = (int)operands.Pop();
            int r = (int)operands.Pop();
            return Color.FromArgb(255, r, g, b);
        }

        /// <summary>
        /// x1 y1 x2 y2 x3 y3 c
        /// Добавляет в текущий путь #66 кривую Безье
        /// p1, p2 - контрольные точки
        /// p3 - конечная точка
        /// </summary>
        private static void AddCurve3()
        {
            PointF lastPoint;
            try
            {
                lastPoint = currentState.currentPath.GetLastPoint();
            }
            catch (Exception)
            {
                lastPoint = currentState.pathFirstPoint;
            }
            double y3 = ReadNumber();
            double x3 = ReadNumber();
            double y2 = ReadNumber();
            double x2 = ReadNumber();
            double y1 = ReadNumber();
            double x1 = ReadNumber();

            currentState.CTM.MultVector(x1, y1, out x1, out y1);
            currentState.CTM.MultVector(x2, y2, out x2, out y2);
            currentState.CTM.MultVector(x3, y3, out x3, out y3);

            PointF p0 = lastPoint;
            PointF p1 = new PointF((float)x1, (float)y1);
            PointF p2 = new PointF((float)x2, (float)y2);
            PointF p3 = new PointF((float)x3, (float)y3);
            currentState.currentPath.AddBezier(p0, p1, p2, p3);
        }

        /// <summary>
        /// x2 y2 x3 y3 v
        /// Добавляет в текущий путь #66 кривую Безье
        /// контрольная точка p1 берется из последней точки пути
        /// p2 - контрольная точка
        /// p3 - конечная точка
        /// </summary>
        private static void AddCurve2()
        {
            PointF lastPoint;
            try
            {
                lastPoint = currentState.currentPath.GetLastPoint();
            }
            catch (Exception)
            {
                lastPoint = currentState.pathFirstPoint;
            }
            double y3 = ReadNumber();
            double x3 = ReadNumber();
            double y2 = ReadNumber();
            double x2 = ReadNumber();

            currentState.CTM.MultVector(x2, y2, out x2, out y2);
            currentState.CTM.MultVector(x3, y3, out x3, out y3);

            PointF p0 = lastPoint;
            PointF p1 = p0;
            PointF p2 = new PointF((float)x2, (float)y2);
            PointF p3 = new PointF((float)x3, (float)y3);
            currentState.currentPath.AddBezier(p0, p1, p2, p3);
        }

        /// <summary>
        /// x1 y1 x3 y3 y
        /// Добавляет в текущий путь #66 кривую Безье
        /// контрольная точка p2 совпадает с точкой p3
        /// p1 - контрольная точка
        /// p3 - конечная точка
        /// </summary>
        private static void AddCurve1()
        {
            PointF lastPoint;
            try
            {
                lastPoint = currentState.currentPath.GetLastPoint();
            }
            catch (Exception)
            {
                lastPoint = currentState.pathFirstPoint;
            }
            double y3 = ReadNumber();
            double x3 = ReadNumber();
            double y1 = ReadNumber();
            double x1 = ReadNumber();

            currentState.CTM.MultVector(x1, y1, out x1, out y1);
            currentState.CTM.MultVector(x3, y3, out x3, out y3);

            PointF p0 = lastPoint;
            PointF p1 = new PointF((float)x1, (float)y1);
            PointF p3 = new PointF((float)x3, (float)y3);
            PointF p2 = p3;
            currentState.currentPath.AddBezier(p0, p1, p2, p3);
        }

	/// <summary>
        /// W
        /// устанавливает путь отсечения используя правило nonzero winding (ненулевой контур)
        /// текущий путь используется для отсечения(clipping) региона рисования
        /// </summary>
        private static void SetClipPath()
        {
            currentState.currentPath.FillMode = FillMode.Winding;
            graphics.SetClip(currentState.currentPath);
            currentState.clippingRegion = graphics.Clip;
        }

        /// <summary>
        /// W*
        /// устанавливает путь отсечения используя правило even-odd winding (четный-нечетный контур)
        /// текущий путь используется для отсечения(clipping) региона рисования
        /// </summary>
        private static void SetClipPath2()
        {
            currentState.currentPath.FillMode = FillMode.Alternate;
            graphics.SetClip(currentState.currentPath);
            currentState.clippingRegion = graphics.Clip;           
        }

	/// <summary>
        /// d0
        /// wx wy
        /// Устанавливает ширину глифа и объявляет, что описание глифа указывает как его форму, так и его цвет
        /// </summary>
        private static void SetDisplacement()
        {
            currentState.displacementX = ReadNumber();
            currentState.displacementY = ReadNumber();
        }

        /// <summary>
        /// d1
        /// wx wy llx lly urx ury
        /// Устанавливает ширину и ограничительную рамку глифа и объявляет, что описание глифа указывает только на его форму
        /// </summary>
        private static void SetDisplacementBoundingBox()
        {
            currentState.displacementX = ReadNumber();
            currentState.displacementY = ReadNumber();
            double llx = ReadNumber();
            double lly = ReadNumber();
            double urx = ReadNumber();
            double ury = ReadNumber();
            RectangleF boundingBox = new RectangleF((float)llx, (float)lly, (float)urx, (float)ury);
        }	

        /// <summary>
        /// отрисовка внешнего объекта
        /// параметр - имя объекта, это имя является ключом в подсловаре XObject в словаре ресурсов
        /// словарь ресурсов передается как параметр в Render его нужно сохранить в классе как поле
        /// Dictionary<string, object> resources;
        /// Значение по ключу в подсловаре XObject является ссылкой на объект - поток
        /// Его загружаем из PDF файла.Если тип объекта (Type) - Image, то создаем объект PdfImage
        /// и рисуем bitmap.Оконные координаты получаются путем умножения вектора (0, 0) на матрицу CTM
        /// размеры изображения(оно может масштабироваться) получаются умножением вектора(1, 1) на матрицу CTM
        /// нужно нарисовать изображение с новыми размерами, а не с исходными
        /// </summary>
        private static void PaintObject()
        {
	    Dictionary<string,object> dict;
	    double x;
	    double y;
	    double xx;
	    double yy;
            string param = (string)operands.Pop();
            object link = ((Dictionary<string, object>)resources["XObject"])[param];
            byte[] stream =  (byte[])PDFFile.LoadLink(link, out dict);
            if ((string)dict["Type"] == "XObject" && (string)dict["Subtype"] == "Image")
            {
                currentState.CTM.MultVector(0, 0, out x, out y);
                currentState.CTM.MultVector((double)dict["Width"], (double)dict["Height"], out xx, out yy);
                PdfImage image = new PdfImage(dict, stream);
                graphics.DrawImage(image.bitmap, (float)x, (float)y, (float)xx, (float)yy);
            }
        }
    }
}
