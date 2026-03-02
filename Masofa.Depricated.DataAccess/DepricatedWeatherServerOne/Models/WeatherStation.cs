using System;
using System.Collections.Generic;

namespace Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models;

[MigrationCompareAttributeWeather(CompareToType = typeof(Masofa.Common.Models.Weather.WeatherStation),
    CompareToDbContext = typeof(Masofa.DataAccess.MasofaWeatherDbContext))]
public partial class WeatherStation
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public Guid? ProviderId { get; set; }

    public double? Lat { get; set; }

    public double? Lon { get; set; }

    public int? X { get; set; }

    public int? Y { get; set; }

    public string? Application { get; set; }

    public string? Code { get; set; }
}
