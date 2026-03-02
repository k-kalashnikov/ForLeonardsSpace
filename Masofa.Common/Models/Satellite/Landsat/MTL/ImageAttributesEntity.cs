using Masofa.Common.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Satellite.Landsat
{
    [Table("ImageAttributes")]
    [PartitionedTable]
    public class ImageAttributesEntity : BaseEntity
    {
        public string SpacecraftId { get; set; }
        public string SensorId { get; set; }
        public string WrsType { get; set; }
        public string WrsPath { get; set; }
        public string WrsRow { get; set; }
        public string NadirOffnadir { get; set; }
        public string TargetWrsPath { get; set; }
        public string TargetWrsRow { get; set; }
        public string DateAcquired { get; set; }
        public string SceneCenterTime { get; set; }
        public string StationId { get; set; }
        public string CloudCover { get; set; }
        public string CloudCoverLand { get; set; }
        public string ImageQualityOli { get; set; }
        public string ImageQualityTirs { get; set; }
        public string SaturationBand1 { get; set; }
        public string SaturationBand2 { get; set; }
        public string SaturationBand3 { get; set; }
        public string SaturationBand4 { get; set; }
        public string SaturationBand5 { get; set; }
        public string SaturationBand6 { get; set; }
        public string SaturationBand7 { get; set; }
        public string SaturationBand8 { get; set; }
        public string SaturationBand9 { get; set; }
        public string RollAngle { get; set; }
        public string SunAzimuth { get; set; }
        public string SunElevation { get; set; }
        public string EarthSunDistance { get; set; }
    }

    public class ImageAttributesEntityHistory : BaseHistoryEntity<ImageAttributesEntity> { }
}
