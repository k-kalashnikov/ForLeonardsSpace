using Masofa.Common.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Weather;

[PartitionedTable]
[Table("WeatherStationsDatum")]
public partial class WeatherStationsDatum
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public Guid? WeatherStationId { get; set; }

    public DateTime Date { get; set; }

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
