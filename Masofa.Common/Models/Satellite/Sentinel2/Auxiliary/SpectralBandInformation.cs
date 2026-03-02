using System.Text.Json.Serialization;
using System.Globalization;

namespace Masofa.Common.Models.Satellite.Auxiliary;

public class SpectralBandInformation
{
    [JsonPropertyName("bandId")]
    public string BandId { get; set; }

    [JsonPropertyName("PHYSICAL_GAINS")]
    public PhysicalGains PhysicalGains { get; set; }

    [JsonPropertyName("COMPRESSION_RATE")]
    public CompressionRate CompressionRate { get; set; }

    [JsonPropertyName("INTEGRATION_TIME")]
    public IntegrationTime IntegrationTime { get; set; }

    public override string ToString()
    {
        return string.Join("|",
        [
            BandId,
            PhysicalGains.ToString(),
            CompressionRate.ToString(),
            IntegrationTime.ToString()
        ]);
    }

    public static SpectralBandInformation FromString(string str)
    {
        var parts = str?.Split('|');
        if (parts?.Length < 3)
        {
            return null;
        }

        return new SpectralBandInformation
        {
            BandId = parts[0],
            PhysicalGains = PhysicalGains.FromString(parts[1]),
            CompressionRate = CompressionRate.FromString(parts[2]),
            IntegrationTime = IntegrationTime.FromString(parts[3])
        };
    }
}

public class PhysicalGains
{
    [JsonPropertyName("geometry")]
    public string Geometry { get; set; }

    [JsonPropertyName("metadataLevel")]
    public string MetadataLevel { get; set; }

    [JsonPropertyName("value")]
    public double Value { get; set; }

    public override string ToString()
    {
        return $"{Geometry}|{MetadataLevel}|{Value}";
    }

    public static PhysicalGains FromString(string str)
    {
        var parts = str?.Split('|');
        if (parts?.Length != 3) return null;

        return new PhysicalGains
        {
            Geometry = parts[0],
            MetadataLevel = parts[1],
            Value = double.Parse(parts[2], CultureInfo.InvariantCulture)
        };
    }
}

public class CompressionRate
{
    [JsonPropertyName("metadataLevel")]
    public string MetadataLevel { get; set; }

    [JsonPropertyName("value")]
    public double Value { get; set; }

    public override string ToString()
    {
        return $"{MetadataLevel}|{Value}";
    }

    public static CompressionRate FromString(string str)
    {
        var parts = str?.Split('|');
        if (parts?.Length < 2)
        {
            return null;
        }

        return new CompressionRate
        {
            MetadataLevel = parts[0],
            Value = double.Parse(parts[1], CultureInfo.InvariantCulture)
        };
    }
}

public class IntegrationTime
{
    [JsonPropertyName("metadataLevel")]
    public string MetadataLevel { get; set; }

    [JsonPropertyName("unit")]
    public string Unit { get; set; }

    [JsonPropertyName("value")]
    public double Value { get; set; }

    public override string ToString()
    {
        return $"{MetadataLevel}|{Unit}|{Value}";
    }

    public static IntegrationTime FromString(string str)
    {
        var parts = str?.Split('|');
        if (parts?.Length < 3)
        {
            return null;
        }

        return new IntegrationTime
        {
            MetadataLevel = parts[0],
            Unit = parts[1],
            Value = double.Parse(parts[2], CultureInfo.InvariantCulture)
        };
    }
}
