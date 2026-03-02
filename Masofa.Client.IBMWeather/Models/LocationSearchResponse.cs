using System.Text.Json.Serialization;

namespace Masofa.Client.IBMWeather.Models;

/// <summary>
/// Response model for /v3/location/search endpoint
/// Provides location search results based on query parameters
/// </summary>
public class LocationSearchResponse
{
    [JsonPropertyName("location")]
    public LocationSearchInfo Location { get; set; } = new();
}

/// <summary>
/// Location search information details
/// </summary>
public class LocationSearchInfo
{
    [JsonPropertyName("address")]
    public List<string> Address { get; set; } = new();

    [JsonPropertyName("adminDistrict")]
    public List<string> AdminDistrict { get; set; } = new();

    [JsonPropertyName("adminDistrictCode")]
    public List<string> AdminDistrictCode { get; set; } = new();

    [JsonPropertyName("airportName")]
    public List<string> AirportName { get; set; } = new();

    [JsonPropertyName("city")]
    public List<string> City { get; set; } = new();

    [JsonPropertyName("country")]
    public List<string> Country { get; set; } = new();

    [JsonPropertyName("countryCode")]
    public List<string> CountryCode { get; set; } = new();

    [JsonPropertyName("displayName")]
    public List<string> DisplayName { get; set; } = new();

    [JsonPropertyName("iataCode")]
    public List<string> IataCode { get; set; } = new();

    [JsonPropertyName("icaoCode")]
    public List<string> IcaoCode { get; set; } = new();

    [JsonPropertyName("latitude")]
    public List<double> Latitude { get; set; } = new();

    [JsonPropertyName("longitude")]
    public List<double> Longitude { get; set; } = new();

    [JsonPropertyName("type")]
    public List<string> Type { get; set; } = new();

    [JsonPropertyName("pwsId")]
    public List<string?> PwsId { get; set; } = new();

    [JsonPropertyName("placeId")]
    public List<string> PlaceId { get; set; } = new();

    [JsonPropertyName("postalCode")]
    public List<string?> PostalCode { get; set; } = new();

    [JsonPropertyName("postalKey")]
    public List<string?> PostalKey { get; set; } = new();
}
