using Masofa.Common.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Satellite.Landsat
{
    [Table("Level1RadiometricRescalings")]
    [PartitionedTable]
    public class Level1RadiometricRescalingEntity : BaseEntity
    {
        public string RadianceMultBand1 { get; set; }
        public string RadianceMultBand2 { get; set; }
        public string RadianceMultBand3 { get; set; }
        public string RadianceMultBand4 { get; set; }
        public string RadianceMultBand5 { get; set; }
        public string RadianceMultBand6 { get; set; }
        public string RadianceMultBand7 { get; set; }
        public string RadianceMultBand8 { get; set; }
        public string RadianceMultBand9 { get; set; }
        public string RadianceMultBand10 { get; set; }
        public string RadianceMultBand11 { get; set; }
        public string RadianceAddBand1 { get; set; }
        public string RadianceAddBand2 { get; set; }
        public string RadianceAddBand3 { get; set; }
        public string RadianceAddBand4 { get; set; }
        public string RadianceAddBand5 { get; set; }
        public string RadianceAddBand6 { get; set; }
        public string RadianceAddBand7 { get; set; }
        public string RadianceAddBand8 { get; set; }
        public string RadianceAddBand9 { get; set; }
        public string RadianceAddBand10 { get; set; }
        public string RadianceAddBand11 { get; set; }
        public string ReflectanceMultBand1 { get; set; }
        public string ReflectanceMultBand2 { get; set; }
        public string ReflectanceMultBand3 { get; set; }
        public string ReflectanceMultBand4 { get; set; }
        public string ReflectanceMultBand5 { get; set; }
        public string ReflectanceMultBand6 { get; set; }
        public string ReflectanceMultBand7 { get; set; }
        public string ReflectanceMultBand8 { get; set; }
        public string ReflectanceMultBand9 { get; set; }
        public string ReflectanceAddBand1 { get; set; }
        public string ReflectanceAddBand2 { get; set; }
        public string ReflectanceAddBand3 { get; set; }
        public string ReflectanceAddBand4 { get; set; }
        public string ReflectanceAddBand5 { get; set; }
        public string ReflectanceAddBand6 { get; set; }
        public string ReflectanceAddBand7 { get; set; }
        public string ReflectanceAddBand8 { get; set; }
        public string ReflectanceAddBand9 { get; set; }
    }

    public class Level1RadiometricRescalingEntityHistory : BaseHistoryEntity<Level1RadiometricRescalingEntity> { }
}
