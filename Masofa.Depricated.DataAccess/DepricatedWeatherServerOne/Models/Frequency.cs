using System;
using System.Collections.Generic;

namespace Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models;

[MigrationCompareAttributeWeather(CompareToType = typeof(Masofa.Common.Models.Dictionaries.WeatherFrequency),
    CompareToDbContext = typeof(Masofa.DataAccess.MasofaDictionariesDbContext))]
public partial class Frequency
{
    public int Id { get; set; }

    public string? Name { get; set; }
}
