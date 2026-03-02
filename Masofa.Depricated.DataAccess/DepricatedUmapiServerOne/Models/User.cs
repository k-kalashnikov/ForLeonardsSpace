using System;
using System.Collections.Generic;

namespace Masofa.Depricated.DataAccess.DepricatedUmapiServerOne.Models;

public partial class User
{
    public Guid Id { get; set; }

    public Guid? ParentId { get; set; }

    public string Name { get; set; } = null!;

    public bool Active { get; set; }

    public Guid TypeId { get; set; }

    public string? Details { get; set; }

    public string? IdParentId { get; set; }
}
