using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Masofa.Client.Ugm.Models
{
    public class UgmForecastContainer
    {
        [JsonPropertyName("day")]
        [JsonProperty("day")]
        public List<UgmForecastItem>? Day { get; set; }

        [JsonPropertyName("night")]
        [JsonProperty("night")]
        public List<UgmForecastItem>? Night { get; set; }
    }

    public class UgmForecastItem
    {
        [JsonPropertyName("region_id")]
        [JsonProperty("region_id")]
        public int? RegionId { get; set; }

        [JsonPropertyName("date")]
        [JsonProperty("date")]
        public DateOnly? Date { get; set; }

        [JsonPropertyName("day_part")]
        [JsonProperty("day_part")]
        public string? DayPart { get; set; }

        [JsonPropertyName("icon")]
        [JsonProperty("icon")]
        public string? Icon { get; set; }

        [JsonPropertyName("air_t_min")]
        [JsonProperty("air_t_min")]
        public int? AirTMin { get; set; }

        [JsonPropertyName("air_t_max")]
        [JsonProperty("air_t_max")]
        public int? AirTMax { get; set; }

        [JsonPropertyName("wind_direction")]
        [JsonProperty("wind_direction")]
        public int? WindDirection { get; set; }

        [JsonPropertyName("wind_direction_change")]
        [JsonProperty("wind_direction_change")]
        public int? WindDirectionChange { get; set; }

        [JsonPropertyName("wind_speed_min")]
        [JsonProperty("wind_speed_min")]
        public int? WindSpeedMin { get; set; }

        [JsonPropertyName("wind_speed_max")]
        [JsonProperty("wind_speed_max")]
        public int? WindSpeedMax { get; set; }

        [JsonPropertyName("wind_speed_min_after_change")]
        [JsonProperty("wind_speed_min_after_change")]
        public int? WindSpeedMinAfterChange { get; set; }

        [JsonPropertyName("wind_speed_max_after_change")]
        [JsonProperty("wind_speed_max_after_change")]
        public int? WindSpeedMaxAfterChange { get; set; }

        [JsonPropertyName("cloud_amount")]
        [JsonProperty("cloud_amount")]
        public string? CloudAmount { get; set; }

        [JsonPropertyName("time_period")]
        [JsonProperty("time_period")]
        public string? TimePeriod { get; set; }

        [JsonPropertyName("region_code")]
        [JsonProperty("region_code")]
        public string? RegionCode { get; set; }

        [JsonPropertyName("region_name")]
        [JsonProperty("region_name")]
        public string? RegionName { get; set; }

        [JsonPropertyName("latitude")]
        [JsonProperty("latitude")]
        public double? Latitude { get; set; }

        [JsonPropertyName("longitude")]
        [JsonProperty("longitude")]
        public double? Longitude { get; set; }

        [JsonPropertyName("precipitation")]
        [JsonProperty("precipitation")]
        public string? Precipitation { get; set; }

        [JsonPropertyName("is_occasional")]
        [JsonProperty("is_occasional")]
        public int? IsOccasional { get; set; }

        [JsonPropertyName("is_possible")]
        [JsonProperty("is_possible")]
        public int? IsPossible { get; set; }

        [JsonPropertyName("thunderstorm")]
        [JsonProperty("thunderstorm")]
        public int? Thunderstorm { get; set; }

        [JsonPropertyName("location")]
        [JsonProperty("location")]
        public string? Location { get; set; }
    }
}
