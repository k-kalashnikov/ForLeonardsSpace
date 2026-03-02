using System;
using System.Collections.Generic;

namespace Masofa.Depricated.DataAccess.DepricatedUmapiServerTwo.Models;

public partial class ForeginWeatherRegion
{
    public Guid? Id { get; set; }

    public string? RegionName { get; set; }

    public string? RegionNameEn { get; set; }

    public string? RegionNameUz { get; set; }

    public int? RegionLevel { get; set; }

    public Guid? ParentId { get; set; }

    public bool? Active { get; set; }
}
