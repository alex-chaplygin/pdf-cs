using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Input;

namespace PdfCS
{
    /// <summary>
    /// 
    /// </summary>
    public class PdfGraphics
    {
        /// <summary>
        /// функция оператора графики
        /// </summary>
        delegate void Operator();

        /// <summary>
        /// 
        /// </summary>
        private static Dictionary<string, Operator> commands = new Dictionary<string, Operator>
        {
            {"cm", new Operator(SetMatrix)},
        };

        /// <summary>
        ///  объект из формы, куда выводится графика
        /// </summary>
        private static Graphics graphics;

        /// <summary>
        /// состояние графики
        /// </summary> 
        private struct State
        {
            /// <summary>
            /// текущяя матрица трансформации #50
            /// </summary>
            public static Matrix CTM;
        };

        private static State currentState;

        /// <summary>
        /// стек операндов команд
        /// </summary>
        private static Stack operands;

        private static Dictionary<string, Operator> Commands { get => commands; set => commands = value; }

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

            State.CTM = matrix.Mult(State.CTM);
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
    }
}
