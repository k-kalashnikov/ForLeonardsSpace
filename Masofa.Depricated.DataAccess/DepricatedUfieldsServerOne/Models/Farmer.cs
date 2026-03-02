using System;
using System.Collections.Generic;

namespace Masofa.Depricated.DataAccess.DepricatedUfieldsServerOne.Models;

public partial class Farmer
{
    public Guid Id { get; set; }

    public string? ExternalId { get; set; }

    public string? Name { get; set; }

    public virtual FarmerMetadatum? FarmerMetadatum { get; set; }
}
