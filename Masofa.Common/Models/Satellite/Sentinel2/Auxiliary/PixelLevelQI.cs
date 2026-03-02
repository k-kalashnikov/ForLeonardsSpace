using System.Text.Json.Serialization;

namespace Masofa.Common.Models.Satellite.Auxiliary;

public class PixelLevelQI
{
    [JsonPropertyName("geometry")]
    public string Geometry { get; set; }

    [JsonPropertyName("MASK_FILENAME")]
    public List<string> MaskFilenames { get; set; }

    public override string ToString()
    {
        var masks = MaskFilenames != null
            ? string.Join(",", MaskFilenames.Select(m => m))
            : string.Empty;

        return $"{Geometry}#{masks}";
    }

    public static PixelLevelQI FromString(string str)
    {
        if (string.IsNullOrWhiteSpace(str)) return null;

        var parts = str.Split('#');
        if (parts.Length != 2) return null;

        var geometry = parts[0];
        var masksRaw = parts[1];

        var masks = masksRaw.Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(x => x)
                            .Where(m => m != null)
                            .ToList();

        return new PixelLevelQI
        {
            Geometry = geometry,
            MaskFilenames = masks
        };
    }
}