using System;
using System.Collections.Generic;

namespace Masofa.Depricated.DataAccess.DepricatedUalertsServerOne.Models;

public partial class DisasterList
{
    public Guid Id { get; set; }

    public Guid? Disaster { get; set; }

    public Guid? RegionId { get; set; }

    public double? Area { get; set; }

    public double? AreaPercent { get; set; }
}
