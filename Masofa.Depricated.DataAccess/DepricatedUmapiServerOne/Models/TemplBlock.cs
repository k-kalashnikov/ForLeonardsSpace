using System;
using System.Collections.Generic;

namespace Masofa.Depricated.DataAccess.DepricatedUmapiServerOne.Models;

public partial class TemplBlock
{
    public Guid Id { get; set; }

    public Guid TemplId { get; set; }

    public string Type { get; set; } = null!;
}
