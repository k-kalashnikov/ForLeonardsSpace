using System;
using System.Collections.Generic;

namespace Masofa.Depricated.DataAccess.DepricatedUfieldsServerOne.Models;

public partial class FarmerMetadatum
{
    public Guid FarmerId { get; set; }

    public string? Name { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? MiddleName { get; set; }

    public string? Title { get; set; }

    public virtual Farmer Farmer { get; set; } = null!;
}
