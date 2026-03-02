using System;
using System.Collections.Generic;

namespace Masofa.Depricated.DataAccess.DepricatedUalertsServerOne.Models;

public partial class VAlertsFrozen
{
    public Guid? RegionId { get; set; }

    public Guid? DisasterId { get; set; }

    public double? Areapercent { get; set; }

    public double? Area { get; set; }
}
