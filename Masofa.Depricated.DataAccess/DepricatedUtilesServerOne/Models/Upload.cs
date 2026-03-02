using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedUtilesServerOne.Models;

public partial class Upload
{
    public Guid Id { get; set; }

    public string? Path { get; set; }

    public LocalDateTime? StartedAt { get; set; }

    public Guid? AppId { get; set; }

    public Guid? DateId { get; set; }
}
