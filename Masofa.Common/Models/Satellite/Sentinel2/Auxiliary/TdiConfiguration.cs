using System.Text.Json.Serialization;

namespace Masofa.Common.Models.Satellite.Auxiliary;

public class TdiConfiguration
{
    [JsonPropertyName("bandId")]
    public int BandId { get; set; }

    [JsonPropertyName("value")]
    public int Value { get; set; }

    public override string ToString()
    {
        return string.Join("|",
        [
            BandId.ToString(),
            Value.ToString()
        ]);
    }

    public static TdiConfiguration FromString(string str)
    {
        var parts = str?.Split('|');
        if (parts?.Length != 2)
        {
            return null;
        }

        return new TdiConfiguration
        {
            BandId = int.Parse(parts[0]),
            Value = int.Parse(parts[1])
        };
    }
}
