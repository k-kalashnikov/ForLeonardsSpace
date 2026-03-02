using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models;

/// <summary>
/// Справочник индексов периодов развития культур
/// </summary>
[MigrationCompare(CompareToType = typeof(Masofa.Common.Models.Dictionaries.CropPeriodVegetationIndex))]
public partial class CropPeriodVegetationIndex
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор периода развития культуры
    /// </summary>
    public Guid CropPeriodId { get; set; }

    /// <summary>
    /// Идентификатор вегетационного индекса
    /// </summary>
    public Guid VegetationIndexId { get; set; }

    /// <summary>
    /// Значение индекса
    /// </summary>
    public decimal? Value { get; set; }

    /// <summary>
    /// Минимальное значение индекса
    /// </summary>
    public decimal? Min { get; set; }

    /// <summary>
    /// Максимальное значение индекса
    /// </summary>
    public decimal? Max { get; set; }

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

    //public virtual CropPeriod CropPeriod { get; set; } = null!;

    //public virtual VegetationIndex VegetationIndex { get; set; } = null!;
}
