using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models;

/// <summary>
/// Справочник вегетационных индексов
/// </summary>
[MigrationCompare(CompareToType = typeof(Masofa.Common.Models.Dictionaries.VegetationIndex))]
public partial class VegetationIndex
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Название индекса
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Описание на английском
    /// </summary>
    public string? DescriptionEn { get; set; }

    /// <summary>
    /// Описание на узбекском
    /// </summary>
    public string? DescriptionUz { get; set; }

    /// <summary>
    /// Описание на русском
    /// </summary>
    public string? DescriptionRu { get; set; }

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

    //public virtual ICollection<CropPeriodVegetationIndex> CropPeriodVegetationIndices { get; set; } = new List<CropPeriodVegetationIndex>();
}
