using System.Text.Json.Serialization;

namespace Masofa.Common.Models.Satellite.Auxiliary;

public class AngleGrid
{
    [JsonPropertyName("COL_STEP")]
    public int ColStep { get; set; }

    [JsonPropertyName("ROW_STEP")]
    public int RowStep { get; set; }

    [JsonPropertyName("Values_List")]
    public ValuesList ValuesList { get; set; }

    public override string ToString()
    {
        return string.Join("|",
        [
            ColStep.ToString(),
            RowStep.ToString(),
            ValuesList.ToString()
        ]);
    }

    public static AngleGrid FromString(string str)
    {
        var parts = str?.Split('|');
        if (parts?.Length != 3)
        {
            return null;
        }

        return new AngleGrid
        {
            ColStep = int.Parse(parts[0]),
            RowStep = int.Parse(parts[1]),
            ValuesList = ValuesList.FromString(parts[2])
        };
    }
}
