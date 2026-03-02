namespace Masofa.Common.Models.Weather
{

    public partial class AgroClimaticZoneNorm : BaseEntity
    {
        public Guid? ProviderId { get; set; }

        public Guid? AgroClimaticZoneId { get; set; }

        public int? M { get; set; }

        public int? D { get; set; }

        public double? TemperatureAvgNorm { get; set; }

        public double? PrecipitationAvgNorm { get; set; }

        public double? TemperatureMedNorm { get; set; }

        public double? PrecipitationMedNorm { get; set; }

        public double? SolarRadiationAvgNorm { get; set; }

        public double? SolarRadiationMedNorm { get; set; }
    }

    public class AgroClimaticZoneNormHistory : BaseHistoryEntity<AgroClimaticZoneNorm> { }
}