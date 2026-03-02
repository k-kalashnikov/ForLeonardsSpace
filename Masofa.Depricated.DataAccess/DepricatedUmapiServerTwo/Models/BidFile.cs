using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedUmapiServerTwo.Models;

/// <summary>
/// Таблица для хранения информации о файлах, связанных с заявками
/// </summary>
public partial class BidFile
{
    /// <summary>
    /// Уникальный идентификатор записи
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор заявки
    /// </summary>
    public Guid BidId { get; set; }

    /// <summary>
    /// Локальный URL файла
    /// </summary>
    public string LocalUrl { get; set; } = null!;

    /// <summary>
    /// Дата создания записи (по времени Ташкент, UTC+5)
    /// </summary>
    public LocalDateTime? CreateDate { get; set; }

    /// <summary>
    /// Идентификатор пользователя, создавшего запись
    /// </summary>
    public Guid? CreateUser { get; set; }

    public virtual Bid Bid { get; set; } = null!;
}
