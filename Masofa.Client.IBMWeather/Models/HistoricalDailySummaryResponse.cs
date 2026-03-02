using System.Text.Json.Serialization;

namespace Masofa.Client.IBMWeather.Models;

/// <summary>
/// Response model for /v3/wx/conditions/historical/dailysummary/30day endpoint
/// Provides 30-day historical daily weather summary data
/// </summary>
public class HistoricalDailySummaryResponse : List<HistoricalDailySummaryItem>
{
}

/// <summary>
/// Individual item in the historical daily summary response
/// </summary>
public class HistoricalDailySummaryItem
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("v3-wx-conditions-historical-dailysummary-30day")]
    public HistoricalDailySummaryData Data { get; set; } = new();
}

/// <summary>
/// Historical daily summary weather data
/// </summary>
public class HistoricalDailySummaryData
{
    [JsonPropertyName("dayOfWeek")]
    public List<string> DayOfWeek { get; set; } = new();

    [JsonPropertyName("iconCodeDay")]
    public List<int> IconCodeDay { get; set; } = new();

    [JsonPropertyName("iconCodeNight")]
    public List<int> IconCodeNight { get; set; } = new();

    [JsonPropertyName("precip24Hour")]
    public List<double> Precip24Hour { get; set; } = new();

    [JsonPropertyName("rain24Hour")]
    public List<double> Rain24Hour { get; set; } = new();

    [JsonPropertyName("snow24Hour")]
    public List<double> Snow24Hour { get; set; } = new();

    [JsonPropertyName("temperatureMax")]
    public List<int> TemperatureMax { get; set; } = new();

    [JsonPropertyName("temperatureMin")]
    public List<int> TemperatureMin { get; set; } = new();

    [JsonPropertyName("validTimeLocal")]
    public List<string> ValidTimeLocal { get; set; } = new(); // ISO8601 format

    [JsonPropertyName("wxPhraseLongDay")]
    public List<string> WxPhraseLongDay { get; set; } = new();

    [JsonPropertyName("wxPhraseLongNight")]
    public List<string> WxPhraseLongNight { get; set; } = new();
}
