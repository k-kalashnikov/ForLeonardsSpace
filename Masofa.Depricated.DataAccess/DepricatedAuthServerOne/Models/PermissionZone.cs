using System;
using System.Collections.Generic;

namespace Masofa.Depricated.DataAccess.DepricatedAuthServerOne.Models;

public partial class PermissionZone
{
    public Guid Id { get; set; }

    public string Module { get; set; } = null!;

    public string Zone { get; set; } = null!;

    public string Description { get; set; } = null!;
}
