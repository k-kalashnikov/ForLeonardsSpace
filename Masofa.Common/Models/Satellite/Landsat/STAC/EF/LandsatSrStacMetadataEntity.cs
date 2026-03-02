using Masofa.Common.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Satellite.Landsat
{
    /// <summary>
    /// Сущность для хранения ссылок на данные Surface Reflectance (SR) STAC
    /// </summary>
    [PartitionedTable]
    [Table("LandsatSrStacMetadatas")]
    public class LandsatSrStacMetadataEntity : BaseEntity
    {
        /// <summary>
        /// Идентификатор продукта спутника
        /// </summary>
        public string SatellateProductId { get; set; } = default!;

        /// <summary>
        /// Идентификатор основной сущности STAC Feature
        /// </summary>
        public Guid? StacFeatureId { get; set; }
    }

    public class LandsatSrStacMetadataEntityHistory : BaseHistoryEntity<LandsatSrStacMetadataEntity> { }
}
