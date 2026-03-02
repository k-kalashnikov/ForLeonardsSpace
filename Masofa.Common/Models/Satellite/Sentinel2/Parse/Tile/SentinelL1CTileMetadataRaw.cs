using System.Text.Json.Serialization;
using Masofa.Common.Models.Satellite.Auxiliary;

namespace Masofa.Common.Models.Satellite.Parse.Sentinel.Tile
{
    /// <summary>
    /// JSON модель для L1C Tile Metadata
    /// </summary>
    public class SentinelL1CTileMetadataRaw
    {
        [JsonPropertyName("n1")]
        public string N1 { get; set; }

        [JsonPropertyName("xsi")]
        public string Xsi { get; set; }

        [JsonPropertyName("schemaLocation")]
        public string SchemaLocation { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("General_Info")]
        public GeneralInfo GeneralInfo { get; set; }

        [JsonPropertyName("Geometric_Info")]
        public GeometricInfo GeometricInfo { get; set; }

        [JsonPropertyName("Quality_Indicators_Info")]
        public QualityIndicatorsInfo QualityIndicatorsInfo { get; set; }

        /// <summary>
        /// Удобный геттер для получения SensingTime
        /// </summary>
        public string SensingTime => GeneralInfo?.SensingTime.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Секция General Info
    /// </summary>
    public class GeneralInfo
    {
        [JsonPropertyName("TILE_ID")]
        public string TileId { get; set; }

        [JsonPropertyName("DATASTRIP_ID")]
        public string DatastripId { get; set; }

        [JsonPropertyName("DOWNLINK_PRIORITY")]
        public string DownlinkPriority { get; set; }

        [JsonPropertyName("SENSING_TIME")]
        public DateTime SensingTime { get; set; }

        [JsonPropertyName("Archiving_Info")]
        public ArchivingInfo ArchivingInfo { get; set; }
    }

    /// <summary>
    /// Секция Archiving Info
    /// </summary>
    public class ArchivingInfo
    {
        [JsonPropertyName("metadataLevel")]
        public string MetadataLevel { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("ARCHIVING_CENTRE")]
        public string ArchivingCentre { get; set; }

        [JsonPropertyName("ARCHIVING_TIME")]
        public DateTime ArchivingTime { get; set; }
    }

    /// <summary>
    /// Секция Geometric Info
    /// </summary>
    public class GeometricInfo
    {
        [JsonPropertyName("Tile_Geocoding")]
        public TileGeocoding TileGeocoding { get; set; }

        [JsonPropertyName("Tile_Angles")]
        public TileAngles TileAngles { get; set; }
    }

    /// <summary>
    /// Секция Tile Geocoding
    /// </summary>
    public class TileGeocoding
    {
        [JsonPropertyName("metadataLevel")]
        public string MetadataLevel { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("HORIZONTAL_CS_NAME")]
        public string HorizontalCsName { get; set; }

        [JsonPropertyName("HORIZONTAL_CS_CODE")]
        public string HorizontalCsCode { get; set; }

        [JsonPropertyName("Size")]
        public Size[] Sizes { get; set; }

        [JsonPropertyName("Geoposition")]
        public Geoposition[] Geopositions { get; set; }
    }

    /// <summary>
    /// Секция Size
    /// </summary>
    public class Size
    {
        [JsonPropertyName("resolution")]
        public string Resolution { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("NROWS")]
        public int Nrows { get; set; }

        [JsonPropertyName("NCOLS")]
        public int Ncols { get; set; }

        public override string ToString()
        {
            return string.Join("|",
            [
                Resolution ?? "0",
                Nrows.ToString(),
                Ncols.ToString()
            ]);
        }
    }

    /// <summary>
    /// Секция Geoposition
    /// </summary>
    public class Geoposition
    {
        [JsonPropertyName("resolution")]
        public string Resolution { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("ULX")]
        public int Ulx { get; set; }

        [JsonPropertyName("ULY")]
        public int Uly { get; set; }

        [JsonPropertyName("XDIM")]
        public int Xdim { get; set; }

        [JsonPropertyName("YDIM")]
        public int Ydim { get; set; }

        public override string ToString()
        {
            return string.Join("|",
            [
                Resolution ?? "0",
                Ulx.ToString(),
                Uly.ToString(),
                Xdim.ToString(),
                Ydim.ToString()
            ]);
        }
    }

    /// <summary>
    /// Секция Tile Angles
    /// </summary>
    public class TileAngles
    {
        [JsonPropertyName("metadataLevel")]
        public string MetadataLevel { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("Sun_Angles_Grid")]
        public SunAnglesGrid SunAnglesGrid { get; set; }

        [JsonPropertyName("Mean_Sun_Angle")]
        public MeanSunAngle MeanSunAngle { get; set; }

        [JsonPropertyName("Viewing_Incidence_Angles_Grids")]
        public ViewingIncidenceAnglesGrids[] ViewingIncidenceAnglesGrids { get; set; }

        [JsonPropertyName("Mean_Viewing_Incidence_Angle_List")]
        public MeanViewingIncidenceAngleList MeanViewingIncidenceAngleList { get; set; }
    }

    /// <summary>
    /// Секция Sun Angles Grid
    /// </summary>
    public class SunAnglesGrid
    {
        [JsonPropertyName("Zenith")]
        public Zenith Zenith { get; set; }

        [JsonPropertyName("Azimuth")]
        public Azimuth Azimuth { get; set; }
    }

    /// <summary>
    /// Секция Zenith
    /// </summary>
    public class Zenith
    {
        [JsonPropertyName("COL_STEP")]
        public int ColStep { get; set; }

        [JsonPropertyName("ROW_STEP")]
        public int RowStep { get; set; }

        [JsonPropertyName("Values_List")]
        public ValuesList ValuesList { get; set; }
    }

    /// <summary>
    /// Секция Values List
    /// </summary>
    public class ValuesList
    {
        [JsonPropertyName("VALUES")]
        public string[] Values { get; set; }
    }

    /// <summary>
    /// Секция Azimuth
    /// </summary>
    public class Azimuth
    {
        [JsonPropertyName("COL_STEP")]
        public int ColStep { get; set; }

        [JsonPropertyName("ROW_STEP")]
        public int RowStep { get; set; }

        [JsonPropertyName("Values_List")]
        public ValuesList1 ValuesList { get; set; }
    }

    /// <summary>
    /// Секция Values List 1
    /// </summary>
    public class ValuesList1
    {
        [JsonPropertyName("VALUES")]
        public string[] Values { get; set; }
    }

    /// <summary>
    /// Секция Mean Sun Angle
    /// </summary>
    public class MeanSunAngle
    {
        [JsonPropertyName("ZENITH_ANGLE")]
        public string ZenithAngle { get; set; }

        [JsonPropertyName("AZIMUTH_ANGLE")]
        public string AzimuthAngle { get; set; }
    }

    /// <summary>
    /// Секция Mean Viewing Incidence Angle List
    /// </summary>
    public class MeanViewingIncidenceAngleList
    {
        [JsonPropertyName("Mean_Viewing_Incidence_Angle")]
        public MeanViewingIncidenceAngle[] MeanViewingIncidenceAngles { get; set; }
    }

    /// <summary>
    /// Секция Mean Viewing Incidence Angle
    /// </summary>
    public class MeanViewingIncidenceAngle
    {
        [JsonPropertyName("bandId")]
        public string BandId { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("ZENITH_ANGLE")]
        public string ZenithAngle { get; set; }

        [JsonPropertyName("AZIMUTH_ANGLE")]
        public string AzimuthAngle { get; set; }

        public override string ToString()
        {
            return string.Join("|",
            [
                BandId ?? string.Empty,
                Value ?? string.Empty,
                ZenithAngle ?? string.Empty,
                AzimuthAngle ?? string.Empty
            ]);
        }
    }

    /// <summary>
    /// Секция Viewing Incidence Angles Grids
    /// </summary>
    public class ViewingIncidenceAnglesGrids
    {
        [JsonPropertyName("bandId")]
        public string BandId { get; set; }

        [JsonPropertyName("detectorId")]
        public string DetectorId { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("Zenith")]
        public Zenith1 Zenith { get; set; }

        [JsonPropertyName("Azimuth")]
        public Azimuth1 Azimuth { get; set; }

        public override string ToString()
        {
            var zenithValues = Zenith?.ValuesList?.Values != null ? string.Join(",", Zenith.ValuesList.Values) : string.Empty;
            var azimuthValues = Azimuth?.ValuesList?.Values != null ? string.Join(",", Azimuth.ValuesList.Values) : string.Empty;

            return string.Join("|",
            [
                BandId ?? string.Empty,
                DetectorId ?? string.Empty,
                Value ?? string.Empty,
                Zenith?.ColStep.ToString() ?? "0",
                Zenith?.RowStep.ToString() ?? "0",
                zenithValues,
                Azimuth?.ColStep.ToString() ?? "0",
                Azimuth?.RowStep.ToString() ?? "0",
                azimuthValues
            ]);
        }
    }

    /// <summary>
    /// Секция Zenith 1
    /// </summary>
    public class Zenith1
    {
        [JsonPropertyName("COL_STEP")]
        public int ColStep { get; set; }

        [JsonPropertyName("ROW_STEP")]
        public int RowStep { get; set; }

        [JsonPropertyName("Values_List")]
        public ValuesList2 ValuesList { get; set; }
    }

    /// <summary>
    /// Секция Values List 2
    /// </summary>
    public class ValuesList2
    {
        [JsonPropertyName("VALUES")]
        public string[] Values { get; set; }
    }

    /// <summary>
    /// Секция Azimuth 1
    /// </summary>
    public class Azimuth1
    {
        [JsonPropertyName("COL_STEP")]
        public int ColStep { get; set; }

        [JsonPropertyName("ROW_STEP")]
        public int RowStep { get; set; }

        [JsonPropertyName("Values_List")]
        public ValuesList3 ValuesList { get; set; }
    }

    /// <summary>
    /// Секция Values List 3
    /// </summary>
    public class ValuesList3
    {
        [JsonPropertyName("VALUES")]
        public string[] Values { get; set; }
    }

    /// <summary>
    /// Секция Quality Indicators Info
    /// </summary>
    public class QualityIndicatorsInfo
    {
        [JsonPropertyName("metadataLevel")]
        public string MetadataLevel { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("Image_Content_QI")]
        public ImageContentQI ImageContentQI { get; set; }

        [JsonPropertyName("Pixel_Level_QI")]
        public PixelLevelQI PixelLevelQI { get; set; }

        [JsonPropertyName("PVI_FILENAME")]
        public string PviFilename { get; set; }
    }

    /// <summary>
    /// Секция Image Content QI
    /// </summary>
    public class ImageContentQI
    {
        [JsonPropertyName("CLOUDY_PIXEL_PERCENTAGE")]
        public string CloudyPixelPercentage { get; set; }

        [JsonPropertyName("DEGRADED_MSI_DATA_PERCENTAGE")]
        public string DegradedMsiDataPercentage { get; set; }

        [JsonPropertyName("SNOW_PIXEL_PERCENTAGE")]
        public string SnowPixelPercentage { get; set; }
    }

    /// <summary>
    /// Секция Pixel Level QI
    /// </summary>
    public class PixelLevelQI
    {
        [JsonPropertyName("geometry")]
        public string Geometry { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("MASK_FILENAME")]
        public string[] MaskFilenames { get; set; }

        public override string ToString()
        {
            var masks = MaskFilenames != null
                ? string.Join(",", MaskFilenames)
                : string.Empty;

            return $"{Geometry}#{masks}";
        }
    }
} 