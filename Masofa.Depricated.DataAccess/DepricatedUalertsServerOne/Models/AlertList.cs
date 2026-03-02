using System;
using System.Collections.Generic;

namespace Masofa.Depricated.DataAccess.DepricatedUalertsServerOne.Models;

public partial class AlertList
{
    public Guid Id { get; set; }

    public Guid? RegionId { get; set; }

    public Guid? CropId { get; set; }

    public double? Area { get; set; }

    public double? InfluenceClimate { get; set; }

    public double? Loss { get; set; }
}
