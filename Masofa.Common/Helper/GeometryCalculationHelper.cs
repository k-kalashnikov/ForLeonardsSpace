using Masofa.Common.Services;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Utilities;
using NetTopologySuite;
using NetTopologySuite.Operation.Union;
using Microsoft.Extensions.Configuration;
using NetTopologySuite.Precision;
using NetTopologySuite.Algorithm.Hull;

namespace Masofa.Common.Helper
{
    /// <summary>
    /// Provides utility methods for performing geometric calculations, such as intersections, bounding box creation, 
    /// and geometry parsing. This class is designed to work with polygons and other geometric shapes, leveraging  the
    /// NetTopologySuite library.
    /// </summary>
    /// <remarks>This static helper class includes methods for calculating intersections between polygons,
    /// creating bounding boxes,  generating Well-Known Text (WKT) representations, and validating geometric
    /// relationships. It is intended for use  in applications that require spatial analysis or geometry processing,
    /// such as GIS systems or remote sensing tools.</remarks>
    public static class GeometryCalculationHelper
    {
        private static readonly GeometryFactory _geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

        public static IntersectionResult CalculateIntersectionWithResult(Polygon fieldPolygon, Polygon productPolygon)
        {
            try
            {
                // Проверяем пересечение
                if (!fieldPolygon.Intersects(productPolygon))
                {
                    return new IntersectionResult { HasIntersection = false };
                }

                // Вычисляем пересечение
                var intersection = fieldPolygon.Intersection(productPolygon);

                if (intersection.IsEmpty)
                {
                    return new IntersectionResult { HasIntersection = false };
                }

                // Вычисляем площади
                var intersectionArea = intersection.Area;
                var fieldArea = fieldPolygon.Area;
                var coveragePercentage = fieldArea > 0 ? (intersectionArea / fieldArea) * 100 : 0;

                return new IntersectionResult
                {
                    HasIntersection = true,
                    IntersectionArea = intersectionArea,
                    CoveragePercentage = coveragePercentage,
                    IntersectionGeometry = intersection
                };
            }
            catch (Exception ex)
            {
                return new IntersectionResult 
                { 
                    HasIntersection = false,
                    Message = "Error calculating intersection between field and product polygons"
                };
            }
        }

        /// <summary>
        /// Создает глобальную область поиска из полигонов полей
        /// </summary>
        /// <param name="fieldPolygons">Список полигонов полей</param>
        /// <param name="bufferDistance">Расстояние буфера в метрах</param>
        /// <returns>Объединенный полигон с естественным (вогнутым) контуром</returns>
        public static Polygon? CreateGlobalSearchArea(IEnumerable<Polygon> fieldPolygons)
        {
            try
            {
                // Фильтруем null
                var polygons = fieldPolygons.Where(p => p != null).ToList();
                if (!polygons.Any())
                {
                    return null;
                }

                // Приводим геометрии к валидному виду и снижаем точность, чтобы избежать ошибок Overlay/Union
                var cleanedGeometries = new List<Geometry>(polygons.Count);
                var precisionReducer = new GeometryPrecisionReducer(new PrecisionModel(1e6)); // ~6 знаков после запятой

                foreach (var polygon in polygons)
                {
                    Geometry geometry = polygon;

                    if (!geometry.IsValid)
                    {
                        geometry = GeometryFixer.Fix(geometry);
                    }

                    geometry = precisionReducer.Reduce(geometry);

                    if (!geometry.IsEmpty)
                    {
                        cleanedGeometries.Add(geometry);
                    }
                }

                if (cleanedGeometries.Count == 0)
                {
                    return null;
                }

                // Сбор всех геометрий в коллекцию для построения вогнутой оболочки
                var geomCollection = _geometryFactory.CreateGeometryCollection(cleanedGeometries.ToArray());

                // Автоматический выбор порога ребра на основе плотности точек
                // Рекомендация NTS: использовать 2-4x от UniformGridEdgeLength
                double baseEdge = ConcaveHull.UniformGridEdgeLength(geomCollection);
                // Стартуем с более гладкого контура
                double lengthThreshold = baseEdge * 75.0;

                // Строим вогнутую оболочку
                var concaveHull = new ConcaveHull(geomCollection)
                {
                    MaximumEdgeLength = lengthThreshold,
                    HolesAllowed = false
                };
                var hull = concaveHull.GetHull();

                if (hull == null || hull.IsEmpty)
                {
                    // Фоллбэк: объединение и выпуклая оболочка
                    var unionFallback = new UnaryUnionOp(cleanedGeometries).Union();
                    return unionFallback switch
                    {
                        Polygon p => p,
                        GeometryCollection gc when gc.Count == 1 && gc[0] is Polygon p1 => p1,
                        _ => unionFallback.ConvexHull() as Polygon
                    };
                }

                // Приводим результат к Polygon, если возможно. Если мульти- или коллекция, берём выпуклую оболочку как единый контур
                return hull switch
                {
                    Polygon p => p,
                    GeometryCollection gc when gc.Count == 1 && gc[0] is Polygon p1 => p1,
                    _ => hull.ConvexHull() as Polygon
                };
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Вычисляет MBR (Minimum Bounding Rectangle) для Landsat API
        /// </summary>
        /// <param name="polygon">Полигон</param>
        /// <returns>Кортеж с нижним левым и верхним правым углами</returns>
        public static (Point LowerLeft, Point UpperRight) CalculateLandsatMBR(Polygon polygon)
        {
            try
            {
                var envelope = polygon.EnvelopeInternal;
                
                var lowerLeft = _geometryFactory.CreatePoint(new Coordinate(envelope.MinX, envelope.MinY));
                var upperRight = _geometryFactory.CreatePoint(new Coordinate(envelope.MaxX, envelope.MaxY));

                return (lowerLeft, upperRight);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Создает WKT строку для Sentinel2 API
        /// </summary>
        /// <param name="polygon">Полигон</param>
        /// <returns>WKT строка</returns>
        public static string CreateSentinelWKT(Polygon polygon)
        {
            try
            {
                return polygon.AsText();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Проверяет пересечение двух полигонов
        /// </summary>
        /// <param name="polygon1">Первый полигон</param>
        /// <param name="polygon2">Второй полигон</param>
        /// <returns>true если есть пересечение</returns>
        public static bool CalculateIntersection(Polygon polygon1, Polygon polygon2)
        {
            try
            {
                return polygon1.Intersects(polygon2);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Создает полигон из bounding box
        /// </summary>
        /// <param name="west">Западная граница</param>
        /// <param name="south">Южная граница</param>
        /// <param name="east">Восточная граница</param>
        /// <param name="north">Северная граница</param>
        /// <returns>Полигон</returns>
        public static Polygon? CreatePolygonFromBoundingBox(decimal west, decimal south, decimal east, decimal north)
        {
            try
            {
                var coordinates = new[]
                {
                    new Coordinate((double)west, (double)south),
                    new Coordinate((double)east, (double)south),
                    new Coordinate((double)east, (double)north),
                    new Coordinate((double)west, (double)north),
                    new Coordinate((double)west, (double)south) // замкнуть полигон
                };

                var shell = new LinearRing(coordinates);
                return new Polygon(shell);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Парсит геометрию из JSON
        /// </summary>
        /// <param name="geometryJson">JSON строка с геометрией</param>
        /// <returns>Полигон или null</returns>
        public static Polygon? ParseGeometryFromJson(string geometryJson)
        {
            try
            {
                using var document = System.Text.Json.JsonDocument.Parse(geometryJson);
                var root = document.RootElement;

                if (root.TryGetProperty("type", out var typeElement) && 
                    root.TryGetProperty("coordinates", out var coordinatesElement))
                {
                    var type = typeElement.GetString();
                    
                    if (type == "Polygon")
                    {
                        var coordinates = coordinatesElement.EnumerateArray()
                            .Select(ring => ring.EnumerateArray()
                                .Select(point => new Coordinate(
                                    point[0].GetDouble(),
                                    point[1].GetDouble()))
                                .ToArray())
                            .ToArray();

                        if (coordinates.Length > 0)
                        {
                            var shell = new LinearRing(coordinates[0]);
                            var holes = coordinates.Skip(1).Select(ring => new LinearRing(ring)).ToArray();
                            return new Polygon(shell, holes);
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Проверяет, входит ли поле в границы страны
        /// </summary>
        /// <param name="field">Поле для проверки</param>
        /// <param name="configuration">Конфигурация с границами страны</param>
        /// <returns>true если поле входит в границы страны</returns>
        public static bool IsFieldWithinCountryBoundaries(Masofa.Common.Models.CropMonitoring.Field field, IConfiguration configuration)
        {
            try
            {
                if (field.Polygon == null)
                    return false;

                // Получаем границы страны из конфигурации
                var latMin = configuration.GetValue<double>("CountryBoundaries:LatMin");
                var latMax = configuration.GetValue<double>("CountryBoundaries:LatMax");
                var lonMin = configuration.GetValue<double>("CountryBoundaries:LonMin");
                var lonMax = configuration.GetValue<double>("CountryBoundaries:LonMax");

                // Создаем полигон границ страны
                var countryPolygon = CreatePolygonFromBoundingBox(
                    (decimal)lonMin, (decimal)latMin, (decimal)lonMax, (decimal)latMax);

                if (countryPolygon == null)
                {
                    return false;
                }

                // Проверяем пересечение поля с границами страны
                var intersectionResult = CalculateIntersectionWithResult(field.Polygon, countryPolygon);
                
                return intersectionResult.HasIntersection;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }

    public class IntersectionResult
    {
        public bool HasIntersection { get; set; }
        public double IntersectionArea { get; set; }
        public double CoveragePercentage { get; set; }
        public Geometry? IntersectionGeometry { get; set; }
        public string? Message { get; set; }
    }
}
