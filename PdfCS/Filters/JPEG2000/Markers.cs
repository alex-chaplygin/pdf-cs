namespace PdfCS
{
    public enum Marker
    {
	/// <summary>
	///   начало изображения
	/// </summary>
        StartOfCodestream = 0xFF4F,

	/// <summary>
	///   Начало части тайла
	/// </summary>
        StartOfTilepart = 0xFF90,

	/// <summary>
	///   начало данных части тайла
	/// </summary>
        StartOfData = 0xFF93,

	/// <summary>
	///   конец изображения
	/// </summary>
        EndOfCodestream = 0xFFD9,

	/// <summary>
	///   параметры изображения
	/// </summary>
        ImageAndTileSize = 0xFF51,

	/// <summary>
	///   параметры кодирования по умолчанию
	/// </summary>
        CodingStyleDefault = 0xFF52,

	/// <summary>
	///   параметры кодирования компонента
	/// </summary>
        CodingStyleComponent = 0xFF53,

	/// <summary>
	///   начало региона
	/// </summary>
        RegionOfInterest = 0xFF5E,

	/// <summary>
	///   параметры квантования по умолчанию
	/// </summary>
        QuantizationDefault = 0xFF5C,

	/// <summary>
	///   параметры квантования компонента
	/// </summary>
        QuantizationComponent = 0XFF5D,

	/// <summary>
	///   параметры прогрессии
	/// </summary>
        ProgressionOrderChange = 0xFF5F,

	/// <summary>
	///   размеры части потока для тайла
	/// </summary>
        TilePartLengths = 0xFF55,

	/// <summary>
	///   длины пакетов для всех тайлов в главном заголовке
	/// </summary>
        PacketLengthOfMainHeader = 0xFF57,

	/// <summary>
	///   длины пакетов для всех тайлов в заголовке тайла
	/// </summary>
        PacketLengthOfTilePartHeader = 0xFF58,

	/// <summary>
	///   заголовки пакетов для всех тайлов в главном заголовке
	/// </summary>
        PackedPacketHeadersOfMainHeader = 0xFF60,

	/// <summary>
	///   заголовки пакетов для всех тайлов в заголовке тайла
	/// </summary>
        PackedPacketHeadersOfTilePartHeader = 0xFF61,

	/// <summary>
	///   начало пакета
	/// </summary>
        StartOfPacket = 0xFF91,

	/// <summary>
	///   конец заголовка пакета
	/// </summary>
        EndOfPacketHeader = 0xFF92,

	/// <summary>
	///   регистрация компонента
	/// </summary>
        ComponentRegistration = 0xFF63,

	/// <summary>
	///   комментарий
	/// </summary>
        Comment = 0xFF64,
    }
}
