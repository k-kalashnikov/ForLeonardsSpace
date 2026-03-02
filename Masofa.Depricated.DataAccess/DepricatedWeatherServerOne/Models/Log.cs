using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models;

[MigrationCompareAttributeWeather(CompareToType = typeof(Masofa.Common.Models.Weather.Log),
    CompareToDbContext = typeof(Masofa.DataAccess.MasofaWeatherDbContext))]
public partial class Log
{
    public Guid Id { get; set; }

    public LocalDateTime? Date { get; set; }

    public Guid? JobId { get; set; }

    public string? JobStatus { get; set; }

    public Guid? ProviderId { get; set; }

    public string? Details { get; set; }

    public double? ContentSize { get; set; }

    public string? UserInfo { get; set; }
}
