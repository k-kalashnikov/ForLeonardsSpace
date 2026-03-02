using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models;


public partial class LegacyRegion
{
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

    public string? RegionNameEn { get; set; }

    public string? RegionNameUz { get; set; }

    public string? Mhobt { get; set; }

    public Instant UpdateDate { get; set; }

    public Guid MapId { get; set; }
}
