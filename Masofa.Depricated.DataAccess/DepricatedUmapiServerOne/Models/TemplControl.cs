using System;
using System.Collections.Generic;

namespace Masofa.Depricated.DataAccess.DepricatedUmapiServerOne.Models;

public partial class TemplControl
{
    public Guid Id { get; set; }

    public Guid TemplBlockId { get; set; }

    public Guid? TemplStepId { get; set; }

    public string Type { get; set; } = null!;

    public string? Name { get; set; }

    public bool? Auto { get; set; }

    public string? Source { get; set; }

    public bool? Readonly { get; set; }

    public string? Label { get; set; }

    public string? Descr { get; set; }

    public string? Values { get; set; }

    public string? Days { get; set; }

    public bool? Required { get; set; }
}
