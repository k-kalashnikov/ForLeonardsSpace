using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedUtilesServerOne.Models;

public partial class Country
{
    public string Name { get; set; } = null!;

    public Guid Uid { get; set; }

    public string? Region { get; set; }

    public LocalDateTime? CreatedAt { get; set; }

    public int? Status { get; set; }

    public LocalDate? UpdateDate { get; set; }

    public Guid? AppId { get; set; }

    public LocalDate? StartDate { get; set; }

    public bool? Enabled { get; set; }

    public string? SimpleRegion { get; set; }

    public virtual App? App { get; set; }
}
