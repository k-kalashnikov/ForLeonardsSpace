using System.Text.Json.Serialization;

namespace Masofa.Client.IBMWeather.Models;

/// <summary>
/// Response model for /v3/wx/conditions/historical/hourly/1day endpoint
/// Provides 1-day historical hourly weather data
/// </summary>
public class HistoricalHourlyResponse : List<HistoricalHourlyItem>
{
}

/// <summary>
/// Individual item in the historical hourly response
/// </summary>
public class HistoricalHourlyItem
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("v3-wx-conditions-historical-hourly-1day")]
    public HistoricalHourlyData Data { get; set; } = new();
}

/// <summary>
/// Historical hourly weather data
/// </summary>
public class HistoricalHourlyData
{
    [JsonPropertyName("cloudCeiling")]
    public List<int> CloudCeiling { get; set; } = new();

    [JsonPropertyName("dayOfWeek")]
    public List<string> DayOfWeek { get; set; } = new();

    [JsonPropertyName("dayOrNight")]
    public List<string> DayOrNight { get; set; } = new();

    [JsonPropertyName("iconCode")]
    public List<int> IconCode { get; set; } = new();

    [JsonPropertyName("precip24Hour")]
    public List<double> Precip24Hour { get; set; } = new();

    [JsonPropertyName("pressureAltimeter")]
    public List<double> PressureAltimeter { get; set; } = new();

    [JsonPropertyName("relativeHumidity")]
    public List<int> RelativeHumidity { get; set; } = new();

    [JsonPropertyName("snow24Hour")]
    public List<double> Snow24Hour { get; set; } = new();

    [JsonPropertyName("sunriseTimeLocal")]
    public List<string> SunriseTimeLocal { get; set; } = new(); // ISO8601 format

    [JsonPropertyName("sunsetTimeLocal")]
    public List<string> SunsetTimeLocal { get; set; } = new(); // ISO8601 format

    [JsonPropertyName("temperature")]
    public List<int> Temperature { get; set; } = new();

    [JsonPropertyName("uvDescription")]
    public List<string> UvDescription { get; set; } = new();

    [JsonPropertyName("uvIndex")]
    public List<int> UvIndex { get; set; } = new();

    [JsonPropertyName("validTimeLocal")]
    public List<string> ValidTimeLocal { get; set; } = new(); // ISO8601 format

    [JsonPropertyName("windDirection")]
    public List<int> WindDirection { get; set; } = new();

    [JsonPropertyName("windSpeed")]
    public List<int> WindSpeed { get; set; } = new();

    [JsonPropertyName("wxPhraseLong")]
    public List<string> WxPhraseLong { get; set; } = new();
}
