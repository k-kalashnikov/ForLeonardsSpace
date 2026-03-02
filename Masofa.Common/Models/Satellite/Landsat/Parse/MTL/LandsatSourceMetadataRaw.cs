using System.Text.Json.Serialization;

namespace Masofa.Common.Models.Satellite.Parse.Landsat.MTL
{
    /// <summary>
    /// Модель для десериализации полного MTL JSON файла Landsat
    /// </summary>
    public class LandsatSourceMetadataRaw
    {
        /// <summary>
        /// Основная обертка MTL файла
        /// </summary>
        [JsonPropertyName("LANDSAT_METADATA_FILE")]
        public LandsatMetadataFile? LandsatMetadataFile { get; set; }

        // ID полей для связи с существующими записями (если есть)
        public Guid? ProductContentsId { get; set; }
        public Guid? ImageAttributesId { get; set; }
        public Guid? ProjectionAttributesId { get; set; }
        public Guid? Level2ProcessingRecordId { get; set; }
        public Guid? Level2SurfaceReflectanceParametersId { get; set; }
        public Guid? Level2SurfaceTemperatureParametersId { get; set; }
        public Guid? Level1ProcessingRecordId { get; set; }
        public Guid? Level1MinMaxRadianceId { get; set; }
        public Guid? Level1MinMaxReflectanceId { get; set; }
        public Guid? Level1MinMaxPixelValueId { get; set; }
        public Guid? Level1RadiometricRescalingId { get; set; }
        public Guid? Level1ThermalConstantsId { get; set; }
        public Guid? Level1ProjectionParametersId { get; set; }

        /// <summary>
        /// Удобный геттер для получения SatellateProductId
        /// </summary>
        public string SatellateProductId => LandsatMetadataFile?.ProductContents?.LandsatProductId ?? string.Empty;
    }

    /// <summary>
    /// Основная структура MTL файла
    /// </summary>
    public class LandsatMetadataFile
    {
        /// <summary>
        /// Секция Product Contents
        /// </summary>
        [JsonPropertyName("PRODUCT_CONTENTS")]
        public ProductContents? ProductContents { get; set; }

        /// <summary>
        /// Секция Image Attributes
        /// </summary>
        [JsonPropertyName("IMAGE_ATTRIBUTES")]
        public ImageAttributes? ImageAttributes { get; set; }

        /// <summary>
        /// Секция Projection Attributes
        /// </summary>
        [JsonPropertyName("PROJECTION_ATTRIBUTES")]
        public ProjectionAttributes? ProjectionAttributes { get; set; }

        /// <summary>
        /// Секция Level 2 Processing Record
        /// </summary>
        [JsonPropertyName("LEVEL2_PROCESSING_RECORD")]
        public Level2ProcessingRecord? Level2ProcessingRecord { get; set; }

        /// <summary>
        /// Секция Level 2 Surface Reflectance Parameters
        /// </summary>
        [JsonPropertyName("LEVEL2_SURFACE_REFLECTANCE_PARAMETERS")]
        public Level2SurfaceReflectanceParameters? Level2SurfaceReflectanceParameters { get; set; }

        /// <summary>
        /// Секция Level 2 Surface Temperature Parameters
        /// </summary>
        [JsonPropertyName("LEVEL2_SURFACE_TEMPERATURE_PARAMETERS")]
        public Level2SurfaceTemperatureParameters? Level2SurfaceTemperatureParameters { get; set; }

        /// <summary>
        /// Секция Level 1 Processing Record
        /// </summary>
        [JsonPropertyName("LEVEL1_PROCESSING_RECORD")]
        public Level1ProcessingRecord? Level1ProcessingRecord { get; set; }

        /// <summary>
        /// Секция Level 1 Min Max Radiance
        /// </summary>
        [JsonPropertyName("LEVEL1_MIN_MAX_RADIANCE")]
        public Level1MinMaxRadiance? Level1MinMaxRadiance { get; set; }

        /// <summary>
        /// Секция Level 1 Min Max Reflectance
        /// </summary>
        [JsonPropertyName("LEVEL1_MIN_MAX_REFLECTANCE")]
        public Level1MinMaxReflectance? Level1MinMaxReflectance { get; set; }

        /// <summary>
        /// Секция Level 1 Min Max Pixel Value
        /// </summary>
        [JsonPropertyName("LEVEL1_MIN_MAX_PIXEL_VALUE")]
        public Level1MinMaxPixelValue? Level1MinMaxPixelValue { get; set; }

        /// <summary>
        /// Секция Level 1 Radiometric Rescaling
        /// </summary>
        [JsonPropertyName("LEVEL1_RADIOMETRIC_RESCALING")]
        public Level1RadiometricRescaling? Level1RadiometricRescaling { get; set; }

        /// <summary>
        /// Секция Level 1 Thermal Constants
        /// </summary>
        [JsonPropertyName("LEVEL1_THERMAL_CONSTANTS")]
        public Level1ThermalConstants? Level1ThermalConstants { get; set; }

        /// <summary>
        /// Секция Level 1 Projection Parameters
        /// </summary>
        [JsonPropertyName("LEVEL1_PROJECTION_PARAMETERS")]
        public Level1ProjectionParameters? Level1ProjectionParameters { get; set; }
    }
} 