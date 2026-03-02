using System.Text.Json.Serialization;

namespace Masofa.Common.Models.Satellite.Parse.Landsat.MTL
{
    public class ImageAttributes
    {
        [JsonPropertyName("SPACECRAFT_ID")]
        public string SpacecraftId { get; set; } = default!;

        [JsonPropertyName("SENSOR_ID")]
        public string SensorId { get; set; } = default!;

        [JsonPropertyName("WRS_TYPE")]
        public string WrsType { get; set; } = default!;

        [JsonPropertyName("WRS_PATH")]
        public string WrsPath { get; set; } = default!;

        [JsonPropertyName("WRS_ROW")]
        public string WrsRow { get; set; } = default!;

        [JsonPropertyName("NADIR_OFFNADIR")]
        public string NadirOffnadir { get; set; } = default!;

        [JsonPropertyName("TARGET_WRS_PATH")]
        public string TargetWrsPath { get; set; } = default!;

        [JsonPropertyName("TARGET_WRS_ROW")]
        public string TargetWrsRow { get; set; } = default!;

        [JsonPropertyName("DATE_ACQUIRED")]
        public string DateAcquired { get; set; } = default!;

        [JsonPropertyName("SCENE_CENTER_TIME")]
        public string SceneCenterTime { get; set; } = default!;

        [JsonPropertyName("STATION_ID")]
        public string StationId { get; set; } = default!;

        [JsonPropertyName("CLOUD_COVER")]
        public string CloudCover { get; set; } = default!;

        [JsonPropertyName("CLOUD_COVER_LAND")]
        public string CloudCoverLand { get; set; } = default!;

        [JsonPropertyName("IMAGE_QUALITY_OLI")]
        public string ImageQualityOli { get; set; } = default!;

        [JsonPropertyName("IMAGE_QUALITY_TIRS")]
        public string ImageQualityTirs { get; set; } = default!;

        [JsonPropertyName("SATURATION_BAND_1")]
        public string SaturationBand1 { get; set; } = default!;

        [JsonPropertyName("SATURATION_BAND_2")]
        public string SaturationBand2 { get; set; } = default!;

        [JsonPropertyName("SATURATION_BAND_3")]
        public string SaturationBand3 { get; set; } = default!;

        [JsonPropertyName("SATURATION_BAND_4")]
        public string SaturationBand4 { get; set; } = default!;

        [JsonPropertyName("SATURATION_BAND_5")]
        public string SaturationBand5 { get; set; } = default!;

        [JsonPropertyName("SATURATION_BAND_6")]
        public string SaturationBand6 { get; set; } = default!;

        [JsonPropertyName("SATURATION_BAND_7")]
        public string SaturationBand7 { get; set; } = default!;

        [JsonPropertyName("SATURATION_BAND_8")]
        public string SaturationBand8 { get; set; } = default!;

        [JsonPropertyName("SATURATION_BAND_9")]
        public string SaturationBand9 { get; set; } = default!;

        [JsonPropertyName("ROLL_ANGLE")]
        public string RollAngle { get; set; } = default!;

        [JsonPropertyName("SUN_AZIMUTH")]
        public string SunAzimuth { get; set; } = default!;

        [JsonPropertyName("SUN_ELEVATION")]
        public string SunElevation { get; set; } = default!;

        [JsonPropertyName("EARTH_SUN_DISTANCE")]
        public string EarthSunDistance { get; set; } = default!;
    }
}
