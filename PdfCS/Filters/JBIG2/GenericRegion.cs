using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace PdfCS
{
    /// <summary>
    /// Класс для обработки GRDP
    /// (generic region decoding procedure)
    /// </summary>
    public class GenericRegion
    {
        /// <summary>
        /// Флажок о том, используется ли кодирование MMR
        /// </summary>
        private bool useMMR;

        /// <summary>
        /// Размеры декодируемого региона
        /// </summary>
        private Size sizeGB;

        /// <summary>
        /// Идентификатор шаблона
        /// </summary>
        private int? idTemplate = null;

        /// <summary>
        /// Флажок для включения/выключения использования 
        /// типичного предсказания для GRDP
        /// </summary>
        private bool usePrediction = false;

        /// <summary>
        /// Флажок о том, следует ли пропускать некоторые пиксели при декодировании
        /// </summary>
        private bool useSkip = false;

        /// <summary>
        /// Bitmap, указывающий, какие пиксели следует пропустить
        /// Размер - sizeGB
        /// </summary>
        private bool[,] skip = null;

        /// <summary>
        /// Массив специальных пикселей адаптивного шаблона
        /// </summary>
        private Point[] templatePixels = null;

        /// <summary>
        /// Инициализируем значения закрытых полей класса
        /// с помощью методов Read, отведенных отдельно для каждого поля
        /// </summary>
        /// <param name="bR"></param> Экземпляр класса BitReader, дает возможность свободно считывать биты из потока
        public GenericRegion(BitReader bR)
        {
            ReadUseMMR(bR);
            ReadSize(bR);
            ReadTemplateId(bR);
            ReadUsePredictor(bR);
            ReadUseSkip(bR);
            ReadSkip(bR);
            ReadTemplatePixels(bR);
        }

        /// <summary>
        /// Устанавливаем значение для поля useMMR
        /// </summary>
        private void ReadUseMMR(BitReader bR)
        {
            useMMR = (bR.GetBitMS() != 0);
        }

        /// <summary>
        /// Устанавливаем значение для поля sizeGB
        /// </summary>
        private void ReadSize(BitReader bR)
        {
            sizeGB = new Size(bR.GetBitsMS(32), bR.GetBitsMS(32));
        }

        /// <summary>
        /// Устанавливаем значение для поля idTemplate
        /// В случае, если useMMR = 1 -> оставляем значение по умолчанию
        /// </summary>
        private void ReadTemplateId(BitReader bR)
        {
            if (useMMR)
                return;

            idTemplate = bR.GetBitsMS(2);
        }

        /// <summary>
        /// Устанавливаем значение для поля usePrediction
        /// В случае, если useMMR = 1 -> оставляем значение по умолчанию
        /// </summary>
        private void ReadUsePredictor(BitReader bR)
        {
            if (useMMR)
                return;

            usePrediction = (bR.GetBitMS() != 0);
        }

        /// <summary>
        /// Устанавливаем значение для поля useSkip
        /// В случае, если useMMR = 1 -> оставляем значение по умолчанию
        /// </summary>
        private void ReadUseSkip(BitReader bR)
        {
            if (useMMR)
                return;

            useSkip = (bR.GetBitMS() != 0);
        }

        /// <summary>
        /// Инициализируем матрицу skip
        /// В случае, если useMMR = 1 или useSkip = false -> оставляем состояние матрицы по умолчанию
        /// В случае, если useMMR != 1 и useSkip = true   -> инициализируем матрицу skip размера sizeGB 
        /// </summary>
        private void ReadSkip(BitReader bR)
        {
            if (useMMR || !useSkip)
                return;

            skip = new bool[sizeGB.Width, sizeGB.Height];

            for (int i = 0; i < sizeGB.Width; i++)
                for (int j = 0; j < sizeGB.Height; j++)
                    skip[i, j] = (bR.GetBitMS() != 0);
        }

        /// <summary>
        /// Инициализируем массив адаптивных шаблонов для пикселей templatePixels
        /// В случае, если useMMR = 1      -> оставляем состояние массива по умолчанию
        /// В случае, если useMMR != 1     -> инциализируем массив из 4 пикселей значениями null, устанавливаем значение для первого пикселя
        /// В случае, если idTemplate = 0  -> устанавливаем значения для остальных пикселей
        /// В случае, если idTemplate != 0 -> оставляем значения для остальных пикселей без изменений
        /// </summary>
        private void ReadTemplatePixels(BitReader bR)
        {
            if (useMMR)
                return;

            if (idTemplate == 0)
                templatePixels = new Point[4];
            else
                templatePixels = new Point[1];

            for (int i = 0; i < templatePixels.Length; i++)
                templatePixels[i] = new Point(bR.GetBitsMS(8), bR.GetBitsMS(8));
        }
    }
}
