using System;
using System.Collections.Generic;

namespace Masofa.Depricated.DataAccess.DepricatedUmapiServerOne.Models;

public partial class TemplStep
{
    public Guid Id { get; set; }

    public Guid TemplBlockId { get; set; }

    public string? Num { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Days { get; set; }

    public int? StepsCount { get; set; }
}
