using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models;

[MigrationCompareAttributeWeather(CompareToType = typeof(Masofa.Common.Models.Weather.Alert),
    CompareToDbContext = typeof(Masofa.DataAccess.MasofaWeatherDbContext))]
public partial class Alert
{
    public Guid Id { get; set; }

    public Guid RegionId { get; set; }

    public int? TypeId { get; set; }

    public Guid ProviderId { get; set; }

    public LocalDate Date { get; set; }

    public double? Value { get; set; }

    public Guid? AgroClimaticZonesId { get; set; }
}
