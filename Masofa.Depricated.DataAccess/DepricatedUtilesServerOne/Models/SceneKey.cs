using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedUtilesServerOne.Models;

public partial class SceneKey
{
    public Guid Id { get; set; }

    public Guid Uid { get; set; }

    public int? SensorCode { get; set; }

    public LocalDate Date { get; set; }

    public string Label { get; set; } = null!;

    public int SrcNumber { get; set; }

    public Guid ShapeId { get; set; }

    public LocalDateTime? FieldModified { get; set; }

    public double? NoData { get; set; }

    public LocalDateTime? CreateTime { get; set; }

    public string? Details { get; set; }

    public int? State { get; set; }

    public string? CopernicusJson { get; set; }

    public bool? Copernicus { get; set; }

    public double? Cloud { get; set; }

    public bool? Selected { get; set; }

    public bool? Merged { get; set; }

    public virtual Sensor? SensorCodeNavigation { get; set; }

    public virtual State? StateNavigation { get; set; }
}
