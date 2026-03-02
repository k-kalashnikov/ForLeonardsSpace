using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Weather;

public partial class RegionsDump
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public string? RegionName { get; set; }

    public string? Iso { get; set; }

    public int? RegionLevel { get; set; }

    public double? Lat { get; set; }

    public double? Lon { get; set; }

    public int? RowX { get; set; }

    public int? ColumnY { get; set; }

    public string Polygon { get; set; } = null!;

    public Geometry? PolygonGeom { get; set; }

    public Guid? ParentId { get; set; }

    public bool? Active { get; set; }

    public string? Mhobt { get; set; }
}
