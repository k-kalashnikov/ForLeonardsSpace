using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedUtilesServerOne.Models;

public partial class Download
{
    public Guid Id { get; set; }

    public string? Path { get; set; }

    public LocalDateTime? StartedAt { get; set; }

    public string? Host { get; set; }

    public Guid? AppId { get; set; }

    public LocalDateTime? FinishedAt { get; set; }

    public string? FileName { get; set; }

    public long? Amount { get; set; }

    public virtual App? App { get; set; }
}
