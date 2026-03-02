using Masofa.Common.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Satellite.Landsat
{
    [Table("Level1MinMaxRadiances")]
    [PartitionedTable]
    public class Level1MinMaxRadianceEntity : BaseEntity
    {
        public string RadianceMaximumBand1 { get; set; }
        public string RadianceMinimumBand1 { get; set; }
        public string RadianceMaximumBand2 { get; set; }
        public string RadianceMinimumBand2 { get; set; }
        public string RadianceMaximumBand3 { get; set; }
        public string RadianceMinimumBand3 { get; set; }
        public string RadianceMaximumBand4 { get; set; }
        public string RadianceMinimumBand4 { get; set; }
        public string RadianceMaximumBand5 { get; set; }
        public string RadianceMinimumBand5 { get; set; }
        public string RadianceMaximumBand6 { get; set; }
        public string RadianceMinimumBand6 { get; set; }
        public string RadianceMaximumBand7 { get; set; }
        public string RadianceMinimumBand7 { get; set; }
        public string RadianceMaximumBand8 { get; set; }
        public string RadianceMinimumBand8 { get; set; }
        public string RadianceMaximumBand9 { get; set; }
        public string RadianceMinimumBand9 { get; set; }
        public string RadianceMaximumBand10 { get; set; }
        public string RadianceMinimumBand10 { get; set; }
        public string RadianceMaximumBand11 { get; set; }
        public string RadianceMinimumBand11 { get; set; }
    }

    public class Level1MinMaxRadianceEntityHistory : BaseHistoryEntity<Level1MinMaxRadianceEntity> { }
}
