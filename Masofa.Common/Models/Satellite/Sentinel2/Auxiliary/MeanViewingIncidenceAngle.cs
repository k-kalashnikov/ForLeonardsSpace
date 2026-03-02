using System.Text.Json.Serialization;
using System.Globalization;

namespace Masofa.Common.Models.Satellite.Auxiliary;

public class MeanViewingIncidenceAngle
{
    [JsonPropertyName("bandId")]
    public int BandId { get; set; }

    [JsonPropertyName("ZENITH_ANGLE")]
    public double ZenithAngle { get; set; }

    [JsonPropertyName("AZIMUTH_ANGLE")]
    public double AzimuthAngle { get; set; }

    public override string ToString()
    {
        return string.Join("|",
        [
            BandId.ToString(),
            string.Empty, // Value field from parse model
            ZenithAngle.ToString(CultureInfo.InvariantCulture),
            AzimuthAngle.ToString(CultureInfo.InvariantCulture)
        ]);
    }

    public static MeanViewingIncidenceAngle FromString(string str)
    {
        var parts = str?.Split('|');
        if (parts?.Length != 4)
        {
            return null;
        }

        return new MeanViewingIncidenceAngle
        {
            BandId = int.Parse(parts[0]),
            ZenithAngle = double.Parse(parts[2], CultureInfo.InvariantCulture),
            AzimuthAngle = double.Parse(parts[3], CultureInfo.InvariantCulture)
        };
    }
}
