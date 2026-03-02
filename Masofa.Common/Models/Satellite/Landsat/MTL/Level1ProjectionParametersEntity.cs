using Masofa.Common.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Satellite.Landsat
{
    [Table("Level1ProjectionParameters")]
    [PartitionedTable]
    public class Level1ProjectionParametersEntity : BaseEntity
    {
        public string MapProjection { get; set; }
        public string Datum { get; set; }
        public string Ellipsoid { get; set; }
        public string UtmZone { get; set; }
        public string GridCellSizePanchromatic { get; set; }
        public string GridCellSizeReflective { get; set; }
        public string GridCellSizeThermal { get; set; }
        public string Orientation { get; set; }
        public string ResamplingOption { get; set; }
    }

    public class Level1ProjectionParametersEntityHistory : BaseHistoryEntity<Level1ProjectionParametersEntity> { }
}
