using System.Text.Json.Serialization;

namespace Masofa.Common.Models.Satellite.Auxiliary;

public class Check
{
    [JsonPropertyName("inspection")]
    public string? Inspection { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("extraValues")]
    public ExtraValues? ExtraValues { get; set; }

    public override string ToString()
    {
        return string.Join("|",
        [
            Inspection ?? "",
            Message ?? "",
            ExtraValues?.ToString() ?? ""
        ]);
    }

    public static Check FromString(string str)
    {
        var parts = str?.Split('|');
        if (parts?.Length != 3)
        {
            return null;
        }

        return new Check
        {
            Inspection = parts[0],
            Message = parts[1],
            ExtraValues = !string.IsNullOrWhiteSpace(parts[2]) 
                ? new ExtraValues { Value = parts[2] }
                : null
        };
    }
}

public class ExtraValues
{
    [JsonPropertyName("value")]
    public object? Value { get; set; }

    public override string ToString() => Value?.ToString() ?? "";
}
