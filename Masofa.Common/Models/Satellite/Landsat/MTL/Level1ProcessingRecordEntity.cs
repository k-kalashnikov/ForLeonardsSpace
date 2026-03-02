using Masofa.Common.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Satellite.Landsat
{
    [PartitionedTable]
    [Table("Level1ProcessingRecords")]
    public class Level1ProcessingRecordEntity : BaseEntity
    {
        public string Origin { get; set; }
        public string DigitalObjectIdentifier { get; set; }
        public string RequestId { get; set; }
        public string LandsatSceneId { get; set; }
        public string LandsatProductId { get; set; }
        public string ProcessingLevel { get; set; }
        public string CollectionCategory { get; set; }
        public string OutputFormat { get; set; }
        public string DateProductGenerated { get; set; }
        public string ProcessingSoftwareVersion { get; set; }
        public string FileNameBand1 { get; set; }
        public string FileNameBand2 { get; set; }
        public string FileNameBand3 { get; set; }
        public string FileNameBand4 { get; set; }
        public string FileNameBand5 { get; set; }
        public string FileNameBand6 { get; set; }
        public string FileNameBand7 { get; set; }
        public string FileNameBand8 { get; set; }
        public string FileNameBand9 { get; set; }
        public string FileNameBand10 { get; set; }
        public string FileNameBand11 { get; set; }
        public string FileNameQualityL1Pixel { get; set; }
        public string FileNameQualityL1RadiometricSaturation { get; set; }
        public string FileNameAngleCoefficient { get; set; }
        public string FileNameAngleSensorAzimuthBand4 { get; set; }
        public string FileNameAngleSensorZenithBand4 { get; set; }
        public string FileNameAngleSolarAzimuthBand4 { get; set; }
        public string FileNameAngleSolarZenithBand4 { get; set; }
        public string FileNameMetadataOdl { get; set; }
        public string FileNameMetadataXml { get; set; }
        public string FileNameCpf { get; set; }
        public string FileNameBpfOli { get; set; }
        public string FileNameBpfTirs { get; set; }
        public string FileNameRlut { get; set; }
        public string DataSourceElevation { get; set; }
        public string GroundControlPointsVersion { get; set; }
        public string GroundControlPointsModel { get; set; }
        public string GeometricRmseModel { get; set; }
        public string GeometricRmseModelY { get; set; }
        public string GeometricRmseModelX { get; set; }
        public string GroundControlPointsVerify { get; set; }
        public string GeometricRmseVerify { get; set; }
        public string EphemerisType { get; set; }
    }

    public class Level1ProcessingRecordEntityHistory : BaseHistoryEntity<Level1ProcessingRecordEntity> { }
}
