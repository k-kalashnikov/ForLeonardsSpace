using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Masofa.Common.Models.Satellite.Parse.Landsat.STAC
{
    /// <summary>
    /// Представляет ресурс в объекте STAC
    /// </summary>
    public class StacAsset
    {
        /// <summary>
        /// Заголовок ресурса
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; } = default!;

        /// <summary>
        /// Описание ресурса (опционально)
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        /// <summary>
        /// MIME-тип ресурса
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; } = default!;

        /// <summary>
        /// Роли ресурса
        /// </summary>
        [JsonPropertyName("roles")]
        public List<string> Roles { get; set; } = default!;

        /// <summary>
        /// URL ресурса
        /// </summary>
        [JsonPropertyName("href")]
        public string Href { get; set; } = default!;

        /// <summary>
        /// Альтернативные места хранения ресурса (опционально)
        /// </summary>
        [JsonPropertyName("alternate")]
        public StacAssetAlternate? Alternate { get; set; }

        /// <summary>
        /// Информация о спектральных каналах ресурса (опционально)
        /// </summary>
        [JsonPropertyName("eo:bands")]
        public List<StacEoBand>? EoBands { get; set; }

        /// <summary>
        /// Битовые поля классификации для ресурса (опционально)
        /// </summary>
        [JsonPropertyName("classification:bitfields")]
        public List<StacClassificationBitfield>? ClassificationBitfields { get; set; }
    }
}
