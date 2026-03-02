using System.Text.Json.Serialization;

namespace Masofa.Common.Models.Satellite.Auxiliary;

public class ViewingIncidenceAnglesGrid
{
    [JsonPropertyName("bandId")]
    public int BandId { get; set; }

    [JsonPropertyName("detectorId")]
    public int DetectorId { get; set; }

    [JsonPropertyName("Zenith")]
    public AngleGrid Zenith { get; set; }

    [JsonPropertyName("Azimuth")]
    public AngleGrid Azimuth { get; set; }

    public override string ToString()
    {
        var zenithValues = Zenith?.ValuesList?.Values != null ? string.Join(",", Zenith.ValuesList.Values) : string.Empty;
        var azimuthValues = Azimuth?.ValuesList?.Values != null ? string.Join(",", Azimuth.ValuesList.Values) : string.Empty;

        return string.Join("|",
        [
            BandId.ToString(),
            DetectorId.ToString(),
            Zenith?.ColStep.ToString() ?? "0",
            Zenith?.RowStep.ToString() ?? "0",
            zenithValues,
            Azimuth?.ColStep.ToString() ?? "0",
            Azimuth?.RowStep.ToString() ?? "0",
            azimuthValues
        ]);
    }

    public static ViewingIncidenceAnglesGrid FromString(string str)
    {
        var parts = str?.Split('|');
        if (parts?.Length != 9)
        {
            return null;
        }

        return new ViewingIncidenceAnglesGrid
        {
            BandId = int.Parse(parts[0]),
            DetectorId = int.Parse(parts[1]),
                         Zenith = new AngleGrid
             {
                 ColStep = int.Parse(parts[3]),
                 RowStep = int.Parse(parts[4]),
                 ValuesList = new ValuesList { Values = parts[4].Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() }
             },
             Azimuth = new AngleGrid
             {
                 ColStep = int.Parse(parts[6]),
                 RowStep = int.Parse(parts[7]),
                 ValuesList = new ValuesList { Values = parts[8].Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() }
             }
        };
    }
}
