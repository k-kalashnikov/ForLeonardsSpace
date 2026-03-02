using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Masofa.Common.Models.Satellite.Parse.Landsat.STAC
{
    /// <summary>
    /// Представляет объект STAC, который является основным контейнером для элемента STAC
    /// </summary>
    public class StacFeature
    {
        /// <summary>
        /// Идентификатор объекта
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = default!;

        /// <summary>
        /// Тип объекта, обычно "Feature"
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; } = default!;

        /// <summary>
        /// Версия STAC
        /// </summary>
        [JsonPropertyName("stac_version")]
        public string StacVersion { get; set; } = default!;

        /// <summary>
        /// Используемые расширения STAC
        /// </summary>
        [JsonPropertyName("stac_extensions")]
        public List<string> StacExtensions { get; set; } = default!;

        /// <summary>
        /// Описание объекта
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; } = default!;

        /// <summary>
        /// Ограничивающий прямоугольник объекта [запад, юг, восток, север]
        /// </summary>
        [JsonPropertyName("bbox")]
        public List<double> BoundingBox { get; set; } = default!;

        /// <summary>
        /// Геометрия объекта
        /// </summary>
        [JsonPropertyName("geometry")]
        public StacGeometry Geometry { get; set; } = default!;

        /// <summary>
        /// Свойства объекта
        /// </summary>
        [JsonPropertyName("properties")]
        public StacProperties Properties { get; set; } = default!;

        /// <summary>
        /// Ресурсы объекта
        /// </summary>
        [JsonPropertyName("assets")]
        public Dictionary<string, StacAsset> Assets { get; set; } = default!;

        /// <summary>
        /// Ссылки объекта
        /// </summary>
        [JsonPropertyName("links")]
        public List<StacLink> Links { get; set; } = default!;

        /// <summary>
        /// Идентификатор коллекции объекта
        /// </summary>
        [JsonPropertyName("collection")]
        public string Collection { get; set; } = default!;
    }
}
