using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedUtilesServerOne.Models;

public partial class Zone
{
    public Guid Id { get; set; }

    public int SensorCode { get; set; }

    public string Label { get; set; } = null!;

    public LocalDateTime? CreatedAt { get; set; }

    public Guid? AppId { get; set; }

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual Sensor SensorCodeNavigation { get; set; } = null!;
}
