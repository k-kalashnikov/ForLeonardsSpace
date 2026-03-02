using System.Text.Json.Serialization;
using System.Globalization;

namespace Masofa.Common.Models.Satellite.Auxiliary;

public class TileGeoposition
{
    [JsonPropertyName("resolution")]
    public int Resolution { get; set; }

    [JsonPropertyName("ULX")]
    public double ULX { get; set; }

    [JsonPropertyName("ULY")]
    public double ULY { get; set; }

    [JsonPropertyName("XDIM")]
    public double XDim { get; set; }

    [JsonPropertyName("YDIM")]
    public double YDim { get; set; }

    public override string ToString()
    {
        return string.Join("|",
        [
            Resolution.ToString(),
            ULX.ToString(),
            ULY.ToString(),
            XDim.ToString(),
            YDim.ToString(),
        ]);
    }

    public static TileGeoposition FromString(string str)
    {
        var parts = str?.Split('|');
        if (parts?.Length != 5)
        {
            return null;
        }

        return new TileGeoposition
        {
            Resolution = int.Parse(parts[0]),
            ULX = double.Parse(parts[1], CultureInfo.InvariantCulture),
            ULY = double.Parse(parts[2], CultureInfo.InvariantCulture),
            XDim = double.Parse(parts[3], CultureInfo.InvariantCulture),
            YDim = double.Parse(parts[4], CultureInfo.InvariantCulture)
        };
    }
}
