using System.Text.Json.Serialization;

namespace Masofa.Common.Models.Satellite.Parse.Landsat.MTL
{
    public class Level1ThermalConstants
    {
        [JsonPropertyName("K1_CONSTANT_BAND_10")]
        public string K1ConstantBand10 { get; set; } = default!;

        [JsonPropertyName("K2_CONSTANT_BAND_10")]
        public string K2ConstantBand10 { get; set; } = default!;

        [JsonPropertyName("K1_CONSTANT_BAND_11")]
        public string K1ConstantBand11 { get; set; } = default!;

        [JsonPropertyName("K2_CONSTANT_BAND_11")]
        public string K2ConstantBand11 { get; set; } = default!;
    }
}
