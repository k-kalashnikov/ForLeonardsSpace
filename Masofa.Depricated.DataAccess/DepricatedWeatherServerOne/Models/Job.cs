using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models;

[MigrationCompareAttributeWeather(CompareToType = typeof(Masofa.Common.Models.Weather.Job),
    CompareToDbContext = typeof(Masofa.DataAccess.MasofaWeatherDbContext))]
public partial class Job
{
    public Guid Id { get; set; }

    public LocalDateTime? Date { get; set; }

    public string? Action { get; set; }

    public Guid? ProviderId { get; set; }

    public Guid? JobStatusId { get; set; }

    public string? Application { get; set; }

    public string? Result { get; set; }

    public string? Path { get; set; }
}
