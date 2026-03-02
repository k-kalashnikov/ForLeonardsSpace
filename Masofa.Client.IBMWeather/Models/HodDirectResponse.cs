using System.Text.Json.Serialization;

namespace Masofa.Client.IBMWeather.Models;

/// <summary>
/// Response model for /v3/wx/hod/r1/direct endpoint
/// Provides Hour of Day (HOD) weather data for a specific location
/// </summary>
public class HodDirectResponse : List<HodDirectItem>
{
}

/// <summary>
/// Individual HOD weather data item
/// </summary>
public class HodDirectItem
{
    [JsonPropertyName("requestedLatitude")]
    public double RequestedLatitude { get; set; }

    [JsonPropertyName("requestedLongitude")]
    public double RequestedLongitude { get; set; }

    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }

    [JsonPropertyName("gridpointId")]
    public string GridpointId { get; set; } = string.Empty;

    [JsonPropertyName("validTimeUtc")]
    public string ValidTimeUtc { get; set; } = string.Empty; // ISO8601 format

    [JsonPropertyName("temperature")]
    public double Temperature { get; set; }

    [JsonPropertyName("temperatureDewPoint")]
    public double TemperatureDewPoint { get; set; }

    [JsonPropertyName("evapotranspiration")]
    public double Evapotranspiration { get; set; }

    [JsonPropertyName("uvIndex")]
    public int UvIndex { get; set; }

    [JsonPropertyName("precip1Hour")]
    public double Precip1Hour { get; set; }

    [JsonPropertyName("windSpeed")]
    public double WindSpeed { get; set; }

    [JsonPropertyName("windDirection")]
    public double WindDirection { get; set; }

    // Additional weather parameters can be added here based on the products parameter in the request
    // The exact fields depend on what weather parameters are requested via the products parameter
}
