using System.Text.Json.Serialization;

namespace Masofa.Client.IBMWeather.Models;

/// <summary>
/// Response model for /v3/location/point endpoint
/// Provides location information based on geocode coordinates
/// </summary>
public class LocationPointResponse
{
    [JsonPropertyName("location")]
    public LocationInfo Location { get; set; } = new();
}

/// <summary>
/// Location information details
/// </summary>
public class LocationInfo
{
    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }

    [JsonPropertyName("city")]
    public string City { get; set; } = string.Empty;

    [JsonPropertyName("country")]
    public string Country { get; set; } = string.Empty;

    [JsonPropertyName("countryCode")]
    public string CountryCode { get; set; } = string.Empty;

    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonPropertyName("adminDistrict")]
    public string AdminDistrict { get; set; } = string.Empty;

    [JsonPropertyName("adminDistrictCode")]
    public string AdminDistrictCode { get; set; } = string.Empty;

    [JsonPropertyName("iataCode")]
    public string? IataCode { get; set; }

    [JsonPropertyName("icaoCode")]
    public string? IcaoCode { get; set; }

    [JsonPropertyName("pwsId")]
    public string? PwsId { get; set; }

    [JsonPropertyName("locId")]
    public string? LocId { get; set; }

    [JsonPropertyName("placeId")]
    public string? PlaceId { get; set; }

    [JsonPropertyName("postalKey")]
    public string? PostalKey { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty; // "city", "pws", "airport", etc.
}
