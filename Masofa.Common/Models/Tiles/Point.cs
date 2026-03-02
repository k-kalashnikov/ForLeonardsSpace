using System.Globalization;

namespace Masofa.Common.Models.Tiles
{
    /// <summary>
    /// Вспомогательная точка (x,y,zoom) для ссылок на тайлы/слои.
    /// </summary>
    public class Point : BaseEntity
    {
        /// <summary>
        /// Координата X
        /// </summary>
        public decimal X { get; set; }

        /// <summary>
        /// Координата Y
        /// </summary>
        public decimal Y { get; set; }

        /// <summary>
        /// Zoom-уровень
        /// </summary>
        public int Zoom { get; set; }

        #region ImportFromCoord
        /// <summary>
        /// Конвертирует географические координаты в метры Web-Mercator
        /// и сохраняет результат в <see cref="X"/> и <see cref="Y"/>.
        /// </summary>
        /// <param name="lat">Широта (WGS-84) в градусах.</param>
        /// <param name="lon">Долгота (WGS-84) в градусах.</param>
        public void ImportFromCoord(double lat, double lon)
        {
            const double earthRadius = 6378137.0;
            const double originShift = Math.PI * earthRadius;

            double mx = lon * originShift / 180.0;
            double my = Math.Log(Math.Tan((90.0 + lat) * Math.PI / 360.0)) / (Math.PI / 180.0);
            my = my * originShift / 180.0;

            X = (decimal)mx;
            Y = (decimal)my;
        }

        /// <summary>
        /// Перегрузка <see cref="ImportFromCoord(double,double)"/> для строковых координат.
        /// </summary>
        /// <param name="lat">Широта в виде строки.</param>
        /// <param name="lon">Долгота в виде строки.</param>
        public void ImportFromCoord(string lat, string lon)
        {
            double latitude = double.Parse(lat, CultureInfo.InvariantCulture);
            double longitude = double.Parse(lon, CultureInfo.InvariantCulture);
            ImportFromCoord(latitude, longitude);
        }
        #endregion
    }

    public class PointHistory : BaseHistoryEntity<Point> { }
}
