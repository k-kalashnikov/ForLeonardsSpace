using NetTopologySuite.Geometries;

namespace Masofa.Common.Models.Satellite
{
    /// <summary>
    /// Minimum Bounding Rectangle для Landsat API
    /// </summary>
    public class LandsatMBR
    {
        /// <summary>
        /// Нижний левый угол (юго-запад)
        /// </summary>
        public Point LowerLeft { get; set; }
        
        /// <summary>
        /// Верхний правый угол (северо-восток)
        /// </summary>
        public Point UpperRight { get; set; }
        
        /// <summary>
        /// Преобразование в формат для Landsat API
        /// </summary>
        public object ToApiFormat()
        {
            return new
            {
                filterType = "mbr",
                lowerLeft = new { latitude = LowerLeft.Y, longitude = LowerLeft.X },
                upperRight = new { latitude = UpperRight.Y, longitude = UpperRight.X }
            };
        }
        
        /// <summary>
        /// Создает MBR из двух точек
        /// </summary>
        public static LandsatMBR Create(Point lowerLeft, Point upperRight)
        {
            return new LandsatMBR
            {
                LowerLeft = lowerLeft,
                UpperRight = upperRight
            };
        }
    }
}
