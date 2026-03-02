using Masofa.Common.Models.Satellite;
using System.Globalization;

namespace Masofa.Common.Models.Tiles
{
    /// <summary>
    /// Тайл подложки (растровый спутниковый снимок или другой растр),
    /// привязанный к системе координат Web-Mercator (EPSG:3857).
    /// </summary>
    public class Tile : BaseEntity
    {
        /// <summary>
        /// Координата X левого-верхнего угла тайла
        /// </summary>
        public decimal X { get; set; }

        /// <summary>
        /// Координата Y левого-верхнего угла тайла
        /// </summary>
        public decimal Y { get; set; }

        /// <summary>
        /// Размер тайла по оси X (ширина)
        /// </summary>
        public decimal Length { get; set; }

        /// <summary>
        /// Размер тайла по оси Y (высота)
        /// </summary>
        public decimal Width { get; set; }

        /// <summary>
        /// Уровень масштабирования (zoom-уровень)
        /// </summary>
        public int Zoom { get; set; }

        /// <summary>
        /// Идентификатор файла тайла в хранилище
        /// </summary>
        public Guid FileId { get; set; }

        /// <summary>
        /// Дата/время снимка, который представлен тайлом
        /// </summary>
        public DateTime TileSnapShotDate { get; set; }

        /// <summary>
        /// Источник данных
        /// </summary>
        public ProductSourceType ProductSourceType { get; set; }

        /// <summary>
        /// Внешний ключ на конкретный продукт-источник
        /// </summary>
        public Guid ProductSourceId { get; set; }

        #region ImportFromCoord
        /// <summary>
        /// Заполняет поля X, Y, Length, Width, исходя из географических координат
        /// левого-верхнего угла и текущего значения <see cref="Zoom"/>.
        /// </summary>
        /// <param name="lat">Широта (WGS-84) в градусах.</param>
        /// <param name="lon">Долгота (WGS-84) в градусах.</param>
        public void ImportFromCoord(double lat, double lon)
        {
            const int tilePixelSize = 256;
            const double earthRadius = 6378137.0;
            const double initialResolution = 2 * Math.PI * earthRadius / tilePixelSize;
            const double originShift = Math.PI * earthRadius;

            // Переводим lat/lon в метры Web-Mercator
            double mx = lon * originShift / 180.0;
            double my = Math.Log(Math.Tan((90.0 + lat) * Math.PI / 360.0)) / (Math.PI / 180.0);
            my = my * originShift / 180.0;

            // Разрешение (метров/пиксель) на текущем zoom
            double res = initialResolution / Math.Pow(2, Zoom);

            // Пиксельные координаты относительно левого-верхнего угла мира
            double px = (mx + originShift) / res;
            double py = (my + originShift) / res;

            // Номер тайла
            int tileX = (int)Math.Floor(px / tilePixelSize);
            int tileY = (int)Math.Floor(py / tilePixelSize);

            // Координаты левого-верхнего угла тайла в метрах
            double tileMinX = tileX * tilePixelSize * res - originShift;
            double tileMinY = tileY * tilePixelSize * res - originShift;

            // Записываем в модель
            X = (decimal)tileMinX;
            Y = (decimal)tileMinY;
            Length = (decimal)(tilePixelSize * res);
            Width = (decimal)(tilePixelSize * res);
        }

        /// <summary>
        /// Перегрузка <see cref="ImportFromCoord(double,double)"/> для строковых аргументов.
        /// </summary>
        /// <param name="lat">Широта в виде строки с десятичной точкой.</param>
        /// <param name="lon">Долгота в виде строки с десятичной точкой.</param>
        public void ImportFromCoord(string lat, string lon)
        {
            double latitude = double.Parse(lat, CultureInfo.InvariantCulture);
            double longitude = double.Parse(lon, CultureInfo.InvariantCulture);
            ImportFromCoord(latitude, longitude);
        }
        #endregion
    }

    public class TileHistory : BaseHistoryEntity<Tile> { }
}