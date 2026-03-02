using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedUtilesServerOne.Models;

public partial class GroupShape
{
    public Guid GroupId { get; set; }

    public Guid ShapeId { get; set; }

    public LocalDateTime? CreatedAt { get; set; }
}
