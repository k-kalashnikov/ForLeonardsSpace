using System;
using System.Collections.Generic;

namespace Masofa.Depricated.DataAccess.DepricatedUmapiServerTwo.Models;

public partial class WateringType
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? NameEn { get; set; }

    public string? NameUz { get; set; }
}
