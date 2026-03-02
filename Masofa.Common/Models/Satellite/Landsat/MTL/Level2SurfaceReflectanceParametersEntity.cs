using Masofa.Common.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Satellite.Landsat
{
    [Table("Level2SurfaceReflectanceParameters")]
    [PartitionedTable]
    public class Level2SurfaceReflectanceParametersEntity : BaseEntity
    {
        public string ReflectanceMaximumBand1 { get; set; }
        public string ReflectanceMinimumBand1 { get; set; }
        public string ReflectanceMaximumBand2 { get; set; }
        public string ReflectanceMinimumBand2 { get; set; }
        public string ReflectanceMaximumBand3 { get; set; }
        public string ReflectanceMinimumBand3 { get; set; }
        public string ReflectanceMaximumBand4 { get; set; }
        public string ReflectanceMinimumBand4 { get; set; }
        public string ReflectanceMaximumBand5 { get; set; }
        public string ReflectanceMinimumBand5 { get; set; }
        public string ReflectanceMaximumBand6 { get; set; }
        public string ReflectanceMinimumBand6 { get; set; }
        public string ReflectanceMaximumBand7 { get; set; }
        public string ReflectanceMinimumBand7 { get; set; }
        public string QuantizeCalMaxBand1 { get; set; }
        public string QuantizeCalMinBand1 { get; set; }
        public string QuantizeCalMaxBand2 { get; set; }
        public string QuantizeCalMinBand2 { get; set; }
        public string QuantizeCalMaxBand3 { get; set; }
        public string QuantizeCalMinBand3 { get; set; }
        public string QuantizeCalMaxBand4 { get; set; }
        public string QuantizeCalMinBand4 { get; set; }
        public string QuantizeCalMaxBand5 { get; set; }
        public string QuantizeCalMinBand5 { get; set; }
        public string QuantizeCalMaxBand6 { get; set; }
        public string QuantizeCalMinBand6 { get; set; }
        public string QuantizeCalMaxBand7 { get; set; }
        public string QuantizeCalMinBand7 { get; set; }
        public string ReflectanceMultBand1 { get; set; }
        public string ReflectanceMultBand2 { get; set; }
        public string ReflectanceMultBand3 { get; set; }
        public string ReflectanceMultBand4 { get; set; }
        public string ReflectanceMultBand5 { get; set; }
        public string ReflectanceMultBand6 { get; set; }
        public string ReflectanceMultBand7 { get; set; }
        public string ReflectanceAddBand1 { get; set; }
        public string ReflectanceAddBand2 { get; set; }
        public string ReflectanceAddBand3 { get; set; }
        public string ReflectanceAddBand4 { get; set; }
        public string ReflectanceAddBand5 { get; set; }
        public string ReflectanceAddBand6 { get; set; }
        public string ReflectanceAddBand7 { get; set; }
    }

    public class Level2SurfaceReflectanceParametersEntityHistory : BaseHistoryEntity<Level2SurfaceReflectanceParametersEntity> { }
}
