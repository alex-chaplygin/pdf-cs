using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace PdfCS
{
    public class PdfImage
    {
        /// <summary>
        /// ширина в точках
        /// </summary>
        private int width;

        /// <summary>
        /// высота в точках
        /// </summary>
        private int height;

        /// <summary>
        /// пространство цветов
        /// </summary>
        private string colorSpace;

        /// <summary>
        /// число бит на цветовой компонент
        /// </summary>
        private int bitsPerComponent;

        /// <summary>
        /// Преобразованное изображение
        /// </summary>
        public Bitmap bitmap;

        /// <summary>
        /// Конструктор создающий изображение
        /// </summary>
        /// <param name="dic">словарь изображения</param>
        /// <param name="stream">поток данных</param>
        public PdfImage (Dictionary<string, object> dic, byte[] stream)
        {
            if (dic.ContainsKey("Type") && dic["Type"] is string && (string)dic["Type"] != "XObject")
                throw new Exception("Type не XObject");
            if (dic.ContainsKey("Subtype") && dic["Subtype"] is string && (string)dic["Subtype"] != "Image")
                throw new Exception("Subtype не Image");
            if (dic.ContainsKey("Width") && dic["Width"] is int)
                width = (int)dic["Width"];
            if (dic.ContainsKey("Height") && dic["Height"] is int)
                height = (int)dic["Height"];
            if (dic.ContainsKey("ColorSpace"))
                if (dic["ColorSpace"] is Array)
                    throw new Exception("ColorSpace - массив");
                else
                    colorSpace = (string)dic["ColorSpace"];
            if (dic.ContainsKey("BitsPerComponent") && dic["BitsPerComponent"] is int)
                bitsPerComponent = (int)dic["BitsPerComponent"];
            GCHandle gch = GCHandle.Alloc(stream, GCHandleType.Pinned);
            IntPtr scan0 = gch.AddrOfPinnedObject();
            if (bitsPerComponent == 1)
                bitmap = new Bitmap(width, height, 0, PixelFormat.Format1bppIndexed, scan0);
            else if (bitsPerComponent == 4)
                bitmap = new Bitmap(width, height, 0, PixelFormat.Format4bppIndexed, scan0);
            else if(bitsPerComponent == 8)
                bitmap = new Bitmap(width, height, 0, PixelFormat.Format8bppIndexed, scan0);
            else if(bitsPerComponent == 16)
                bitmap = new Bitmap(width, height, 0, PixelFormat.Format16bppRgb555, scan0);
        }
    }
}
