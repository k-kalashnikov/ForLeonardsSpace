using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Weather;

[Table("WeatherStationsDataEx")]
public partial class WeatherStationsDataEx
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
