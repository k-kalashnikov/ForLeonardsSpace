using Masofa.Common.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Satellite.Landsat
{
    [Table("Level2SurfaceTemperatureParameters")]
    [PartitionedTable]
    public class Level2SurfaceTemperatureParametersEntity : BaseEntity
    {
        public string TemperatureMaximumBandStB10 { get; set; }
        public string TemperatureMinimumBandStB10 { get; set; }
        public string QuantizeCalMaximumBandStB10 { get; set; }
        public string QuantizeCalMinimumBandStB10 { get; set; }
        public string TemperatureMultBandStB10 { get; set; }
        public string TemperatureAddBandStB10 { get; set; }
    }

    public class Level2SurfaceTemperatureParametersEntityHistory : BaseHistoryEntity<Level2SurfaceTemperatureParametersEntity> { }
}
