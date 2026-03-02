using System;
using System.Collections.Generic;

namespace Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models;

[MigrationCompareAttributeWeather(CompareToType = typeof(Masofa.Common.Models.Weather.RegionsAgroClimaticZone),
    CompareToDbContext = typeof(Masofa.DataAccess.MasofaWeatherDbContext))]
public partial class RegionsAgroClimaticZone
{
    public Guid RegionId { get; set; }

    public Guid AgroClimaticZonesId { get; set; }
}
