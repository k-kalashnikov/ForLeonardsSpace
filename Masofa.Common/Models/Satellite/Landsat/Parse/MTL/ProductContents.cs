using System.Text.Json.Serialization;

namespace Masofa.Common.Models.Satellite.Parse.Landsat.MTL
{
    public class ProductContents
    {
        [JsonPropertyName("ORIGIN")]
        public string Origin { get; set; } = default!;

        [JsonPropertyName("DIGITAL_OBJECT_IDENTIFIER")]
        public string DigitalObjectIdentifier { get; set; } = default!;

        [JsonPropertyName("LANDSAT_PRODUCT_ID")]
        public string LandsatProductId { get; set; } = default!;

        [JsonPropertyName("PROCESSING_LEVEL")]
        public string ProcessingLevel { get; set; } = default!;

        [JsonPropertyName("COLLECTION_NUMBER")]
        public string CollectionNumber { get; set; } = default!;

        [JsonPropertyName("COLLECTION_CATEGORY")]
        public string CollectionCategory { get; set; } = default!;

        [JsonPropertyName("OUTPUT_FORMAT")]
        public string OutputFormat { get; set; } = default!;

        [JsonPropertyName("FILE_NAME_BAND_1")]
        public string FileNameBand1 { get; set; } = default!;

        [JsonPropertyName("FILE_NAME_BAND_2")]
        public string FileNameBand2 { get; set; } = default!;

        [JsonPropertyName("FILE_NAME_BAND_3")]
        public string FileNameBand3 { get; set; } = default!;

        [JsonPropertyName("FILE_NAME_BAND_4")]
        public string FileNameBand4 { get; set; } = default!;

        [JsonPropertyName("FILE_NAME_BAND_5")]
        public string FileNameBand5 { get; set; } = default!;

        [JsonPropertyName("FILE_NAME_BAND_6")]
        public string FileNameBand6 { get; set; } = default!;

        [JsonPropertyName("FILE_NAME_BAND_7")]
        public string FileNameBand7 { get; set; } = default!;

        [JsonPropertyName("FILE_NAME_BAND_ST_B10")]
        public string FileNameBandStB10 { get; set; } = default!;

        [JsonPropertyName("FILE_NAME_THERMAL_RADIANCE")]
        public string FileNameThermalRadiance { get; set; } = default!;

        [JsonPropertyName("FILE_NAME_UPWELL_RADIANCE")]
        public string FileNameUpwellRadiance { get; set; } = default!;

        [JsonPropertyName("FILE_NAME_DOWNWELL_RADIANCE")]
        public string FileNameDownwellRadiance { get; set; } = default!;

        [JsonPropertyName("FILE_NAME_ATMOSPHERIC_TRANSMITTANCE")]
        public string FileNameAtmosphericTransmittance { get; set; } = default!;

        [JsonPropertyName("FILE_NAME_EMISSIVITY")]
        public string FileNameEmissivity { get; set; } = default!;

        [JsonPropertyName("FILE_NAME_EMISSIVITY_STDEV")]
        public string FileNameEmissivityStdev { get; set; } = default!;

        [JsonPropertyName("FILE_NAME_CLOUD_DISTANCE")]
        public string FileNameCloudDistance { get; set; } = default!;

        [JsonPropertyName("FILE_NAME_QUALITY_L2_AEROSOL")]
        public string FileNameQualityL2Aerosol { get; set; } = default!;

        [JsonPropertyName("FILE_NAME_QUALITY_L2_SURFACE_TEMPERATURE")]
        public string FileNameQualityL2SurfaceTemperature { get; set; } = default!;

        [JsonPropertyName("FILE_NAME_QUALITY_L1_PIXEL")]
        public string FileNameQualityL1Pixel { get; set; } = default!;

        [JsonPropertyName("FILE_NAME_QUALITY_L1_RADIOMETRIC_SATURATION")]
        public string FileNameQualityL1RadiometricSaturation { get; set; } = default!;

        [JsonPropertyName("FILE_NAME_ANGLE_COEFFICIENT")]
        public string FileNameAngleCoefficient { get; set; } = default!;

        [JsonPropertyName("FILE_NAME_METADATA_ODL")]
        public string FileNameMetadataOdl { get; set; } = default!;

        [JsonPropertyName("FILE_NAME_METADATA_XML")]
        public string FileNameMetadataXml { get; set; } = default!;

        [JsonPropertyName("DATA_TYPE_BAND_1")]
        public string DataTypeBand1 { get; set; } = default!;

        [JsonPropertyName("DATA_TYPE_BAND_2")]
        public string DataTypeBand2 { get; set; } = default!;

        [JsonPropertyName("DATA_TYPE_BAND_3")]
        public string DataTypeBand3 { get; set; } = default!;

        [JsonPropertyName("DATA_TYPE_BAND_4")]
        public string DataTypeBand4 { get; set; } = default!;

        [JsonPropertyName("DATA_TYPE_BAND_5")]
        public string DataTypeBand5 { get; set; } = default!;

        [JsonPropertyName("DATA_TYPE_BAND_6")]
        public string DataTypeBand6 { get; set; } = default!;

        [JsonPropertyName("DATA_TYPE_BAND_7")]
        public string DataTypeBand7 { get; set; } = default!;

        [JsonPropertyName("DATA_TYPE_BAND_ST_B10")]
        public string DataTypeBandStB10 { get; set; } = default!;

        [JsonPropertyName("DATA_TYPE_THERMAL_RADIANCE")]
        public string DataTypeThermalRadiance { get; set; } = default!;

        [JsonPropertyName("DATA_TYPE_UPWELL_RADIANCE")]
        public string DataTypeUpwellRadiance { get; set; } = default!;

        [JsonPropertyName("DATA_TYPE_DOWNWELL_RADIANCE")]
        public string DataTypeDownwellRadiance { get; set; } = default!;

        [JsonPropertyName("DATA_TYPE_ATMOSPHERIC_TRANSMITTANCE")]
        public string DataTypeAtmosphericTransmittance { get; set; } = default!;

        [JsonPropertyName("DATA_TYPE_EMISSIVITY")]
        public string DataTypeEmissivity { get; set; } = default!;

        [JsonPropertyName("DATA_TYPE_EMISSIVITY_STDEV")]
        public string DataTypeEmissivityStdev { get; set; } = default!;

        [JsonPropertyName("DATA_TYPE_CLOUD_DISTANCE")]
        public string DataTypeCloudDistance { get; set; } = default!;

        [JsonPropertyName("DATA_TYPE_QUALITY_L2_AEROSOL")]
        public string DataTypeQualityL2Aerosol { get; set; } = default!;

        [JsonPropertyName("DATA_TYPE_QUALITY_L2_SURFACE_TEMPERATURE")]
        public string DataTypeQualityL2SurfaceTemperature { get; set; } = default!;

        [JsonPropertyName("DATA_TYPE_QUALITY_L1_PIXEL")]
        public string DataTypeQualityL1Pixel { get; set; } = default!;

        [JsonPropertyName("DATA_TYPE_QUALITY_L1_RADIOMETRIC_SATURATION")]
        public string DataTypeQualityL1RadiometricSaturation { get; set; } = default!;
    }
}