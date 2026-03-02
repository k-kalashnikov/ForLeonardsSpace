using System;
using System.Collections.Generic;

namespace Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models;

[MigrationCompareAttributeWeather(CompareToType = typeof(Masofa.Common.Models.Weather.ApplicationProperty),
    CompareToDbContext = typeof(Masofa.DataAccess.MasofaWeatherDbContext))]
public partial class ApplicationProperty
{
    public string Application { get; set; } = null!;

    public string Key { get; set; } = null!;

    public bool? Active { get; set; }

    public string? Value { get; set; }

    public string? Description { get; set; }
}
