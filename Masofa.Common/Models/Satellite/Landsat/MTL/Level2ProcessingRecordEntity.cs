using Masofa.Common.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Satellite.Landsat
{
    [Table("Level2ProcessingRecords")]
    [PartitionedTable]
    public class Level2ProcessingRecordEntity : BaseEntity
    {
        public string Origin { get; set; }
        public string DigitalObjectIdentifier { get; set; }
        public string RequestId { get; set; }
        public string LandsatProductId { get; set; }
        public string ProcessingLevel { get; set; }
        public string OutputFormat { get; set; }
        public string DateProductGenerated { get; set; }
        public string ProcessingSoftwareVersion { get; set; }
        public string AlgorithmSourceSurfaceReflectance { get; set; }
        public string DataSourceOzone { get; set; }
        public string DataSourcePressure { get; set; }
        public string DataSourceWaterVapor { get; set; }
        public string DataSourceAirTemperature { get; set; }
        public string AlgorithmSourceSurfaceTemperature { get; set; }
        public string DataSourceReanalysis { get; set; }
    }

    public class Level2ProcessingRecordEntityHistory : BaseHistoryEntity<Level2ProcessingRecordEntity> { }
}
