using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Weather;

public partial class WeatherStation
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
