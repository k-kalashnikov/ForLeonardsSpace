using System.Text.Json.Serialization;

namespace Masofa.Client.IBMWeather.Models;

/// <summary>
/// Response model for /v1/geocode/{lat}/{lon}/observations.json endpoint
/// Provides current weather observations for a specific location
/// </summary>
public class GeocodeObservationsResponse
{
    [JsonPropertyName("metadata")]
    public ObservationMetadata Metadata { get; set; } = new();

    [JsonPropertyName("observation")]
    public WeatherObservation Observation { get; set; } = new();
}

/// <summary>
/// Metadata information for the observation response
/// </summary>
public class ObservationMetadata
{
    [JsonPropertyName("language")]
    public string Language { get; set; } = string.Empty;

    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }

    [JsonPropertyName("units")]
    public string Units { get; set; } = string.Empty;

    [JsonPropertyName("status_code")]
    public int StatusCode { get; set; }
}

/// <summary>
/// Current weather observation data
/// </summary>
public class WeatherObservation
{
    [JsonPropertyName("temp")]
    public int Temp { get; set; }

    [JsonPropertyName("feels_like")]
    public int FeelsLike { get; set; }

    [JsonPropertyName("dewPt")]
    public int DewPt { get; set; }

    [JsonPropertyName("rh")]
    public int Rh { get; set; }

    [JsonPropertyName("wspd")]
    public int Wspd { get; set; }

    [JsonPropertyName("wdir")]
    public int Wdir { get; set; }

    [JsonPropertyName("wdir_cardinal")]
    public string WdirCardinal { get; set; } = string.Empty;

    [JsonPropertyName("gust")]
    public int? Gust { get; set; }

    [JsonPropertyName("pressure")]
    public double Pressure { get; set; }

    [JsonPropertyName("wx_phrase")]
    public string WxPhrase { get; set; } = string.Empty;

    [JsonPropertyName("precip_total")]
    public double PrecipTotal { get; set; }

    [JsonPropertyName("uv_index")]
    public int UvIndex { get; set; }

    [JsonPropertyName("uv_desc")]
    public string UvDesc { get; set; } = string.Empty;

    [JsonPropertyName("obs_id")]
    public string ObsId { get; set; } = string.Empty;

    [JsonPropertyName("obs_name")]
    public string ObsName { get; set; } = string.Empty;

    [JsonPropertyName("max_temp")]
    public int MaxTemp { get; set; }

    [JsonPropertyName("min_temp")]
    public int MinTemp { get; set; }

    [JsonPropertyName("icon_extd")]
    public int IconExtd { get; set; }
}
