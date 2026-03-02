using System;
using System.Collections.Generic;

namespace Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models;

[MigrationCompareAttributeWeather(CompareToType = typeof(Masofa.Common.Models.Weather.XslsUzUnputColumn),
    CompareToDbContext = typeof(Masofa.DataAccess.MasofaWeatherDbContext))]
public partial class XslsUzUnputColumn
{
    public Guid Id { get; set; }

    public string? XlsColumnName { get; set; }

    public string? DbTableName { get; set; }

    public string? DbColumnName { get; set; }
}
