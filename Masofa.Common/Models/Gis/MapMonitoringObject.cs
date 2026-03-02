using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Gis
{
    public class MapMonitoringObject : BaseEntity
    {
        private MapMonitoringObjectType _type;
        private Polygon? _polygon;

        public string Name { get; set; } = null!;

        public Guid UserId { get; set; }

        public MapMonitoringObjectType Type
        {
            get => _type;
            set
            {
                _type = value;
                if (_type == MapMonitoringObjectType.Polygon && Polygon != null)
                {
                    Point = Polygon.Centroid;
                }
            }
        }

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public Point? Point { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public Polygon? Polygon
        {
            get => _polygon;
            set
            {
                if (Type == MapMonitoringObjectType.Polygon)
                {
                    _polygon = value;
                    if (_polygon != null)
                    {
                        Point = _polygon.Centroid;
                    }
                }
            }
        }

        [NotMapped]
        public IEnumerable<CoordinateDto> PolygonJson
        {
            set
            {
                ArgumentNullException.ThrowIfNull(value);

                var list = value.ToList();

                if (list.Count == 0)
                {
                    throw new ArgumentException("At least one coordinate is required.", nameof(value));
                }

                var coords = list
                    .Select(p => new Coordinate(p.Lon, p.Lat))
                    .Concat([new Coordinate(list.First().Lon, list.First().Lat)])
                    .ToArray();

                var factory = new GeometryFactory(new PrecisionModel(), 4326);
                var ring = factory.CreateLinearRing(coords);
                Polygon = factory.CreatePolygon(ring);
            }
            get
            {
                return Polygon != null
                    ? Polygon.ExteriorRing.Coordinates
                        .Take(Polygon.ExteriorRing.NumPoints - 1)
                        .Select(c => new CoordinateDto
                        {
                            Lon = c.X,
                            Lat = c.Y,
                        })
                    : Array.Empty<CoordinateDto>();
            }
        }

        [NotMapped]
        public CoordinateDto? PointJson
        {
            get
            {
                if (Point != null)
                {
                    return new()
                    {
                        Lat = Point.Y,
                        Lon = Point.X,
                    };
                }

                return null;
            }
            set
            {
                if (value != null && Type == MapMonitoringObjectType.Point)
                {
                    Point = new Point(value.Lon, value.Lat);
                }
            }
        }
    }

    public enum MapMonitoringObjectType
    {
        Polygon = 1,
        Point = 2
    }

    public class CoordinateDto
    {
        public double Lon { get; set; }
        public double Lat { get; set; }
    }

    public class MapMonitoringObjectHistory : BaseHistoryEntity<MapMonitoringObject> { }
}
