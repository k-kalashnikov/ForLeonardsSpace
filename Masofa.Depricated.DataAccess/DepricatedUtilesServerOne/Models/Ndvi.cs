using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedUtilesServerOne.Models;

public partial class Ndvi
{
    public Guid Id { get; set; }

    public Guid Uid { get; set; }

    public int? SensorCode { get; set; }

    public LocalDate NdviDate { get; set; }

    public List<double>? NdviValue { get; set; }

    public double? Cloud { get; set; }

    public LocalDateTime? CreateTime { get; set; }

    public LocalDateTime? FieldModified { get; set; }

    public string? FieldName { get; set; }

    public int? W { get; set; }

    public int? H { get; set; }

    public List<short>? Xy { get; set; }

    public bool? Visibility { get; set; }

    public bool? HasBoundary { get; set; }

    public int? Internals { get; set; }

    public List<short>? InternalXy { get; set; }

    public List<double>? Rtvic { get; set; }

    public List<double>? Ndmi { get; set; }

    public List<double>? Ndwi { get; set; }

    public List<double>? Gndvi { get; set; }

    public List<double>? Savi { get; set; }

    public List<double>? Vari { get; set; }

    public List<double>? Cvi { get; set; }

    public List<double>? Lai { get; set; }

    public virtual Sensor? SensorCodeNavigation { get; set; }
}
