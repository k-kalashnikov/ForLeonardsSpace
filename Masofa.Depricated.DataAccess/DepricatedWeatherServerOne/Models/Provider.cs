using System;
using System.Collections.Generic;

namespace Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models;

[MigrationCompareAttributeWeather(CompareToType = typeof(Masofa.Common.Models.Dictionaries.WeatherProvider),
    CompareToDbContext = typeof(Masofa.DataAccess.MasofaDictionariesDbContext))]
public partial class Provider
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public double? Z { get; set; }

    public int? FrequencyId { get; set; }

    public bool? Active { get; set; }

    public bool? Editable { get; set; }
}
