using System;
using System.Collections.Generic;

namespace Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models;

[MigrationCompareAttributeWeather(CompareToType = typeof(Masofa.Common.Models.Dictionaries.WeatherImageType),
    CompareToDbContext = typeof(Masofa.DataAccess.MasofaDictionariesDbContext))]
public partial class ImageType
{
    public Guid Id { get; set; }

    public int? Code { get; set; }

    public string? Name { get; set; }

    public string? NameEn { get; set; }

    public string? NameUz { get; set; }
}
