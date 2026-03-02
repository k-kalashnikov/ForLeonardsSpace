using Masofa.Common.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Satellite.Landsat
{
    [Table("Level1ThermalConstants")]
    [PartitionedTable]
    public class Level1ThermalConstantsEntity : BaseEntity
    {
        public string K1ConstantBand10 { get; set; }
        public string K2ConstantBand10 { get; set; }
        public string K1ConstantBand11 { get; set; }
        public string K2ConstantBand11 { get; set; }
    }

    public class Level1ThermalConstantsEntityHistory : BaseHistoryEntity<Level1ThermalConstantsEntity> { }
}
