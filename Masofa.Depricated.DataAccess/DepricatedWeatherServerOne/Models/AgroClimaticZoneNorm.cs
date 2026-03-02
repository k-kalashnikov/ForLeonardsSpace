using System;
using System.Collections.Generic;

namespace Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models;

[MigrationCompareAttributeWeather(CompareToType = typeof(Masofa.Common.Models.Weather.AgroClimaticZoneNorm),
    CompareToDbContext = typeof(Masofa.DataAccess.MasofaWeatherDbContext))]
public partial class AgroClimaticZoneNorm
{
    public Guid Id { get; set; }

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
