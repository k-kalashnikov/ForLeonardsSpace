using System.Text.Json.Serialization;

namespace Masofa.Common.Models.Satellite.Parse.Landsat.MTL
{
    public class Level1ProjectionParameters
    {
        [JsonPropertyName("MAP_PROJECTION")]
        public string MapProjection { get; set; } = default!;

        [JsonPropertyName("DATUM")]
        public string Datum { get; set; } = default!;

        [JsonPropertyName("ELLIPSOID")]
        public string Ellipsoid { get; set; } = default!;

        [JsonPropertyName("UTM_ZONE")]
        public string UtmZone { get; set; } = default!;

        [JsonPropertyName("GRID_CELL_SIZE_PANCHROMATIC")]
        public string GridCellSizePanchromatic { get; set; } = default!;

        [JsonPropertyName("GRID_CELL_SIZE_REFLECTIVE")]
        public string GridCellSizeReflective { get; set; } = default!;

        [JsonPropertyName("GRID_CELL_SIZE_THERMAL")]
        public string GridCellSizeThermal { get; set; } = default!;

        [JsonPropertyName("ORIENTATION")]
        public string Orientation { get; set; } = default!;

        [JsonPropertyName("RESAMPLING_OPTION")]
        public string ResamplingOption { get; set; } = default!;
    }
}
