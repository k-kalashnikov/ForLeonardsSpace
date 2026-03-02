namespace Masofa.Common.Models.Satellite
{
    /// <summary>
    /// Метадата S2 продукта
    /// </summary>
    /// <remarks>
    /// Справочник метадаты S2 продуктов
    /// </remarks>
    public class Sentinel2ProductMetadata : BaseEntity
    {

        /// <summary>Имя</summary>
        public string? Name { get; set; }

        /// <summary>Тип содержимого</summary>
        public string? ContentType { get; set; }

        /// <summary>Длина содержимого</summary>
        public long? ContentLength { get; set; }

        /// <summary>Дата источника</summary>
        public DateTime? OriginDate { get; set; }

        /// <summary>Дата публикации</summary>
        public DateTime? PublicationDate { get; set; }

        /// <summary>Дата изменения</summary>
        public DateTime? ModificationDate { get; set; }

        /// <summary>Онлайн</summary>
        public bool Online { get; set; }

        /// <summary>Дата завершения</summary>
        public DateTime? EvictionDate { get; set; }

        /// <summary>Путь к продукту на Sentinel сервере</summary>
        public string? S3Path { get; set; }

        /// <summary>Покрытие</summary>
        public string? Footprint { get; set; }

        /// <summary>Контродльная сумма MD5</summary>
        public string? ChecksumMd5 { get; set; }

        /// <summary>Дата начала съемки</summary>
        public DateTime ContentDateStart { get; set; }

        /// <summary>Дата завершения съемки</summary>
        public DateTime ContentDateEnd { get; set; }

        /// <summary>
        /// Ссылка на модель Product
        /// </summary>
        public string? ProductId { get; set; }
    }

    public class Sentinel2ProductMetadataHistory : BaseHistoryEntity<Sentinel2ProductMetadata> { }
}
