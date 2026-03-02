using System;
using System.Collections.Generic;

namespace Masofa.Depricated.DataAccess.DepricatedUmapiServerTwo.Models;

public partial class Pest
{
    public Guid Id { get; set; }

    public Guid CropId { get; set; }

    public string Name { get; set; } = null!;

    public string? NameEn { get; set; }

    public string? NameUz { get; set; }
}
