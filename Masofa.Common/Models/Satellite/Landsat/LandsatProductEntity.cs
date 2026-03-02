using Masofa.Common.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Satellite.Landsat
{
    /// <summary>
    /// Сущность для объединения всех типов данных Landsat (MTL, SR, ST)
    /// </summary>
    [Table("LandsatProducts")]
    [PartitionedTable]
    public class LandsatProductEntity : BaseEntity
    {
        /// <summary>
        /// Идентификатор продукта спутника
        /// </summary>
        public string SatellateProductId { get; set; } = default!;

        /// <summary>
        /// Идентификатор метаданных MTL
        /// </summary>
        public Guid? LandsatSourceMetadataId { get; set; }

        /// <summary>
        /// Идентификатор метаданных SR STAC
        /// </summary>
        public Guid? LandsatSrStacMetadataId { get; set; }

        /// <summary>
        /// Идентификатор метаданных ST STAC
        /// </summary>
        public Guid? LandsatStStacMetadataId { get; set; }
    }

    public class LandsatProductEntityHistory : BaseHistoryEntity<LandsatProductEntity> { }
}
