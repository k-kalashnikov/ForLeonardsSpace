using System;
using System.Collections.Generic;

namespace Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models;

[MigrationCompareAttributeWeather(CompareToType = typeof(Masofa.Common.Models.Dictionaries.WeatherCondition),
    CompareToDbContext = typeof(Masofa.DataAccess.MasofaDictionariesDbContext))]
public partial class Condition
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public Guid? ProviderId { get; set; }

    public int? ProviderCode { get; set; }
}
