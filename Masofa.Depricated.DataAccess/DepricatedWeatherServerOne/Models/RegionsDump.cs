using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models;

[MigrationCompareAttributeWeather(CompareToType = typeof(Masofa.Common.Models.Weather.RegionsDump),
    CompareToDbContext = typeof(Masofa.DataAccess.MasofaWeatherDbContext))]
public partial class RegionsDump
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

    public string? Mhobt { get; set; }
}
