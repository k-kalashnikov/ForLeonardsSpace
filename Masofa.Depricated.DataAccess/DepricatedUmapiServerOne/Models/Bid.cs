using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedUmapiServerOne.Models;

public partial class Bid
{
    public Guid Id { get; set; }

    public Guid? ParentId { get; set; }

    public Guid CreateUser { get; set; }

    public LocalDateTime CreateDate { get; set; }

    public Guid ModifyUser { get; set; }

    public LocalDateTime ModifyDate { get; set; }

    public bool Active { get; set; }

    public Guid BidTypeId { get; set; }

    public Guid BidStateId { get; set; }

    public Guid? ForemanId { get; set; }

    public Guid? WorkerId { get; set; }

    public LocalDateTime? StartDate { get; set; }

    public LocalDateTime? DeadlineDate { get; set; }

    public LocalDateTime? EndDate { get; set; }

    public Guid? FieldId { get; set; }

    public Guid? ContentId { get; set; }

    public Guid? RegionId { get; set; }

    public Guid? CropId { get; set; }

    public Guid? VarietyId { get; set; }

    public string? Comment { get; set; }

    public string? Description { get; set; }

    public double? Lat { get; set; }

    public double? Lng { get; set; }

    public bool? Published { get; set; }

    public bool Cancelled { get; set; }

    public long Number { get; set; }

    public LocalDate? FieldPlantingDate { get; set; }

    public string? GeoJson { get; set; }

    public string? ContourId { get; set; }
}
