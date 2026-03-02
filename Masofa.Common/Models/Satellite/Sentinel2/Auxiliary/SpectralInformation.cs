using System.Text.Json.Serialization;
using System.Globalization;

namespace Masofa.Common.Models.Satellite.Auxiliary;

public class SpectralInformation
{
    [JsonPropertyName("bandId")]
    public int BandId { get; set; }

    [JsonPropertyName("physicalBand")]
    public string PhysicalBand { get; set; }

    [JsonPropertyName("RESOLUTION")]
    public int Resolution { get; set; }

    [JsonPropertyName("Wavelength")]
    public Wavelength Wavelength { get; set; }

    public override string ToString()
    {
        return string.Join("|",
        [
            BandId.ToString(),
            PhysicalBand,
            Resolution.ToString(),
            Wavelength.ToString()
        ]);
    }

    public static SpectralInformation FromString(string str)
    {
        var parts = str?.Split('|');
        if (parts?.Length != 4)
        {
            return null;
        }

        return new SpectralInformation
        {
            BandId = int.Parse(parts[0]),
            PhysicalBand = parts[1],
            Resolution = int.Parse(parts[2]),
            Wavelength = Wavelength.FromString(parts[3])
        };
    }
}

public class Wavelength
{
    [JsonPropertyName("MIN")]
    public double Min { get; set; }

    [JsonPropertyName("MAX")]
    public double Max { get; set; }

    [JsonPropertyName("CENTRAL")]
    public double Central { get; set; }

    public override string ToString()
    {
        return string.Join("|",
        [
            Min.ToString(),
            Max.ToString(),
            Central.ToString()
        ]);
    }

    public static Wavelength FromString(string str)
    {
        var parts = str?.Split('|');
        if (parts?.Length != 3)
        {
            return null;
        }

        return new Wavelength
        {
            Min = double.Parse(parts[0], CultureInfo.InvariantCulture),
            Max = double.Parse(parts[1], CultureInfo.InvariantCulture),
            Central = double.Parse(parts[2], CultureInfo.InvariantCulture)
        };
    }
}