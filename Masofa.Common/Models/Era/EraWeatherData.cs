using Masofa.Common.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Era
{
    [Table("EraWeatherData")]
    [PartitionedTable]
    public class EraWeatherData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public DateTime? OriginalDateTimeUtc { get; set; }

        public double? Temperature { get; set; }

        public double? RelativeHumidity { get; set; }

        public double? DewPoint { get; set; }

        public double? Precipitation { get; set; }

        public double? CloudCover { get; set; }

        public double? WindSpeed { get; set; }

        public double? WindDirection { get; set; }

        public double? GroundTemperature { get; set; }

        public double? SoilTemperature { get; set; }

        public int? ConditionIds { get; set; }

        public double? SoilHumidity50cm { get; set; }

        public double? SoilHumidity2m { get; set; }
        public double? SolarRadiation { get; set; }

        public Guid EraWeatherStationId { get; set; }
    }
}
