using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Masofa.Client.IBMWeather.Models
{
    /// <summary>
    /// Response model for /v3/wx/forecast/daily/7day endpoint
    /// Provides 7-day weather forecast data
    /// </summary>
    public class ForecastDailyResponse
    {
        [JsonPropertyName("calendarDayTemperatureMax")]
        [JsonProperty("calendarDayTemperatureMax")]
        public List<int?>? CalendarDayTemperatureMax { get; set; } = [];

        [JsonPropertyName("calendarDayTemperatureMin")]
        [JsonProperty("calendarDayTemperatureMin")]
        public List<int?>? CalendarDayTemperatureMin { get; set; } = [];

        [JsonPropertyName("dayOfWeek")]
        [JsonProperty("dayOfWeek")]
        public List<string?>? DayOfWeek { get; set; } = [];

        [JsonPropertyName("qpf")]
        [JsonProperty("qpf")]
        public List<double?>? Qpf { get; set; } = [];

        [JsonPropertyName("qpfSnow")]
        [JsonProperty("qpfSnow")]
        public List<double?>? QpfSnow { get; set; } = [];

        [JsonPropertyName("temperatureMax")]
        [JsonProperty("temperatureMax")]
        public List<int?>? TemperatureMax { get; set; } = [];

        [JsonPropertyName("temperatureMin")]
        [JsonProperty("temperatureMin")]
        public List<int?>? TemperatureMin { get; set; } = [];

        [JsonPropertyName("validTimeUtc")]
        [JsonProperty("validTimeUtc")]
        public List<int?>? ValidTimeUtc { get; set; } = [];

        [JsonPropertyName("validTimeLocal")]
        [JsonProperty("validTimeLocal")]
        public List<string?>? ValidTimeLocal { get; set; } = [];

        [JsonPropertyName("sunriseTimeLocal")]
        [JsonProperty("sunriseTimeLocal")]
        public List<string> SunriseTimeLocal { get; set; } = new(); // ISO8601 format

        [JsonPropertyName("sunsetTimeLocal")]
        [JsonProperty("sunsetTimeLocal")]
        public List<string> SunsetTimeLocal { get; set; } = new(); // ISO8601 format

        [JsonPropertyName("daypart")]
        [JsonProperty("daypart")]
        public List<DayPartForecast> Daypart { get; set; } = [];
    }

    /// <summary>
    /// Day part forecast data (day/night periods)
    /// </summary>
    public class DayPartForecast
    {
        [JsonPropertyName("cloudCover")]
        [JsonProperty("cloudCover")]
        public List<double?>? CloudCover { get; set; } = [];

        [JsonPropertyName("dayOrNight")]
        [JsonProperty("dayOrNight")]
        public List<string?>? DayOrNight { get; set; } = [];

        [JsonPropertyName("daypartName")]
        [JsonProperty("daypartName")]
        public List<string?>? DaypartName { get; set; } = [];

        [JsonPropertyName("precipChance")]
        [JsonProperty("precipChance")]
        public List<int?>? PrecipChance { get; set; } = [];

        [JsonPropertyName("precipType")]
        [JsonProperty("precipType")]
        public List<string?>? PrecipType { get; set; } = [];

        [JsonPropertyName("qpf")]
        [JsonProperty("qpf")]
        public List<double?>? Qpf { get; set; } = []; // Quantitative Precipitation Forecast

        [JsonPropertyName("qpfSnow")]
        [JsonProperty("qpfSnow")]
        public List<double?>? QpfSnow { get; set; } = []; // Quantitative Precipitation Forecast for Snow

        [JsonPropertyName("relativeHumidity")]
        [JsonProperty("relativeHumidity")]
        public List<int?>? RelativeHumidity { get; set; } = [];

        [JsonPropertyName("temperature")]
        [JsonProperty("temperature")]
        public List<int?>? Temperature { get; set; } = [];

        [JsonPropertyName("windDirection")]
        [JsonProperty("windDirection")]
        public List<int?>? WindDirection { get; set; } = [];

        [JsonPropertyName("windSpeed")]
        [JsonProperty("windSpeed")]
        public List<int?>? WindSpeed { get; set; } = [];

        [JsonPropertyName("wxPhraseLong")]
        [JsonProperty("wxPhraseLong")]
        public List<string?>? WxPhraseLong { get; set; } = [];
    }
}