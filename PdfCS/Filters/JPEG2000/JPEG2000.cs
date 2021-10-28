using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfCS
{
    /// <summary>
    /// Класс, декодирующий изображение формата JPEG2000
    /// </summary>
    public class JPEG2000
    {
	/// <summary>
	///   объект для чтения бит
	/// </summary>
        private BitReader bitReader;

	/// <summary>
	///   параметры изображения ImageAndTileSize
	/// </summary>
	private ImageParams imageParams;

	/// <summary>
	///   общие параметры кодирования
	/// </summary>
	private CodingStyle codingStyle;

	/// <summary>
	///   параметры квантования
	/// </summary>
	private Quantization quantization;

	/// <summary>
	///   информация о текущей части тайла
	/// </summary>
	private TilePartInfo tilePartInfo;
	
	/// <summary>
	///   распакованное изображение
	/// </summary>
	private byte[] unpackedData;

	/// <summary>
	///   Создает объект чтения JPEG2000
	/// </summary>
        /// <param name="data">закодированное изображение</param>
        public JPEG2000(byte[] data)
        {
            bitReader = new BitReader(data);
        }

	/// <summary>
	///   Метод декодирования фильтра JPEG2000
	/// </summary>
        /// <param name="data">закодированное изображение</param>
        /// <returns>
        /// распакованный массив байт
        /// </returns>
	public static byte[] Decode(byte[] data)
	{
	    JPEG2000 j = new JPEG2000(data);
	    j.Read();
	    return j.unpackedData;
	}
	
        /// <summary>
        /// Считывает маркер из потока
        /// </summary>      
        ///  <returns>Двухбайтовое число</returns>
        private Marker ReadMarker()
        {
            return (Marker)bitReader.GetBitsLS(16);
        }

	/// <summary>
	///   Читает все изображение
	/// </summary>
	private void Read()
	{
	    Marker m = ReadMarker();
	    if (ReadMarker() != Marker.StartOfCodestream)
		throw new Exception("Нет StartOfCodestream");
	    ReadMain();
	    while (true)
	    {
		tilePartInfo = new TilePartInfo(bitReader);
		ReadTilePart();
		m = ReadMarker();
		if (m == Marker.StartOfTilepart)
		    continue;
		else if (m != Marker.EndOfCodestream)
		    break;
		else
		    throw new Exception($"Read: Неправильный маркер {(ushort)m}");
	    }
	}

	/// <summary>
	///   Считывает главный заголовок
	/// </summary>
	private void ReadMain()
	{
	    Marker m = ReadMarker();
	    if (m != Marker.ImageAndTileSize)
		throw new Exception("Нет ImageAndTileSize");
	    imageParams = new ImageParams(bitReader);
	    while (m != Marker.StartOfTilepart)
	    {
		m = ReadMarker();
		if (m == Marker.CodingStyleDefault)
		    codingStyle = new CodingStyle(bitReader);
		else if (m == Marker.QuantizationDefault)
		    quantization = new Quantization(bitReader);
		// другие маркеры ...
		else
		    throw new Exception($"ReadMain: Неправильный маркер {(ushort)m}");
	    }
	}

	/// <summary>
	///   чтение части тайла
	/// </summary>
	private void ReadTilePart()
	{
	    Marker m = ReadMarker();
	    while (m != Marker.StartOfData)
	    {
		// читаем маркеры заголовка части тайла
	    }
	    ReadStream();	    
	}

	/// <summary>
	///   чтение потока данных части тайла
	/// </summary>
	private void ReadStream()
	{
	}	
    }
}
