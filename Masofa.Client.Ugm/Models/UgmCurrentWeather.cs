using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Masofa.Client.Ugm.Models
{

    public class UgmCurrentWeather
    {
        [JsonPropertyName("region_id")]
        [JsonProperty("region_id")]
        public int? RegionId { get; set; }

        [JsonPropertyName("datetime")]
        [JsonProperty("datetime")]
        public DateTime? DateTime { get; set; }

        [JsonPropertyName("cloud_amount")]
        [JsonProperty("cloud_amount")]
        public string? CloudAmount { get; set; }

        [JsonPropertyName("air_t")]
        [JsonProperty("air_t")]
        public double? AirT { get; set; }

        [JsonPropertyName("weather_code")]
        [JsonProperty("weather_code")]
        public string? WeatherCode { get; set; }

        [JsonPropertyName("datetime_ms")]
        [JsonProperty("datetime_ms")]
        public long? DateTimeMs { get; set; }

        [JsonPropertyName("time_of_day")]
        [JsonProperty("time_of_day")]
        public string? TimeOfDay { get; set; }

        [JsonPropertyName("city")]
        [JsonProperty("city")]
        public City? City { get; set; }
    }

    public class City
    {
        [JsonPropertyName("name")]
        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonPropertyName("is_regional_center")]
        [JsonProperty("is_regional_center")]
        public bool? IsRegionalCenter { get; set; }

        [JsonPropertyName("latitude")]
        [JsonProperty("latitude")]
        public double? Latitude { get; set; }

        [JsonPropertyName("longitude")]
        [JsonProperty("longitude")]
        public double? Longitude { get; set; }

        [JsonPropertyName("title")]
        [JsonProperty("title")]
        public string? Title { get; set; }
    }
}
