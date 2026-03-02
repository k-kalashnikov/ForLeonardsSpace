using System.Text.Json.Serialization;

namespace Masofa.Common.Models.Satellite.Parse.Landsat.MTL
{
    public class Level1MinMaxReflectance
    {
        [JsonPropertyName("REFLECTANCE_MAXIMUM_BAND_1")]
        public string ReflectanceMaximumBand1 { get; set; } = default!;

        [JsonPropertyName("REFLECTANCE_MINIMUM_BAND_1")]
        public string ReflectanceMinimumBand1 { get; set; } = default!;

        [JsonPropertyName("REFLECTANCE_MAXIMUM_BAND_2")]
        public string ReflectanceMaximumBand2 { get; set; } = default!;

        [JsonPropertyName("REFLECTANCE_MINIMUM_BAND_2")]
        public string ReflectanceMinimumBand2 { get; set; } = default!;

        [JsonPropertyName("REFLECTANCE_MAXIMUM_BAND_3")]
        public string ReflectanceMaximumBand3 { get; set; } = default!;

        [JsonPropertyName("REFLECTANCE_MINIMUM_BAND_3")]
        public string ReflectanceMinimumBand3 { get; set; } = default!;

        [JsonPropertyName("REFLECTANCE_MAXIMUM_BAND_4")]
        public string ReflectanceMaximumBand4 { get; set; } = default!;

        [JsonPropertyName("REFLECTANCE_MINIMUM_BAND_4")]
        public string ReflectanceMinimumBand4 { get; set; } = default!;

        [JsonPropertyName("REFLECTANCE_MAXIMUM_BAND_5")]
        public string ReflectanceMaximumBand5 { get; set; } = default!;

        [JsonPropertyName("REFLECTANCE_MINIMUM_BAND_5")]
        public string ReflectanceMinimumBand5 { get; set; } = default!;

        [JsonPropertyName("REFLECTANCE_MAXIMUM_BAND_6")]
        public string ReflectanceMaximumBand6 { get; set; } = default!;

        [JsonPropertyName("REFLECTANCE_MINIMUM_BAND_6")]
        public string ReflectanceMinimumBand6 { get; set; } = default!;

        [JsonPropertyName("REFLECTANCE_MAXIMUM_BAND_7")]
        public string ReflectanceMaximumBand7 { get; set; } = default!;

        [JsonPropertyName("REFLECTANCE_MINIMUM_BAND_7")]
        public string ReflectanceMinimumBand7 { get; set; } = default!;

        [JsonPropertyName("REFLECTANCE_MAXIMUM_BAND_8")]
        public string ReflectanceMaximumBand8 { get; set; } = default!;

        [JsonPropertyName("REFLECTANCE_MINIMUM_BAND_8")]
        public string ReflectanceMinimumBand8 { get; set; } = default!;

        [JsonPropertyName("REFLECTANCE_MAXIMUM_BAND_9")]
        public string ReflectanceMaximumBand9 { get; set; } = default!;

        [JsonPropertyName("REFLECTANCE_MINIMUM_BAND_9")]
        public string ReflectanceMinimumBand9 { get; set; } = default!;
    }
}
