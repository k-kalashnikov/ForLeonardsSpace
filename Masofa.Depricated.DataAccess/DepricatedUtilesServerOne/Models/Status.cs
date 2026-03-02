using System;
using System.Collections.Generic;

namespace Masofa.Depricated.DataAccess.DepricatedUtilesServerOne.Models;

public partial class Status
{
    public Guid Id { get; set; }

    public int Code { get; set; }

    public string? Describe { get; set; }
}
