using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedUtilesServerOne.Models;

public partial class LoadTask
{
    public Guid Id { get; set; }

    public Guid Uid { get; set; }

    public LocalDateTime? FieldModified { get; set; }

    public LocalDateTime? CreatedAt { get; set; }

    public LocalDate? StartDate { get; set; }

    public int? Status { get; set; }

    public Guid? AppId { get; set; }

    public LocalDateTime? StartedAt { get; set; }

    public LocalDateTime? FinishedAt { get; set; }

    public string? Result { get; set; }

    public virtual App? App { get; set; }

    public virtual State? StatusNavigation { get; set; }
}
