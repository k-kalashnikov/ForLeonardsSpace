using Masofa.Common.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Satellite.Landsat
{
    [Table("Level1MinMaxPixelValues")]
    [PartitionedTable]
    public class Level1MinMaxPixelValueEntity : BaseEntity
    {
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
        public string QuantizeCalMaxBand8 { get; set; }
        public string QuantizeCalMinBand8 { get; set; }
        public string QuantizeCalMaxBand9 { get; set; }
        public string QuantizeCalMinBand9 { get; set; }
        public string QuantizeCalMaxBand10 { get; set; }
        public string QuantizeCalMinBand10 { get; set; }
        public string QuantizeCalMaxBand11 { get; set; }
        public string QuantizeCalMinBand11 { get; set; }
    }

    public class Level1MinMaxPixelValueEntityHistory : BaseHistoryEntity<Level1MinMaxPixelValueEntity> { }
}
