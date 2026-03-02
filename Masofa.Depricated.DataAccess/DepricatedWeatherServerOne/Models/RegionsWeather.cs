using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models;

public partial class RegionsWeather
{
    public Guid Id { get; set; }

    public Guid RegionId { get; set; }

    public Guid ProviderId { get; set; }

    public Guid AgroClimaticZoneId { get; set; }

    public LocalDate Date { get; set; }

    public double? Temp { get; set; }

    public double? Precipitation { get; set; }

    public double? TempDevMax { get; set; }

    public double? PrecDevMax { get; set; }

    public double? TempDevMin { get; set; }

    public double? PrecDevMin { get; set; }
}
