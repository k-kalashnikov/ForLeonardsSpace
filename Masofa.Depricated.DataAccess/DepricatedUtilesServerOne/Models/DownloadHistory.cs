using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedUtilesServerOne.Models;

public partial class DownloadHistory
{
    public Guid Id { get; set; }

    public long? Amount { get; set; }

    public string? Path { get; set; }

    public LocalDateTime? Date { get; set; }

    public Guid? AppId { get; set; }
}
