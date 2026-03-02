using Masofa.Common.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Satellite.Sentinel
{
    /// <summary>
    /// Сущность для объединения всех типов данных Sentinel2
    /// </summary>
    [Table("Sentinel2Products")]
    [PartitionedTable]
    public class Sentinel2ProductEntity : BaseEntity
    {
        /// <summary>
        /// Идентификатор продукта спутника
        /// </summary>
        public string SatellateProductId { get; set; } = default!;

        /// <summary>
        /// Идентификатор метаданных SentinelProductQualityMetadataId
        /// </summary>
        public Guid? SentinelProductQualityMetadataId { get; set; }

        /// <summary>
        /// Идентификатор метаданных SentinelL1CTileMetadataId
        /// </summary>
        public Guid? SentinelL1CTileMetadataId { get; set; }

        /// <summary>
        /// Идентификатор метаданных SentinelL1CProductMetadataId
        /// </summary>
        public Guid? SentinelL1CProductMetadataId { get; set; }

        /// <summary>
        /// Идентификатор метаданных SentinelInspireMetadataId
        /// </summary>
        public Guid? SentinelInspireMetadataId { get; set; }

        public DateTime? OriginDate { get; set; }
    }

    public class Sentinel2ProductEntityHistory : BaseHistoryEntity<Sentinel2ProductEntity> { }
}
