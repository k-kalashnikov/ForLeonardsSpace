using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedUfieldsServerOne.Models;

public partial class SeasonsHistory
{
    public Guid Id { get; set; }

    public Guid SeasonId { get; set; }

    public LocalDate? StartDate { get; set; }

    public LocalDate? EndDate { get; set; }

    public string? Title { get; set; }

    public LocalDate? PlantingDate { get; set; }

    public LocalDate? HarvestingDate { get; set; }

    public Guid? FieldId { get; set; }

    public Geometry? Polygon { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public Guid? CropId { get; set; }

    public Guid? VarietyId { get; set; }

    public LocalDate? PlantingDatePlan { get; set; }

    public double? FieldArea { get; set; }

    public LocalDate? HarvestingDatePlan { get; set; }

    public double? YieldHa { get; set; }

    public double? Yield { get; set; }

    public LocalDateTime? CreateDate { get; set; }

    public string? CreateUser { get; set; }

    public LocalDateTime? ModifyDate { get; set; }

    public string? ModifyUser { get; set; }

    public LocalDateTime? CreatedAt { get; set; }
}
