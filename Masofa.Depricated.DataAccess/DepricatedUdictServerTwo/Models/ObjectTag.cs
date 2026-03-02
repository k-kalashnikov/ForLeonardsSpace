using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models;

/// <summary>
/// Файлы, прикрепленные к записям справочников
/// </summary>
public partial class ObjectTag
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор связанной записи справочника
    /// </summary>
    public Guid ObjectId { get; set; }

    public Guid TagId { get; set; }

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

    public virtual Tag Tag { get; set; } = null!;
}
