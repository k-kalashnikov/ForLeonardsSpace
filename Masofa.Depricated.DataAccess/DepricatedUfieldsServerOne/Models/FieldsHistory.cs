using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedUfieldsServerOne.Models;

public partial class FieldsHistory
{
    public Guid Id { get; set; }

    public Guid FieldId { get; set; }

    public string? Name { get; set; }

    public double? FieldArea { get; set; }

    public Guid? FarmerId { get; set; }

    public Guid? RegionId { get; set; }

    public string? ExternalId { get; set; }

    public string? ContourId { get; set; }

    public Guid? DistrictId { get; set; }

    public double? SoilIndex { get; set; }

    public Guid? SoilTypeId { get; set; }

    public Guid? AgroclimaticZoneId { get; set; }

    public Guid? ClassifierId { get; set; }

    public string? Comment { get; set; }

    public Guid? AgricultureProducerId { get; set; }

    public Guid? IrrigationTypeId { get; set; }

    public Guid? IrrigationSourceId { get; set; }

    public bool? WaterSaving { get; set; }

    public LocalDateTime? CreateDate { get; set; }

    public string? CreateUser { get; set; }

    public LocalDateTime? ModifyDate { get; set; }

    public string? ModifyUser { get; set; }

    public Geometry? Polygon { get; set; }

    public bool? Control { get; set; }

    public LocalDateTime? CreatedAt { get; set; }
}
