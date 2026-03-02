using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Masofa.Client.Era5.Models
{
    public class ERAWeatherResponse
    {
        [JsonPropertyName("latitude")]
        [JsonProperty("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        [JsonProperty("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("hourly")]
        [JsonProperty("hourly")]
        public ERAWeatherData? WeatherData { get; set; }
    }

    public class ERAWeatherData
    {
        [JsonPropertyName("time")]
        [JsonProperty("time")]
        public List<DateTime>? UtcTime { get; set; }

        [JsonPropertyName("temperature_2m")]
        [JsonProperty("temperature_2m")]
        public List<double?>? Temperature { get; set; }

        [JsonPropertyName("relative_humidity_2m")]
        [JsonProperty("relative_humidity_2m")]
        public List<double?>? RelativeHumidity { get; set; }

        [JsonPropertyName("dew_point_2m")]
        [JsonProperty("dew_point_2m")]
        public List<double?>? DewPoint { get; set; }

        [JsonPropertyName("precipitation")]
        [JsonProperty("precipitation")]
        public List<double?>? Precipitation { get; set; }

        [JsonPropertyName("cloud_cover")]
        [JsonProperty("cloud_cover")]
        public List<double?>? CloudCover { get; set; }

        [JsonPropertyName("wind_speed_10m")]
        [JsonProperty("wind_speed_10m")]
        public List<double?>? WindSpeed { get; set; }

        [JsonPropertyName("wind_direction_10m")]
        [JsonProperty("wind_direction_10m")]
        public List<double?>? WindDirection { get; set; }

        [JsonPropertyName("surface_temperature")]
        [JsonProperty("surface_temperature")]
        public List<double?>? GroundTemperature { get; set; }

        [JsonPropertyName("soil_temperature_0_to_7cm")]
        [JsonProperty("soil_temperature_0_to_7cm")]
        public List<double?>? SoilTemperature { get; set; }

        [JsonPropertyName("weather_code")]
        [JsonProperty("weather_code")]
        public List<int?>? ConditionIds { get; set; }

        [JsonPropertyName("soil_moisture_0_to_7cm")]
        [JsonProperty("soil_moisture_0_to_7cm")]
        public List<double?>? SoilHumidity50cm { get; set; }

        [JsonPropertyName("soil_moisture_100_to_255cm")]
        [JsonProperty("soil_moisture_100_to_255cm")]
        public List<double?>? SoilHumidity2m { get; set; }

        [JsonPropertyName("shortwave_radiation")]
        [JsonProperty("shortwave_radiation")]
        public List<double?>? SolarRadiation { get; set; }
    }
}
