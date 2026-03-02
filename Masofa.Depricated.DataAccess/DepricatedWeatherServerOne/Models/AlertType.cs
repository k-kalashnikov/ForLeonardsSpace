using System;
using System.Collections.Generic;

namespace Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models;

[MigrationCompareAttributeWeather(CompareToType = typeof(Masofa.Common.Models.Dictionaries.WeatherAlertType),
    CompareToDbContext = typeof(Masofa.DataAccess.MasofaDictionariesDbContext))]
public partial class AlertType
{
    public int Id { get; set; }

    public int? Type { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public double? Value { get; set; }

    public string? NameEn { get; set; }

    public string? NameUz { get; set; }

    public string? DescriptionEn { get; set; }

    public string? DescriptionUz { get; set; }

    public string? FieldName { get; set; }
}
