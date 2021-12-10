using System.Drawing;

namespace PdfCS
{
    /// <summary>
    /// Кривой участок пути
    /// </summary>
    public class Curve : Segment
    {
      /// <summary>
      /// Направляющие точки кривой - {начальная, конечная}
      /// </summary>
      public Point[] points;
    }
}
