using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.CropMonitoring
{
    /// <summary>
    /// Представляет информацию об загружаемом поле
    /// </summary>
    public class ImportedField : BaseEntity
    {
        /// <summary>
        /// Наименование поля
        /// </summary>
        public string FieldName { get; set; } = string.Empty;

        /// <summary>
        /// Координаты границ поля
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public NetTopologySuite.Geometries.Polygon? Polygon { get; set; }

        /// <summary>
        /// Текстовое представление полигона поля
        /// </summary>
        [NotMapped]
        public string? PolygonJson
        {
            get
            {
                var poly = Polygon;
                if (poly == null || poly.IsEmpty)
                {
                    return null;
                }

                return poly.AsText();
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    Polygon = null;
                }
                else
                {
                    try
                    {
                        var reader = new NetTopologySuite.IO.WKTReader();
                        var geometry = reader.Read(value);
                        if (geometry is Polygon polygon)
                        {
                            Polygon = polygon;
                        }
                        else
                        {
                            Polygon = null;
                        }
                    }
                    catch
                    {
                        Polygon = null;
                    }
                }
            }
        }

        /// <summary>
        /// Площадь поля (га)
        /// </summary>
        [NotMapped]
        public double? PolygonSquare
        {
            get
            {
                if (Polygon == null)
                {
                    return null;
                }

                var c = Polygon.Centroid;
                var lon = c.X;
                var lat = c.Y;

                int zone = (int)Math.Floor((lon + 180d) / 6d) + 1;
                bool north = lat >= 0;

                var csFactory = new ProjNet.CoordinateSystems.CoordinateSystemFactory();
                var wgs84 = ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84;
                var utm = ProjNet.CoordinateSystems.ProjectedCoordinateSystem.WGS84_UTM(zone, north);

                var transformer = new ProjNet.CoordinateSystems.Transformations.CoordinateTransformationFactory()
                    .CreateFromCoordinateSystems(wgs84, utm);

                var geometryFactory = new NetTopologySuite.Geometries.GeometryFactory(new NetTopologySuite.Geometries.PrecisionModel(), (int)utm.AuthorityCode);

                var coordinates = Polygon.ExteriorRing.Coordinates
                    .Select(coord =>
                    {
                        var point = new[] { coord.X, coord.Y };
                        var transformed = transformer.MathTransform.Transform(point);
                        return new NetTopologySuite.Geometries.Coordinate(transformed[0], transformed[1]);
                    })
                    .ToArray();

                var linearRing = geometryFactory.CreateLinearRing(coordinates);
                var transformedPolygon = geometryFactory.CreatePolygon(linearRing);

                return transformedPolygon.Area / 10_000;
            }
        }

        /// <summary>
        /// Аттрибутивные данные загружаемого поля
        /// </summary>
        public string? DataJson { get; set; }

        /// <summary>
        /// Идентификатор отчета загрузки полей
        /// </summary>
        public Guid ImportedFieldReportId { get; set; }
    }

    public class ImportedFieldHistory : BaseHistoryEntity<ImportedField> { }
}
