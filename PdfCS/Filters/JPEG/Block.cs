using System;

namespace PdfCS
{
    /// <summary>
    /// Класс блок для фильтра JPEG
    /// </summary>
    public class Block
    {
        //
        private readonly static short[] block2 = new short[8 * 8 * 4 * 4];

        //
        static byte Lerp(int x, float x0, byte y0, float x1, byte y1)
        {
            float res = y0 + (float)(y1 - y0) / (x1 - x0) * (x - x0);
            if (res > 255)
                return 255;
            else if (res < 0)
                return 0;
            else
                return (byte)res;
        }

        /*
	static void InterpolateBlock(short[,] block, int chan, int coefH, int coefV, int dataX, int dataY, byte[] data)
        {
            Console.WriteLine("interpolate %d %d %d %d\n", coefH, coefV, dataX, dataY);

            for (int x = 0; x < 8 * coefH; x++)
            {
                int x0 = x / coefH;
                if (x0 == 7)
                    x0--;
                int x1 = x0 + 1;

                Console.WriteLine("x = %d x0 = %d x1 = %d\n", x, x0, x1);

                for (int y = 0; y < 8 * coefV; y++)
                {
                    block2[y * 8 * coefH + x] = Lerp(x, x0 * coefH + 1.0f
                        / coefH, (byte)block[x0, y / coefV], x1 * coefH + 1.0f
                        / coefH, (byte)block[x1, y / coefV]);
                }
            }
            for (int y = 0; y < 8 * coefV; y++)
            {
                int x0 = y / coefV;
                if (x0 == 7)
                    x0--;
                int x1 = x0 + 1;

                Console.WriteLine("y = %d x0 = %d x1 = %d\n", y, x0, x1);

                for (int x = 0; x < 8 * coefH; x++)
                {
                    block2[y * 8 * coefH + x] = Lerp(y, x0 * coefV + 1.0f
                        / coefV, (byte)block2[x0 * coefV * 8 * coefH + x], x1 * coefV + 1.0f
                        / coefV, (byte)block2[x1 * coefV * 8 * coefH + x]);
                }
            }
            for (int y = 0; y < 8 * coefV; y++)
                for (int x = 0; x < 8 * coefH; x++)
                    data[((dataY + y) * Main.Frame.width + dataX + x) * Main.Frame.numChan + chan] = (byte)block2[y * 8 * coefH + x];
        }

        //
        void PutBlock(short[,] block, int chan, int h, int v, int dataPosX, int dataPosY, byte[] data, int vMax, int hMax)
        {
            int dataX, dataY;
            if (Main.Jpeg.channels[chan].H < hMax || Main.Jpeg.channels[chan].V < vMax)
                InterpolateBlock(block, chan, hMax / Main.Jpeg.channels[chan].H, vMax / Main.Jpeg.channels[chan].V, 
                    (dataPosX * hMax + h) * 8, (dataPosY * vMax + v) * 8, data);
            else
                for (int y = 0; y < 8; y++)
                    for (int x = 0; x < 8; x++)
                    {
                        dataX = (dataPosX * hMax + h) * 8 + x;
                        dataY = (dataPosY * vMax + v) * 8 + y;
                        data[(dataY * Main.Frame.width + dataX) * Main.Frame.numChan + chan] = (byte)block[x, y];
                    }
        }
	*/
        //
        int CorrectJpegSize(int size, int factor)
        {
            if (size % 8 == 0 && (size / factor) % 8 == 0)
                return size;
            while ((size / factor) % 8 != 0)
                size++;

            return size;
        }
    }
}
