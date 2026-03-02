using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedUtilesServerOne.Models;

public partial class Group
{
    public Guid Id { get; set; }

    public int? SensorCode { get; set; }

    public Guid? ZoneId { get; set; }

    public string Label { get; set; } = null!;

    public double? North { get; set; }

    public double? South { get; set; }

    public double? West { get; set; }

    public double? East { get; set; }

    public LocalDateTime? CreatedAt { get; set; }

    public LocalDateTime? Modified { get; set; }

    public virtual Zone? Zone { get; set; }
}
