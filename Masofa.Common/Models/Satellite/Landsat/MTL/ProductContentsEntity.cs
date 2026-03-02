using Masofa.Common.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Satellite.Landsat
{
    [PartitionedTable]
    [Table("ProductContents")]
    public class ProductContentsEntity : BaseEntity
    {
        public string Origin { get; set; }
        public string DigitalObjectIdentifier { get; set; }
        public string LandsatProductId { get; set; }
        public string ProcessingLevel { get; set; }
        public string CollectionNumber { get; set; }
        public string CollectionCategory { get; set; }
        public string OutputFormat { get; set; }
        public string FileNameBand1 { get; set; }
        public string FileNameBand2 { get; set; }
        public string FileNameBand3 { get; set; }
        public string FileNameBand4 { get; set; }
        public string FileNameBand5 { get; set; }
        public string FileNameBand6 { get; set; }
        public string FileNameBand7 { get; set; }
        public string FileNameBandStB10 { get; set; }
        public string FileNameThermalRadiance { get; set; }
        public string FileNameUpwellRadiance { get; set; }
        public string FileNameDownwellRadiance { get; set; }
        public string FileNameAtmosphericTransmittance { get; set; }
        public string FileNameEmissivity { get; set; }
        public string FileNameEmissivityStdev { get; set; }
        public string FileNameCloudDistance { get; set; }
        public string FileNameQualityL2Aerosol { get; set; }
        public string FileNameQualityL2SurfaceTemperature { get; set; }
        public string FileNameQualityL1Pixel { get; set; }
        public string FileNameQualityL1RadiometricSaturation { get; set; }
        public string FileNameAngleCoefficient { get; set; }
        public string FileNameMetadataOdl { get; set; }
        public string FileNameMetadataXml { get; set; }
        public string DataTypeBand1 { get; set; }
        public string DataTypeBand2 { get; set; }
        public string DataTypeBand3 { get; set; }
        public string DataTypeBand4 { get; set; }
        public string DataTypeBand5 { get; set; }
        public string DataTypeBand6 { get; set; }
        public string DataTypeBand7 { get; set; }
        public string DataTypeBandStB10 { get; set; }
        public string DataTypeThermalRadiance { get; set; }
        public string DataTypeUpwellRadiance { get; set; }
        public string DataTypeDownwellRadiance { get; set; }
        public string DataTypeAtmosphericTransmittance { get; set; }
        public string DataTypeEmissivity { get; set; }
        public string DataTypeEmissivityStdev { get; set; }
        public string DataTypeCloudDistance { get; set; }
        public string DataTypeQualityL2Aerosol { get; set; }
        public string DataTypeQualityL2SurfaceTemperature { get; set; }
        public string DataTypeQualityL1Pixel { get; set; }
        public string DataTypeQualityL1RadiometricSaturation { get; set; }
    }

    public class ProductContentsEntityHistory : BaseHistoryEntity<ProductContentsEntity> { }
}
