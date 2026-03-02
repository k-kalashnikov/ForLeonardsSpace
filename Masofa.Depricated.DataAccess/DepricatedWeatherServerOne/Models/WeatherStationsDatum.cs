using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models;

[MigrationCompareAttributeWeather(CompareToType = typeof(Masofa.Common.Models.Weather.WeatherStationsDatum),
    CompareToDbContext = typeof(Masofa.DataAccess.MasofaWeatherDbContext))]
public partial class WeatherStationsDatum
{
    public Guid Id { get; set; }

    public Guid? WeatherStationId { get; set; }

    public LocalDateTime? Date { get; set; }

    public double? Temperature { get; set; }

    public double? TemperatureMax { get; set; }

    public double? TemperatureMin { get; set; }

    public double? Precipitation { get; set; }

    public double? WindSpeed { get; set; }

    public double? WindSpeedMin { get; set; }

    public double? WindSpeedMax { get; set; }

    public double? WindDirection { get; set; }

    public double? Windchill { get; set; }

    public double? CloudCover { get; set; }

    public double? RelativeHumidity { get; set; }

    public int? ConditionCode { get; set; }

    public double? SolarRadiation { get; set; }

    public double? DewPoint { get; set; }

    public double? HumidityMin { get; set; }

    public double? HumidityMax { get; set; }
}
