using System;
using System.Collections.Generic;

namespace Masofa.Depricated.DataAccess.DepricatedAuthServerOne.Models;

public partial class Permission
{
    public Guid Id { get; set; }

    public string OwnerId { get; set; } = null!;

    public int Type { get; set; }

    public string Name { get; set; } = null!;

    public int Access { get; set; }
}
