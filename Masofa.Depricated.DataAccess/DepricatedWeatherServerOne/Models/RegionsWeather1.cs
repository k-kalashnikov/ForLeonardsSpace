using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models;

[MigrationCompareAttributeWeather(CompareToType = typeof(Masofa.Common.Models.Weather.RegionsWeather),
    CompareToDbContext = typeof(Masofa.DataAccess.MasofaWeatherDbContext))]
public partial class RegionsWeather1
{
    public Guid RegionId { get; set; }

    public Guid ProviderId { get; set; }

    public LocalDate Date { get; set; }

    public double? Temp { get; set; }

    public double? Precipitation { get; set; }

    public double? TempDev { get; set; }

    public double? PrecDev { get; set; }
}
