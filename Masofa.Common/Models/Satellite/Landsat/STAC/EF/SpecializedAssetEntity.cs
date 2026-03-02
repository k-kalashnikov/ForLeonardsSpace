using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Satellite.Landsat
{
    /// <summary>
    /// Базовая сущность для специализированных ассетов STAC в базе данных
    /// </summary>
    public abstract class SpecializedAssetEntity : BaseEntity
    {
        /// <summary>
        /// Идентификатор ассета STAC
        /// </summary>
        public Guid StacAssetId { get; set; }

        /// <summary>
        /// Идентификатор объекта STAC
        /// </summary>
        public Guid StacFeatureId { get; set; }

        /// <summary>
        /// Локальный путь к файлу ассета (опционально)
        /// </summary>
        [MaxLength(2000)]
        public string? LocalPath { get; set; }

        /// <summary>
        /// Размер ассета в байтах (опционально)
        /// </summary>
        public long? SizeInBytes { get; set; }

        /// <summary>
        /// Статус обработки ассета (опционально)
        /// </summary>
        [MaxLength(50)]
        public string? ProcessingStatus { get; set; }

        /// <summary>
        /// Дополнительные метаданные ассета, сохраненные в виде JSON (опционально)
        /// </summary>
        [Column(TypeName = "text")]
        public string? AdditionalMetadataJson { get; set; }
    }
}
