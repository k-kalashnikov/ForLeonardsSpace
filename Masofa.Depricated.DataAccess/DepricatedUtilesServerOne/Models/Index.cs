using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedUtilesServerOne.Models;

public partial class Index
{
    public Guid FieldId { get; set; }

    public Guid Uid { get; set; }

    public LocalDate Date { get; set; }

    public double? RawNdvi { get; set; }

    public double? Ndvi { get; set; }

    public double? Cloud { get; set; }

    public int? SensorCode { get; set; }

    public LocalDateTime? CreateTime { get; set; }

    public virtual Sensor? SensorCodeNavigation { get; set; }

    public virtual Field UidNavigation { get; set; } = null!;
}
