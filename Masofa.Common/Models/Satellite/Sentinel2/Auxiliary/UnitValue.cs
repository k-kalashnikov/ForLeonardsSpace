using System.Text.Json.Serialization;
using System.Globalization;

namespace Masofa.Common.Models.Satellite.Auxiliary;

public class UnitValue
{
    [JsonPropertyName("unit")]
    public string Unit { get; set; }

    [JsonPropertyName("value")]
    public double Value { get; set; }

    public override string ToString()
    {
        return string.Join("|",
        [
            Unit,
            Value.ToString()
        ]);
    }

    public static UnitValue FromString(string str)
    {
        var parts = str?.Split('|');
        if (parts?.Length != 2)
        {
            return null;
        }

        return new UnitValue
        {
            Unit = parts[0],
            Value = double.Parse(parts[1], CultureInfo.InvariantCulture)
        };
    }
}
