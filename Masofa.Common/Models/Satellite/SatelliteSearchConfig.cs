using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Satellite
{
    /// <summary>
    /// Конфигурация поиска спутниковых продуктов
    /// </summary>
    /// <remarks>
    /// Единая конфигурация границ поиска для всех полей
    /// </remarks>
    public class SatelliteSearchConfig : BaseEntity
    {
        /// <summary>
        /// Геометрия для Sentinel2 API (WKT)
        /// </summary>
        public Polygon? SentinelPolygon { get; set; }

        /// <summary>
        /// Нижний левый угол для Landsat API
        /// </summary>
        public Point? LandsatLeftDown { get; set; }
        
        /// <summary>
        /// Верхний правый угол для Landsat API
        /// </summary>
        public Point? LandsatRightUp { get; set; }
        
        /// <summary>
        /// Активность конфигурации
        /// </summary>
        public bool IsActive { get; set; } = true;
        
        /// <summary>
        /// Количество полей, включенных в расчет
        /// </summary>
        public int FieldsCount { get; set; } = 0;
        
        /// <summary>
        /// Буфер вокруг полей в метрах
        /// </summary>
        public double BufferDistance { get; set; } = 0; // 0 по умолчанию
    }
}
