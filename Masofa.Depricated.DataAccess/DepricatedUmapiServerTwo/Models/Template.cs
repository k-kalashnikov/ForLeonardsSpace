using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedUmapiServerTwo.Models;

public partial class Template
{
    public Guid Id { get; set; }

    public Guid CropId { get; set; }

    public int SchemaVersion { get; set; }

    public int ContentVersion { get; set; }

    public bool IsActive { get; set; }

    public string Data { get; set; } = null!;

    public string? Comment { get; set; }

    public Instant CreateDate { get; set; }

    public string CreateUser { get; set; } = null!;

    public Instant UpdateDate { get; set; }

    public string UpdateUser { get; set; } = null!;
}
