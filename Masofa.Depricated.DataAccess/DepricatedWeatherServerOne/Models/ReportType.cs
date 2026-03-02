using System;
using System.Collections.Generic;

namespace Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models;

[MigrationCompareAttributeWeather(CompareToType = typeof(Masofa.Common.Models.Dictionaries.WeatherReportType),
    CompareToDbContext = typeof(Masofa.DataAccess.MasofaDictionariesDbContext))]
public partial class ReportType
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? NameEn { get; set; }

    public string? NameUz { get; set; }

    public string? Css { get; set; }
}
