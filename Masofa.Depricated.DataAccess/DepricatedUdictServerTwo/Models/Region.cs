using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models;

/// <summary>
/// Справочник регионов
/// </summary>
[MigrationCompare(CompareToType = typeof(Masofa.Common.Models.Dictionaries.Region))]
public partial class Region
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор региона
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// Уровень административной единицы
    /// </summary>
    public int? Level { get; set; }

    /// <summary>
    /// Условное обозначение
    /// </summary>
    public string? NameMhobt { get; set; }

    /// <summary>
    /// Наименование админ ед на узб
    /// </summary>
    public string? NameAdminUz { get; set; }

    /// <summary>
    /// Наименование админ ед на англ
    /// </summary>
    public string? NameAdminEn { get; set; }

    /// <summary>
    /// Наименование админ ед на рус
    /// </summary>
    public string? NameAdminRu { get; set; }

    /// <summary>
    /// Население
    /// </summary>
    public decimal? Population { get; set; }

    /// <summary>
    /// Публикация
    /// </summary>
    public bool Visible { get; set; }

    /// <summary>
    /// Код сортировки
    /// </summary>
    public string? OrderCode { get; set; }

    /// <summary>
    /// Дополнительная информация
    /// </summary>
    public string? ExtData { get; set; }

    /// <summary>
    /// Комментарий
    /// </summary>
    public string? Comment { get; set; }

    /// <summary>
    /// Дата создания
    /// </summary>
    public Instant CreateDate { get; set; }

    /// <summary>
    /// Автор
    /// </summary>
    public string CreateUser { get; set; } = null!;

    /// <summary>
    /// Дата обновления
    /// </summary>
    public Instant UpdateDate { get; set; }

    /// <summary>
    /// Автор обновления
    /// </summary>
    public string UpdateUser { get; set; } = null!;

    /// <summary>
    /// Наименование на узбекском
    /// </summary>
    public string? NameUz { get; set; }

    /// <summary>
    /// Наименование на английском
    /// </summary>
    public string? NameEn { get; set; }

    /// <summary>
    /// Наименование на русском
    /// </summary>
    public string? NameRu { get; set; }

    /// <summary>
    /// Идентификатор агроклиматической зоны
    /// </summary>
    public Guid? AgroclimaticZoneId { get; set; }

    /// <summary>
    /// Условное обозначение аббревиатура (en)
    /// </summary>
    public string? ShortNameEn { get; set; }

    /// <summary>
    /// Условное обозначение аббревиатура (ru)
    /// </summary>
    public string? ShortNameRu { get; set; }

    /// <summary>
    /// Условное обозначение аббревиатура (uz)
    /// </summary>
    public string? ShortNameUz { get; set; }

    /// <summary>
    /// Идентификатор типа региона
    /// </summary>
    public Guid? TypeId { get; set; }

    //public virtual AgroclimaticZone? AgroclimaticZone { get; set; }

    //public virtual ICollection<ClimaticStandard> ClimaticStandards { get; set; } = new List<ClimaticStandard>();

    //public virtual ICollection<CropPeriod> CropPeriods { get; set; } = new List<CropPeriod>();

    //public virtual ICollection<Firm> Firms { get; set; } = new List<Firm>();

    //public virtual ICollection<Person> People { get; set; } = new List<Person>();

    //public virtual ICollection<ProviderWeatherCondition> ProviderWeatherConditions { get; set; } = new List<ProviderWeatherCondition>();

    //public virtual ICollection<RegionMap> RegionMaps { get; set; } = new List<RegionMap>();

    //public virtual ICollection<SolarRadiationInfluence> SolarRadiationInfluences { get; set; } = new List<SolarRadiationInfluence>();

    //public virtual RegionType? Type { get; set; }

    //public virtual ICollection<VegetationPeriod> VegetationPeriods { get; set; } = new List<VegetationPeriod>();

    //public virtual ICollection<WeatherStation> WeatherStations { get; set; } = new List<WeatherStation>();

    //public virtual ICollection<Variety> Varieties { get; set; } = new List<Variety>();
}
