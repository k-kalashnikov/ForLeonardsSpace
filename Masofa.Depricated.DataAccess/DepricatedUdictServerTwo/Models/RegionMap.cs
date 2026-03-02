using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models;

/// <summary>
/// Справочник карт регионов
/// </summary>
[MigrationCompare(CompareToType = typeof(Masofa.Common.Models.Dictionaries.RegionMap))]
public partial class RegionMap
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Широта
    /// </summary>
    public decimal? Lat { get; set; }

    /// <summary>
    /// Долгота
    /// </summary>
    public decimal? Lng { get; set; }

    /// <summary>
    /// По оси Х
    /// </summary>
    public int? MozaikX { get; set; }

    /// <summary>
    /// По оси Y
    /// </summary>
    public int? MozaikY { get; set; }

    /// <summary>
    /// Полигон
    /// </summary>
    public Geometry? Polygon { get; set; }

    /// <summary>
    /// Полигон в виде текста (JSON)
    /// </summary>
    public string? PolygonAsText { get; set; }

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
    /// Начало периода актуальности
    /// </summary>
    public Instant? ActiveFrom { get; set; }

    /// <summary>
    /// Окончание периода актуальности
    /// </summary>
    public Instant? ActiveTo { get; set; }

    /// <summary>
    /// Полезная сельскохозяйственная площадь региона
    /// </summary>
    public decimal? AgriculturalArea { get; set; }

    /// <summary>
    /// Идентификатор региона
    /// </summary>
    public Guid RegionId { get; set; }

    /// <summary>
    /// Общая площадь региона
    /// </summary>
    public decimal? TotalArea { get; set; }

    //public virtual Region Region { get; set; } = null!;
}
