using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models;

/// <summary>
/// Справочник данных по солнечной активности и её влиянию на урожайность культур
/// </summary>
[MigrationCompare(CompareToType = typeof(Masofa.Common.Models.Dictionaries.SolarRadiationInfluence))]
public partial class SolarRadiationInfluence
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор региона
    /// </summary>
    public Guid? RegionId { get; set; }

    /// <summary>
    /// Идентификатор культуры
    /// </summary>
    public Guid? CropId { get; set; }

    /// <summary>
    /// Идентификатор сорта
    /// </summary>
    public Guid? VarietyId { get; set; }

    /// <summary>
    /// Начало периода вегетации
    /// </summary>
    public int? DayStart { get; set; }

    /// <summary>
    /// Окончание периода вегетации
    /// </summary>
    public int? DayEnd { get; set; }

    /// <summary>
    /// Оптимальное значение накопленной солнечной радиации
    /// </summary>
    public decimal? RadNorm { get; set; }

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
    /// Идентификатор периода вегетации
    /// </summary>
    public Guid? VegetationPeriodId { get; set; }

    //public virtual Crop? Crop { get; set; }

    //public virtual Region? Region { get; set; }

    //public virtual Variety? Variety { get; set; }

    //public virtual VegetationPeriod? VegetationPeriod { get; set; }
}
