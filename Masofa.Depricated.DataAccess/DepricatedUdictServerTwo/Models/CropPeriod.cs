using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models;

/// <summary>
/// Справочник периодов развития культур
/// </summary>
[MigrationCompare(CompareToType = typeof(Masofa.Common.Models.Dictionaries.CropPeriod))]
public partial class CropPeriod
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
    /// День начала периода
    /// </summary>
    public int? DayStart { get; set; }

    /// <summary>
    /// День окончания периода
    /// </summary>
    public int? DayEnd { get; set; }

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

    //public virtual Crop? Crop { get; set; }

    //public virtual ICollection<CropPeriodVegetationIndex> CropPeriodVegetationIndices { get; set; } = new List<CropPeriodVegetationIndex>();

    //public virtual Region? Region { get; set; }

    //public virtual Variety? Variety { get; set; }
}
