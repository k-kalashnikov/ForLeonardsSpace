using System.Globalization;
using System.Text.Json.Serialization;

namespace Masofa.Common.Models.Satellite.Auxiliary;

public class Datatake
{
    [JsonPropertyName("datatakeIdentifier")]
    public string? DatatakeIdentifier { get; set; }

    [JsonPropertyName("SPACECRAFT_NAME")]
    public string? SpacecraftName { get; set; }

    [JsonPropertyName("DATATAKE_TYPE")]
    public string? DatatakeType { get; set; }

    [JsonPropertyName("DATATAKE_SENSING_START")]
    public DateTime SensingStart { get; set; }

    [JsonPropertyName("SENSING_ORBIT_NUMBER")]
    public int OrbitNumber { get; set; }

    [JsonPropertyName("SENSING_ORBIT_DIRECTION")]
    public string? OrbitDirection { get; set; }

    public override string ToString()
    {
        return string.Join("|",
        [
            DatatakeIdentifier,
            SpacecraftName,
            DatatakeType,
            SensingStart.ToString("o"), // ISO 8601
            OrbitNumber.ToString(),
            OrbitDirection
        ]);
    }

    public static Datatake FromString(string str)
    {
        var parts = str?.Split('|');
        if (parts?.Length < 7)
        {
            return null;
        }

        return new Datatake
        {
            DatatakeIdentifier = parts[1],
            SpacecraftName = parts[2],
            DatatakeType = parts[3],
            SensingStart = DateTime.Parse(parts[4], null, DateTimeStyles.RoundtripKind),
            OrbitNumber = int.Parse(parts[5]),
            OrbitDirection = parts[6]
        };
    }
}
