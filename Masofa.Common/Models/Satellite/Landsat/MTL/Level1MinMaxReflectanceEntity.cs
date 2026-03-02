using Masofa.Common.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Satellite.Landsat
{
    [Table("Level1MinMaxReflectances")]
    [PartitionedTable]
    public class Level1MinMaxReflectanceEntity : BaseEntity
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
        public string ReflectanceMaximumBand8 { get; set; }
        public string ReflectanceMinimumBand8 { get; set; }
        public string ReflectanceMaximumBand9 { get; set; }
        public string ReflectanceMinimumBand9 { get; set; }
    }

    public class Level1MinMaxReflectanceEntityHistory : BaseHistoryEntity<Level1MinMaxReflectanceEntity> { }
}
