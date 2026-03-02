using System;
using System.Collections.Generic;

namespace Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models;

[MigrationCompareAttributeWeather(CompareToType = typeof(Masofa.Common.Models.Weather.WeatherStationsDataEx),
    CompareToDbContext = typeof(Masofa.DataAccess.MasofaWeatherDbContext))]
public partial class WeatherStationsDataEx
{
    public Guid Id { get; set; }

    public double? TemperatureSoil { get; set; }

    public double? TemperatureGroundLevel { get; set; }

    public double? Temperature1mAbove { get; set; }

    public double? Temperature2mUnder { get; set; }

    public double? HumiditySoil50cm { get; set; }

    public double? HumiditySoil2m { get; set; }

    public double? Temp10cmUnder { get; set; }

    public double? Temp30100cm { get; set; }

    public double? Temp1030cm { get; set; }
}
