using System;
using System.Collections.Generic;

namespace Masofa.Depricated.DataAccess.DepricatedUmapiServerTwo.Models;

public partial class Templ
{
    public Guid Id { get; set; }

    public Guid CropId { get; set; }

    public string Version { get; set; } = null!;

    public string? Lang { get; set; }

    public bool Active { get; set; }
}
