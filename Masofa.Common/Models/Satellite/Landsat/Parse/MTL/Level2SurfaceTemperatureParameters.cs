using System.Text.Json.Serialization;

namespace Masofa.Common.Models.Satellite.Parse.Landsat.MTL
{
    public class Level2SurfaceTemperatureParameters
    {
        [JsonPropertyName("TEMPERATURE_MAXIMUM_BAND_ST_B10")]
        public string TemperatureMaximumBandStB10 { get; set; } = default!;

        [JsonPropertyName("TEMPERATURE_MINIMUM_BAND_ST_B10")]
        public string TemperatureMinimumBandStB10 { get; set; } = default!;

        [JsonPropertyName("QUANTIZE_CAL_MAXIMUM_BAND_ST_B10")]
        public string QuantizeCalMaximumBandStB10 { get; set; } = default!;

        [JsonPropertyName("QUANTIZE_CAL_MINIMUM_BAND_ST_B10")]
        public string QuantizeCalMinimumBandStB10 { get; set; } = default!;

        [JsonPropertyName("TEMPERATURE_MULT_BAND_ST_B10")]
        public string TemperatureMultBandStB10 { get; set; } = default!;

        [JsonPropertyName("TEMPERATURE_ADD_BAND_ST_B10")]
        public string TemperatureAddBandStB10 { get; set; } = default!;
    }
}
