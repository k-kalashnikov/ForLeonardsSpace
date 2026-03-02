using System.Text.Json.Serialization;

namespace Masofa.Common.Models.Satellite.Parse.Landsat.STAC
{
    /// <summary>
    /// Представляет ссылку в объекте STAC
    /// </summary>
    public class StacLink
    {
        /// <summary>
        /// Отношение ссылки (например, "root", "parent", "collection", "self")
        /// </summary>
        [JsonPropertyName("rel")]
        public string Rel { get; set; } = default!;

        /// <summary>
        /// URL ссылки
        /// </summary>
        [JsonPropertyName("href")]
        public string Href { get; set; } = default!;
    }
}
