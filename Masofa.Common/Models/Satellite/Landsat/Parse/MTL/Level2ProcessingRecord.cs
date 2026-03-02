using System.Text.Json.Serialization;

namespace Masofa.Common.Models.Satellite.Parse.Landsat.MTL
{
    public class Level2ProcessingRecord
    {
        [JsonPropertyName("ORIGIN")]
        public string Origin { get; set; } = default!;

        [JsonPropertyName("DIGITAL_OBJECT_IDENTIFIER")]
        public string DigitalObjectIdentifier { get; set; } = default!;

        [JsonPropertyName("REQUEST_ID")]
        public string RequestId { get; set; } = default!;

        [JsonPropertyName("LANDSAT_PRODUCT_ID")]
        public string LandsatProductId { get; set; } = default!;

        [JsonPropertyName("PROCESSING_LEVEL")]
        public string ProcessingLevel { get; set; } = default!;

        [JsonPropertyName("OUTPUT_FORMAT")]
        public string OutputFormat { get; set; } = default!;

        [JsonPropertyName("DATE_PRODUCT_GENERATED")]
        public string DateProductGenerated { get; set; } = default!;

        [JsonPropertyName("PROCESSING_SOFTWARE_VERSION")]
        public string ProcessingSoftwareVersion { get; set; } = default!;

        [JsonPropertyName("ALGORITHM_SOURCE_SURFACE_REFLECTANCE")]
        public string AlgorithmSourceSurfaceReflectance { get; set; } = default!;

        [JsonPropertyName("DATA_SOURCE_OZONE")]
        public string DataSourceOzone { get; set; } = default!;

        [JsonPropertyName("DATA_SOURCE_PRESSURE")]
        public string DataSourcePressure { get; set; } = default!;

        [JsonPropertyName("DATA_SOURCE_WATER_VAPOR")]
        public string DataSourceWaterVapor { get; set; } = default!;

        [JsonPropertyName("DATA_SOURCE_AIR_TEMPERATURE")]
        public string DataSourceAirTemperature { get; set; } = default!;

        [JsonPropertyName("ALGORITHM_SOURCE_SURFACE_TEMPERATURE")]
        public string AlgorithmSourceSurfaceTemperature { get; set; } = default!;

        [JsonPropertyName("DATA_SOURCE_REANALYSIS")]
        public string DataSourceReanalysis { get; set; } = default!;
    }
}
