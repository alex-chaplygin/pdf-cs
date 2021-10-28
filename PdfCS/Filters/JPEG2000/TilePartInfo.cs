namespace PdfCS
{
    /// <summary>
    /// Информация о части тайле
    /// </summary>
    public class TilePartInfo
    {
	/// <summary>
	///   длина сегмента (без маркера)
	/// </summary>
	public ushort length;

	/// <summary>
	///   глобальный номер тайла в изображении, начиная с 0
	/// </summary>
	public ushort tileIndex;

	/// <summary>
	///   длина в байтах от первого байта StartOfTile маркера до конца данных части тайла
	///   для последнего тайла равна 0
	/// </summary>
	public uint tilePartLength;

	/// <summary>
	///   индекс части тайла, начиная с 0
	///   не обязательно идут последовательно
	/// </summary>
	public byte tilePartIndex;

	/// <summary>
	///   число частей в тайле
	///   если равно 0, то в этой части не указано число (оно в другой части)
	/// </summary>
	public byte numTileParts;
	
	/// <summary>
	///   читает информацию о части тайла
	/// </summary>
        /// <param name="br">объект чтения бит</param>
	public TilePartInfo(BitReader br)
	{
	    length = (ushort)br.GetBitsLS(16);
	    tileIndex = (ushort)br.GetBitsLS(16);
	    tilePartLength = (uint)br.GetBitsLS(32);
	    tilePartIndex = (byte)br.GetBitsLS(8);
	    numTileParts = (byte)br.GetBitsLS(8);
	}
    }
}
