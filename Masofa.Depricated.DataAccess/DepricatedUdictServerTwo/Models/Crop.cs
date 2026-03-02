using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models;

/// <summary>
/// Справочник культур
/// </summary>
[MigrationCompare(CompareToType = typeof(Masofa.Common.Models.Dictionaries.Crop))]
public partial class Crop
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Наименование на латыни
    /// </summary>
    public string? NameLa { get; set; }

    /// <summary>
    /// Признак осуществления мониторинга
    /// </summary>
    public bool? IsMonitoring { get; set; }

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

    //public virtual ICollection<AgrotechnicalMeasure> AgrotechnicalMeasures { get; set; } = new List<AgrotechnicalMeasure>();

    //public virtual ICollection<CropPeriod> CropPeriods { get; set; } = new List<CropPeriod>();

    //public virtual ICollection<ExperimentalFarmingMethod> ExperimentalFarmingMethods { get; set; } = new List<ExperimentalFarmingMethod>();

    //public virtual ICollection<SolarRadiationInfluence> SolarRadiationInfluences { get; set; } = new List<SolarRadiationInfluence>();

    //public virtual ICollection<Variety> Varieties { get; set; } = new List<Variety>();
}
