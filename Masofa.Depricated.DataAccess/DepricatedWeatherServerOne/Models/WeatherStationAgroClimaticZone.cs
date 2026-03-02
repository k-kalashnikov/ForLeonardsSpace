using System;
using System.Collections.Generic;

namespace Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models;

[MigrationCompareAttributeWeather(CompareToType = typeof(Masofa.Common.Models.Weather.WeatherStationAgroClimaticZone),
    CompareToDbContext = typeof(Masofa.DataAccess.MasofaWeatherDbContext))]
public partial class WeatherStationAgroClimaticZone
{
    public Guid WeatherStationId { get; set; }

    public Guid AgroClimaticZonesId { get; set; }
}
