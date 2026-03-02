using Masofa.Common.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Satellite.Landsat
{
    [Table("ProjectionAttributes")]
    [PartitionedTable]
    public class ProjectionAttributesEntity : BaseEntity
    {
        public string MapProjection { get; set; }
        public string Datum { get; set; }
        public string Ellipsoid { get; set; }
        public string UtmZone { get; set; }
        public string GridCellSizeReflective { get; set; }
        public string GridCellSizeThermal { get; set; }
        public string ReflectiveLines { get; set; }
        public string ReflectiveSamples { get; set; }
        public string ThermalLines { get; set; }
        public string ThermalSamples { get; set; }
        public string Orientation { get; set; }
        public string CornerUlLatProduct { get; set; }
        public string CornerUlLonProduct { get; set; }
        public string CornerUrLatProduct { get; set; }
        public string CornerUrLonProduct { get; set; }
        public string CornerLlLatProduct { get; set; }
        public string CornerLlLonProduct { get; set; }
        public string CornerLrLatProduct { get; set; }
        public string CornerLrLonProduct { get; set; }
        public string CornerUlProjectionXProduct { get; set; }
        public string CornerUlProjectionYProduct { get; set; }
        public string CornerUrProjectionXProduct { get; set; }
        public string CornerUrProjectionYProduct { get; set; }
        public string CornerLlProjectionXProduct { get; set; }
        public string CornerLlProjectionYProduct { get; set; }
        public string CornerLrProjectionXProduct { get; set; }
        public string CornerLrProjectionYProduct { get; set; }
    }

    public class ProjectionAttributesEntityHistory : BaseHistoryEntity<ProjectionAttributesEntity> { }
}
