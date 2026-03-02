using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models;

[MigrationCompareAttributeWeather(CompareToType = typeof(Masofa.Common.Models.Weather.Region),
    CompareToDbContext = typeof(Masofa.DataAccess.MasofaWeatherDbContext))]
public partial class Region
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

    public LocalDateTime UpdateDate { get; set; }
}
