using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Masofa.Common.Models.Satellite.Parse.Landsat.STAC
{
    /// <summary>
    /// Представляет секцию геометрии объекта STAC
    /// </summary>
    public class StacGeometry
    {
        /// <summary>
        /// Тип геометрии, обычно "Polygon" для сцен Landsat
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; } = default!;

        /// <summary>
        /// Координаты геометрии
        /// Для полигона это массив массивов массивов чисел с плавающей точкой (3 уровня вложенности)
        /// Внешний массив представляет полигон
        /// Средний массив представляет кольца полигона (обычно только одно)
        /// Внутренний массив представляет точки кольца, каждая точка - пара чисел [долгота, широта]
        /// </summary>
        [JsonPropertyName("coordinates")]
        public List<List<List<double>>> Coordinates { get; set; } = default!;
    }
}
