using System.Text.Json.Serialization;

namespace Masofa.Common.Models.Satellite.Parse.Landsat.STAC
{
    /// <summary>
    /// Представляет альтернативное место хранения ресурса в объекте STAC
    /// </summary>
    public class StacAssetAlternate
    {
        /// <summary>
        /// Информация о хранилище S3
        /// </summary>
        [JsonPropertyName("s3")]
        public StacAssetAlternateS3 S3 { get; set; } = default!;
    }

    /// <summary>
    /// Представляет информацию о хранилище S3 для ресурса в объекте STAC
    /// </summary>
    public class StacAssetAlternateS3
    {
        /// <summary>
        /// Платформа хранения (например, "AWS")
        /// </summary>
        [JsonPropertyName("storage:platform")]
        public string StoragePlatform { get; set; } = default!;

        /// <summary>
        /// Оплачивает ли запрашивающий доступ
        /// </summary>
        [JsonPropertyName("storage:requester_pays")]
        public bool StorageRequesterPays { get; set; }

        /// <summary>
        /// S3 URL ресурса
        /// </summary>
        [JsonPropertyName("href")]
        public string Href { get; set; } = default!;
    }
}
