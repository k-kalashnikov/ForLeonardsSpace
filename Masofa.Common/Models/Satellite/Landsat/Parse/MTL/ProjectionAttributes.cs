using System.Text.Json.Serialization;

namespace Masofa.Common.Models.Satellite.Parse.Landsat.MTL
{
    public class ProjectionAttributes
    {
        [JsonPropertyName("MAP_PROJECTION")]
        public string MapProjection { get; set; } = default!;

        [JsonPropertyName("DATUM")]
        public string Datum { get; set; } = default!;

        [JsonPropertyName("ELLIPSOID")]
        public string Ellipsoid { get; set; } = default!;

        [JsonPropertyName("UTM_ZONE")]
        public string UtmZone { get; set; } = default!;

        [JsonPropertyName("GRID_CELL_SIZE_REFLECTIVE")]
        public string GridCellSizeReflective { get; set; } = default!;

        [JsonPropertyName("GRID_CELL_SIZE_THERMAL")]
        public string GridCellSizeThermal { get; set; } = default!;

        [JsonPropertyName("REFLECTIVE_LINES")]
        public string ReflectiveLines { get; set; } = default!;

        [JsonPropertyName("REFLECTIVE_SAMPLES")]
        public string ReflectiveSamples { get; set; } = default!;

        [JsonPropertyName("THERMAL_LINES")]
        public string ThermalLines { get; set; } = default!;

        [JsonPropertyName("THERMAL_SAMPLES")]
        public string ThermalSamples { get; set; } = default!;

        [JsonPropertyName("ORIENTATION")]
        public string Orientation { get; set; } = default!;

        [JsonPropertyName("CORNER_UL_LAT_PRODUCT")]
        public string CornerUlLatProduct { get; set; } = default!;

        [JsonPropertyName("CORNER_UL_LON_PRODUCT")]
        public string CornerUlLonProduct { get; set; } = default!;

        [JsonPropertyName("CORNER_UR_LAT_PRODUCT")]
        public string CornerUrLatProduct { get; set; } = default!;

        [JsonPropertyName("CORNER_UR_LON_PRODUCT")]
        public string CornerUrLonProduct { get; set; } = default!;

        [JsonPropertyName("CORNER_LL_LAT_PRODUCT")]
        public string CornerLlLatProduct { get; set; } = default!;

        [JsonPropertyName("CORNER_LL_LON_PRODUCT")]
        public string CornerLlLonProduct { get; set; } = default!;

        [JsonPropertyName("CORNER_LR_LAT_PRODUCT")]
        public string CornerLrLatProduct { get; set; } = default!;

        [JsonPropertyName("CORNER_LR_LON_PRODUCT")]
        public string CornerLrLonProduct { get; set; } = default!;

        [JsonPropertyName("CORNER_UL_PROJECTION_X_PRODUCT")]
        public string CornerUlProjectionXProduct { get; set; } = default!;

        [JsonPropertyName("CORNER_UL_PROJECTION_Y_PRODUCT")]
        public string CornerUlProjectionYProduct { get; set; } = default!;

        [JsonPropertyName("CORNER_UR_PROJECTION_X_PRODUCT")]
        public string CornerUrProjectionXProduct { get; set; } = default!;

        [JsonPropertyName("CORNER_UR_PROJECTION_Y_PRODUCT")]
        public string CornerUrProjectionYProduct { get; set; } = default!;

        [JsonPropertyName("CORNER_LL_PROJECTION_X_PRODUCT")]
        public string CornerLlProjectionXProduct { get; set; } = default!;

        [JsonPropertyName("CORNER_LL_PROJECTION_Y_PRODUCT")]
        public string CornerLlProjectionYProduct { get; set; } = default!;

        [JsonPropertyName("CORNER_LR_PROJECTION_X_PRODUCT")]
        public string CornerLrProjectionXProduct { get; set; } = default!;

        [JsonPropertyName("CORNER_LR_PROJECTION_Y_PRODUCT")]
        public string CornerLrProjectionYProduct { get; set; } = default!;
    }
}
