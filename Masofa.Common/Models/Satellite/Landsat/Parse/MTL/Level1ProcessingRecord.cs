using System.Text.Json.Serialization;

namespace Masofa.Common.Models.Satellite.Parse.Landsat.MTL
{
    public class Level1ProcessingRecord
    {
        [JsonPropertyName("ORIGIN")]
        public string Origin { get; set; } = default!;

        [JsonPropertyName("DIGITAL_OBJECT_IDENTIFIER")]
        public string DigitalObjectIdentifier { get; set; } = default!;

        [JsonPropertyName("REQUEST_ID")]
        public string RequestId { get; set; } = default!;

        [JsonPropertyName("LANDSAT_SCENE_ID")]
        public string LandsatSceneId { get; set; } = default!;

        [JsonPropertyName("LANDSAT_PRODUCT_ID")]
        public string LandsatProductId { get; set; } = default!;

        [JsonPropertyName("PROCESSING_LEVEL")]
        public string ProcessingLevel { get; set; } = default!;

        [JsonPropertyName("COLLECTION_CATEGORY")]
        public string CollectionCategory { get; set; } = default!;

        [JsonPropertyName("OUTPUT_FORMAT")]
        public string OutputFormat { get; set; } = default!;

        [JsonPropertyName("DATE_PRODUCT_GENERATED")]
        public string DateProductGenerated { get; set; } = default!;

        [JsonPropertyName("PROCESSING_SOFTWARE_VERSION")]
        public string ProcessingSoftwareVersion { get; set; } = default!;

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

        [JsonPropertyName("FILE_NAME_BAND_8")]
        public string FileNameBand8 { get; set; } = default!;

        [JsonPropertyName("FILE_NAME_BAND_9")]
        public string FileNameBand9 { get; set; } = default!;

        [JsonPropertyName("FILE_NAME_BAND_10")]
        public string FileNameBand10 { get; set; } = default!;

        [JsonPropertyName("FILE_NAME_BAND_11")]
        public string FileNameBand11 { get; set; } = default!;

        [JsonPropertyName("FILE_NAME_QUALITY_L1_PIXEL")]
        public string FileNameQualityL1Pixel { get; set; } = default!;

        [JsonPropertyName("FILE_NAME_QUALITY_L1_RADIOMETRIC_SATURATION")]
        public string FileNameQualityL1RadiometricSaturation { get; set; } = default!;

        [JsonPropertyName("FILE_NAME_ANGLE_COEFFICIENT")]
        public string FileNameAngleCoefficient { get; set; } = default!;

        [JsonPropertyName("FILE_NAME_ANGLE_SENSOR_AZIMUTH_BAND_4")]
        public string FileNameAngleSensorAzimuthBand4 { get; set; } = default!;

        [JsonPropertyName("FILE_NAME_ANGLE_SENSOR_ZENITH_BAND_4")]
        public string FileNameAngleSensorZenithBand4 { get; set; } = default!;

        [JsonPropertyName("FILE_NAME_ANGLE_SOLAR_AZIMUTH_BAND_4")]
        public string FileNameAngleSolarAzimuthBand4 { get; set; } = default!;

        [JsonPropertyName("FILE_NAME_ANGLE_SOLAR_ZENITH_BAND_4")]
        public string FileNameAngleSolarZenithBand4 { get; set; } = default!;

        [JsonPropertyName("FILE_NAME_METADATA_ODL")]
        public string FileNameMetadataOdl { get; set; } = default!;

        [JsonPropertyName("FILE_NAME_METADATA_XML")]
        public string FileNameMetadataXml { get; set; } = default!;

        [JsonPropertyName("FILE_NAME_CPF")]
        public string FileNameCpf { get; set; } = default!;

        [JsonPropertyName("FILE_NAME_BPF_OLI")]
        public string FileNameBpfOli { get; set; } = default!;

        [JsonPropertyName("FILE_NAME_BPF_TIRS")]
        public string FileNameBpfTirs { get; set; } = default!;

        [JsonPropertyName("FILE_NAME_RLUT")]
        public string FileNameRlut { get; set; } = default!;

        [JsonPropertyName("DATA_SOURCE_ELEVATION")]
        public string DataSourceElevation { get; set; } = default!;

        [JsonPropertyName("GROUND_CONTROL_POINTS_VERSION")]
        public string GroundControlPointsVersion { get; set; } = default!;

        [JsonPropertyName("GROUND_CONTROL_POINTS_MODEL")]
        public string GroundControlPointsModel { get; set; } = default!;

        [JsonPropertyName("GEOMETRIC_RMSE_MODEL")]
        public string GeometricRmseModel { get; set; } = default!;

        [JsonPropertyName("GEOMETRIC_RMSE_MODEL_Y")]
        public string GeometricRmseModelY { get; set; } = default!;

        [JsonPropertyName("GEOMETRIC_RMSE_MODEL_X")]
        public string GeometricRmseModelX { get; set; } = default!;

        [JsonPropertyName("GROUND_CONTROL_POINTS_VERIFY")]
        public string GroundControlPointsVerify { get; set; } = default!;

        [JsonPropertyName("GEOMETRIC_RMSE_VERIFY")]
        public string GeometricRmseVerify { get; set; } = default!;

        [JsonPropertyName("EPHEMERIS_TYPE")]
        public string EphemerisType { get; set; } = default!;
    }
}
