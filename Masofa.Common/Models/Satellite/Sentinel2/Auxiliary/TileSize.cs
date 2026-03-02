using System.Text.Json.Serialization;

namespace Masofa.Common.Models.Satellite.Auxiliary;

public class TileSize
{
    [JsonPropertyName("resolution")]
    public int Resolution { get; set; }

    [JsonPropertyName("NROWS")]
    public int NRows { get; set; }

    [JsonPropertyName("NCOLS")]
    public int NCols { get; set; }

    public override string ToString()
    {
        return string.Join("|",
        [
            Resolution.ToString(),
            NRows.ToString(),
            NCols.ToString()
        ]);
    }

    public static TileSize FromString(string str)
    {
        var parts = str?.Split('|');
        if (parts?.Length != 3)
        {
            return null;
        }

        return new TileSize
        {
            Resolution = int.Parse(parts[0]),
            NRows = int.Parse(parts[1]),
            NCols = int.Parse(parts[2])
        };
    }
}