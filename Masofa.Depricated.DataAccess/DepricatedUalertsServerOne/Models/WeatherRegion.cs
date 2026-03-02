using System;
using System.Collections.Generic;

namespace Masofa.Depricated.DataAccess.DepricatedUalertsServerOne.Models;

public partial class WeatherRegion
{
    public Guid? Id { get; set; }

    public string? RegionName { get; set; }

    public string? RegionNameEn { get; set; }

    public string? RegionNameUz { get; set; }

    public int? RegionLevel { get; set; }
}
