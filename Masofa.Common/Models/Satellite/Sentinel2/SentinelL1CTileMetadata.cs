using Masofa.Common.Attributes;
using Masofa.Common.Models.Satellite.Auxiliary;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Satellite.Sentinel
{

    [PartitionedTable]
    public class SentinelL1CTileMetadata : BaseEntity
    {
        // General Info
        public string TileId { get; set; }
        public string DatastripId { get; set; }
        public string DownlinkPriority { get; set; }
        public DateTime SensingTime { get; set; }

        // Archiving Info
        public string ArchivingCentre { get; set; }
        public DateTime ArchivingTime { get; set; }

        // Geometric Info
        // Tile Geocoding
        public string HorizontalCSName { get; set; }

        public string HorizontalCSCode { get; set; }

        [NotMapped]
        public List<TileSize> Sizes { get; set; }
        public string SizesRaw
        {
            get => Sizes == null ? null : string.Join("~", Sizes.Select(x => x.ToString()));
            set => Sizes = string.IsNullOrEmpty(value) ? null : value.Split('~').Select(TileSize.FromString).Where(x => x != null).ToList();
        }

        [NotMapped]
        public List<TileGeoposition> Geopositions { get; set; }
        public string GeopositionsRaw
        {
            get => Geopositions == null ? null : string.Join("~", Geopositions.Select(x => x.ToString()));
            set => Geopositions = string.IsNullOrEmpty(value) ? null : value.Split('~').Select(TileGeoposition.FromString).Where(x => x != null).ToList();
        }

        // Tile Angles
        // SunAnglesGrid
        public double SunAnglesZenithColStepValue { get; set; }
        public double SunAnglesZenithRowStepValue { get; set; }
        public List<string> SunAnglesZenithValuesList { get; set; }
        public double SunAnglesAzimuthColStepValue { get; set; }
        public double SunAnglesAzimuthRowStepValue { get; set; }
        public List<string> SunAnglesAzimuthValuesList { get; set; }

        // MeanSunAngle
        public string MeanSunAngleZenithAngle { get; set; }
        public string MeanSunAngleAzimuthAngle { get; set; }
        [NotMapped]
        public List<ViewingIncidenceAnglesGrid> ViewingIncidenceAnglesGrids { get; set; }

        public string ViewingIncidenceAnglesGridsRaw
        {
            get => ViewingIncidenceAnglesGrids == null ? null : string.Join("~", ViewingIncidenceAnglesGrids.Select(x => x.ToString()));
            set => ViewingIncidenceAnglesGrids = string.IsNullOrEmpty(value)
                ? null
                : value.Split('~').Select(ViewingIncidenceAnglesGrid.FromString).Where(x => x != null).ToList();
        }
        [NotMapped]
        public List<MeanViewingIncidenceAngle> MeanViewingIncidenceAngleList { get; set; }
        public string MeanViewingIncidenceAngleListRaw
        {
            get => MeanViewingIncidenceAngleList == null ? null : string.Join("~", MeanViewingIncidenceAngleList.Select(x => x.ToString()));
            set => MeanViewingIncidenceAngleList = string.IsNullOrEmpty(value) ? null : value.Split('~').Select(MeanViewingIncidenceAngle.FromString).Where(x => x != null).ToList();
        }

        // Quality Indicators Info
        public double CloudyPixelPercentage { get; set; }
        public double DegradedDataPercentage { get; set; }
        public double SnowPixelPercentage { get; set; }
        [NotMapped]
        public List<PixelLevelQI> PixelLevelQI { get; set; }
        public string PixelLevelQIRaw
        {
            get => PixelLevelQI == null ? null : string.Join("~", PixelLevelQI.Select(x => x.ToString()));
            set => PixelLevelQI = string.IsNullOrEmpty(value) ? null : value.Split('~').Select(Auxiliary.PixelLevelQI.FromString).Where(x => x != null).ToList();
        }
    }

    public class SentinelL1CTileMetadataHistory : BaseHistoryEntity<SentinelL1CTileMetadata> { }
}