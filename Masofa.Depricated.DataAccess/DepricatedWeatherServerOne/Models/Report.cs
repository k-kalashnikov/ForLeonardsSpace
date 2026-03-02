using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models;

[MigrationCompareAttributeWeather(CompareToType = typeof(Masofa.Common.Models.Weather.Report),
    CompareToDbContext = typeof(Masofa.DataAccess.MasofaWeatherDbContext))]
public partial class Report
{
    public Guid Id { get; set; }

    public Guid? ReportType { get; set; }

    public string? Name { get; set; }

    public LocalDate? UpdateDate { get; set; }

    public string? Link { get; set; }

    public string? Description { get; set; }

    public string? SourceQuery { get; set; }
}
