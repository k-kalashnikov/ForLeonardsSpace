using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedUtilesServerOne.Models;

public partial class Log
{
    public int Line { get; set; }

    public string? Host { get; set; }

    public Guid? AppId { get; set; }

    public LocalDateTime? CreatedAt { get; set; }

    public string? Message { get; set; }
}
