using System.Text.Json.Serialization;

namespace Masofa.Common.Models.Satellite.Auxiliary;

public class GippFile
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("version")]
    public string Version { get; set; }

    [JsonPropertyName("filename")]
    public string Filename { get; set; }

    public override string ToString()
    {
        return string.Join("|",
        [
            Type,
            Version,
            Filename
        ]);
    }

    public static GippFile FromString(string str)
    {
        var parts = str?.Split('|');
        if (parts?.Length != 3)
        {
            return null;
        }

        return new GippFile
        {
            Type = parts[0],
            Version = parts[1],
            Filename = parts[2],
        };
    }
}