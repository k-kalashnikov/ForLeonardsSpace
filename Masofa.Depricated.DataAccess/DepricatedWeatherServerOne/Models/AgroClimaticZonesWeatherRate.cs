using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models;

[MigrationCompareAttributeWeather(CompareToType = typeof(Masofa.Common.Models.Weather.AgroClimaticZonesWeatherRate),
    CompareToDbContext = typeof(Masofa.DataAccess.MasofaWeatherDbContext))]
public partial class AgroClimaticZonesWeatherRate
{
    public Guid Id { get; set; }

    public LocalDate? Date { get; set; }

    public double? TempRate { get; set; }

    public double? PrecRate { get; set; }
}
