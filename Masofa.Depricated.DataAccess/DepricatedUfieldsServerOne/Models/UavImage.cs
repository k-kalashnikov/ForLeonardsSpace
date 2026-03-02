using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedUfieldsServerOne.Models;

public partial class UavImage
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public double? Area { get; set; }

    public Guid? RegionId { get; set; }

    public Guid? DistrictId { get; set; }

    public long? FileSize { get; set; }

    public string? ImageType { get; set; }

    public LocalDate? ImageDate { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public List<double>? Bbox { get; set; }

    public LocalDateTime? CreateDate { get; set; }

    public string? CreateUser { get; set; }

    public LocalDateTime? ModifyDate { get; set; }

    public string? ModifyUser { get; set; }

    public string? Comment { get; set; }

    public string? Tags { get; set; }

    public List<Guid>? Crops { get; set; }

    public string? AgricultureProducerId { get; set; }

    public Guid? CameraType { get; set; }

    public string? Trajectory { get; set; }
}
