using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models;

[MigrationCompareAttributeWeather(CompareToType = typeof(Masofa.Common.Models.Weather.AgroClimaticZone), 
    CompareToDbContext = typeof(Masofa.DataAccess.MasofaWeatherDbContext))]
public partial class AgroClimaticZone
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string Polygon { get; set; } = null!;

    public Geometry? PolygonGeom { get; set; }

    public bool? Active { get; set; }

    public string? NameEn { get; set; }

    public string? NameUz { get; set; }
}
