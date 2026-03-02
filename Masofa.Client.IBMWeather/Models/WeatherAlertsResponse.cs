using System.Text.Json.Serialization;

namespace Masofa.Client.IBMWeather.Models;

/// <summary>
/// Response model for /v3/alerts/headlines endpoint
/// Provides weather alerts and warnings for a specific location
/// </summary>
public class WeatherAlertsResponse
{
    [JsonPropertyName("metadata")]
    public AlertsMetadata Metadata { get; set; } = new();

    [JsonPropertyName("alerts")]
    public List<WeatherAlert> Alerts { get; set; } = new();
}

/// <summary>
/// Metadata for alerts response
/// </summary>
public class AlertsMetadata
{
    [JsonPropertyName("next")]
    public int? Next { get; set; }
}

/// <summary>
/// Individual weather alert
/// </summary>
public class WeatherAlert
{
    [JsonPropertyName("adminDistrict")]
    public string? AdminDistrict { get; set; }

    [JsonPropertyName("adminDistrictCode")]
    public string? AdminDistrictCode { get; set; }

    [JsonPropertyName("areaId")]
    public string AreaId { get; set; } = string.Empty;

    [JsonPropertyName("areaName")]
    public string AreaName { get; set; } = string.Empty;

    [JsonPropertyName("areaTypeCode")]
    public string AreaTypeCode { get; set; } = string.Empty; // C (county), Z (zone), CLC (Canada Location Code)

    [JsonPropertyName("certainty")]
    public string Certainty { get; set; } = string.Empty; // Observed, Likely, Possible

    [JsonPropertyName("certaintyCode")]
    public string CertaintyCode { get; set; } = string.Empty; // 1 (Observed), 2 (Likely), 3 (Possible)

    [JsonPropertyName("countryCode")]
    public string CountryCode { get; set; } = string.Empty;

    [JsonPropertyName("countryName")]
    public string CountryName { get; set; } = string.Empty;

    [JsonPropertyName("detailKey")]
    public string DetailKey { get; set; } = string.Empty;

    [JsonPropertyName("disclaimer")]
    public string? Disclaimer { get; set; }

    [JsonPropertyName("displayRank")]
    public int DisplayRank { get; set; }

    [JsonPropertyName("effectiveTimeLocal")]
    public string? EffectiveTimeLocal { get; set; } // ISO8601 format

    [JsonPropertyName("effectiveTimeLocalTimeZone")]
    public string? EffectiveTimeLocalTimeZone { get; set; }

    [JsonPropertyName("eventDescription")]
    public string EventDescription { get; set; } = string.Empty;

    [JsonPropertyName("eventTrackingNumber")]
    public string EventTrackingNumber { get; set; } = string.Empty;

    [JsonPropertyName("expireTimeLocal")]
    public string ExpireTimeLocal { get; set; } = string.Empty;

    [JsonPropertyName("expireTimeLocalTimeZone")]
    public string ExpireTimeLocalTimeZone { get; set; } = string.Empty;

    [JsonPropertyName("expireTimeUTC")]
    public long ExpireTimeUTC { get; set; } // Unix timestamp

    [JsonPropertyName("headlineText")]
    public string HeadlineText { get; set; } = string.Empty;

    [JsonPropertyName("ianaTimeZone")]
    public string? IanaTimeZone { get; set; }

    [JsonPropertyName("identifier")]
    public string Identifier { get; set; } = string.Empty; // Unique alert identifier (checksum)

    [JsonPropertyName("issueTimeLocal")]
    public string IssueTimeLocal { get; set; } = string.Empty;

    [JsonPropertyName("issueTimeLocalTimeZone")]
    public string IssueTimeLocalTimeZone { get; set; } = string.Empty;

    [JsonPropertyName("latitude")]
    public double? Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double? Longitude { get; set; }

    [JsonPropertyName("messageType")]
    public string MessageType { get; set; } = string.Empty; // New, Update, Cancel

    [JsonPropertyName("messageTypeCode")]
    public int MessageTypeCode { get; set; } // 1 (New), 2 (Update), 3 (Cancel)

    [JsonPropertyName("officeAdminDistrict")]
    public string? OfficeAdminDistrict { get; set; }

    [JsonPropertyName("officeAdminDistrictCode")]
    public string? OfficeAdminDistrictCode { get; set; }

    [JsonPropertyName("officeCode")]
    public string OfficeCode { get; set; } = string.Empty; // Office source code (e.g., NWS)

    [JsonPropertyName("officeCountryCode")]
    public string? OfficeCountryCode { get; set; }

    [JsonPropertyName("officeName")]
    public string OfficeName { get; set; } = string.Empty;

    [JsonPropertyName("onsetTimeLocal")]
    public string? OnsetTimeLocal { get; set; }

    [JsonPropertyName("onsetTimeLocalTimeZone")]
    public string? OnsetTimeLocalTimeZone { get; set; }

    [JsonPropertyName("phenomena")]
    public string Phenomena { get; set; } = string.Empty; // Phenomenon code (TO, SU, FL, etc.)

    [JsonPropertyName("processTimeUTC")]
    public long ProcessTimeUTC { get; set; }

    [JsonPropertyName("productIdentifier")]
    public string ProductIdentifier { get; set; } = string.Empty;

    [JsonPropertyName("severity")]
    public string Severity { get; set; } = string.Empty; // Extreme, Severe, Moderate, Minor, Unknown

    [JsonPropertyName("severityCode")]
    public int SeverityCode { get; set; } // 1 (Extreme), 2 (Severe), ...

    [JsonPropertyName("significance")]
    public string Significance { get; set; } = string.Empty; // W (Warning), Y (Advisory), O (Outlook)

    [JsonPropertyName("source")]
    public string Source { get; set; } = string.Empty;

    [JsonPropertyName("urgency")]
    public string Urgency { get; set; } = string.Empty; // Immediate, Expected, Future, Past, Unknown

    [JsonPropertyName("urgencyCode")]
    public int UrgencyCode { get; set; }

    [JsonPropertyName("endTimeLocal")]
    public string? EndTimeLocal { get; set; }

    [JsonPropertyName("endTimeLocalTimeZone")]
    public string? EndTimeLocalTimeZone { get; set; }

    [JsonPropertyName("endTimeUTC")]
    public long? EndTimeUTC { get; set; }

    [JsonPropertyName("category")]
    public List<AlertCategory> Category { get; set; } = new();

    [JsonPropertyName("responseTypes")]
    public List<AlertResponseType> ResponseTypes { get; set; } = new();

    [JsonPropertyName("flood")]
    public FloodAlertInfo? Flood { get; set; }
}

/// <summary>
/// Alert category information
/// </summary>
public class AlertCategory
{
    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    [JsonPropertyName("categoryCode")]
    public int CategoryCode { get; set; }
}

/// <summary>
/// Alert response type information
/// </summary>
public class AlertResponseType
{
    [JsonPropertyName("responseType")]
    public string ResponseType { get; set; } = string.Empty; // Monitor, Evacuate, etc.

    [JsonPropertyName("responseTypeCode")]
    public int ResponseTypeCode { get; set; }
}

/// <summary>
/// Flood-specific alert information
/// </summary>
public class FloodAlertInfo
{
    [JsonPropertyName("floodCrestTimeLocal")]
    public string? FloodCrestTimeLocal { get; set; }

    [JsonPropertyName("floodCrestTimeLocalTimeZone")]
    public string? FloodCrestTimeLocalTimeZone { get; set; }

    [JsonPropertyName("floodEndTimeLocal")]
    public string? FloodEndTimeLocal { get; set; }

    [JsonPropertyName("floodEndTimeLocalTimeZone")]
    public string? FloodEndTimeLocalTimeZone { get; set; }

    [JsonPropertyName("floodImmediateCause")]
    public string? FloodImmediateCause { get; set; }

    [JsonPropertyName("floodImmediateCauseCode")]
    public string? FloodImmediateCauseCode { get; set; }

    [JsonPropertyName("floodLocationId")]
    public string? FloodLocationId { get; set; }

    [JsonPropertyName("floodLocationName")]
    public string? FloodLocationName { get; set; }

    [JsonPropertyName("floodRecordStatus")]
    public string? FloodRecordStatus { get; set; }

    [JsonPropertyName("floodRecordStatusCode")]
    public string? FloodRecordStatusCode { get; set; }

    [JsonPropertyName("floodSeverity")]
    public string? FloodSeverity { get; set; }

    [JsonPropertyName("floodSeverityCode")]
    public string? FloodSeverityCode { get; set; }

    [JsonPropertyName("floodStartTimeLocal")]
    public string? FloodStartTimeLocal { get; set; }

    [JsonPropertyName("floodStartTimeLocalTimeZone")]
    public string? FloodStartTimeLocalTimeZone { get; set; }
}
