using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedUtilesServerOne.Models;

public partial class Shape
{
    public Guid Id { get; set; }

    public int SensorCode { get; set; }

    public string Label { get; set; } = null!;

    public LocalDateTime? CreatedAt { get; set; }

    public Guid? Uid { get; set; }

    public LocalDateTime? FieldModified { get; set; }

    public Guid? ZoneId { get; set; }

    public virtual Sensor SensorCodeNavigation { get; set; } = null!;
}
