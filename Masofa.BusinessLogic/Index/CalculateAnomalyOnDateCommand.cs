using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using Masofa.Common.Models.Satellite;
using Masofa.Common.Models.Satellite.Indices;
using Masofa.Common.Resources;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Masofa.BusinessLogic.Index
{
    public class CalculateAnomalyOnDateCommand : IRequest
    {
        public DateOnly Date { get; set; }
    }

    public class CalculateAnomalyOnDateCommandHandler : IRequestHandler<CalculateAnomalyOnDateCommand>
    {
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }
        private ILogger<CalculateAnomalyOnDateCommandHandler> Logger { get; set; }
        private IMediator Mediator { get; set; }

        private MasofaIdentityDbContext IdentityDbContext { get; set; }
        private MasofaIndicesDbContext IndicesDbContext { get; set; }

        private static readonly int _minValidPixels = 50;
        private static readonly int _srid = 4326;
        private static readonly double _singlePointSize = 10.0;
        private static readonly double _lineBufferRadius = 5.0;

        private static readonly Dictionary<string, (double low, double high)> _thresholds = new()
        {
            ["ndvi"] = (0.15, 0.85),
            ["ndmi"] = (0.15, 0.85),
            ["ndwi"] = (0.15, 0.85),
            ["gndvi"] = (0.15, 0.85),
            ["orvi"] = (0.15, 0.85)
        };

        private readonly Dictionary<AnomalyType, string> _colors = new()
        {
            [AnomalyType.Flooding] = "#1f78b4",
            [AnomalyType.Overwatering] = "#a6cee3",
            [AnomalyType.Drought] = "#e31a1c",
            [AnomalyType.SkippingOfSeedlings] = "#ff7f00",
            [AnomalyType.PrematureAging] = "#b2df8a",
            [AnomalyType.NutritionalStress] = "#6a3d9a"
        };

        public CalculateAnomalyOnDateCommandHandler(
            IBusinessLogicLogger businessLogicLogger,
            ILogger<CalculateAnomalyOnDateCommandHandler> logger,
            IMediator mediator,
            MasofaIdentityDbContext identityDbContext,
            MasofaIndicesDbContext indicesDbContext)
        {
            BusinessLogicLogger = businessLogicLogger;
            Logger = logger;
            Mediator = mediator;
            IdentityDbContext = identityDbContext;
            IndicesDbContext = indicesDbContext;
        }

        public async Task Handle(CalculateAnomalyOnDateCommand request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                var lastUpdateUser = await IdentityDbContext.Users.FirstAsync(m => m.UserName.ToLower().Equals("admin"));

                var anomalyDate = request.Date;

                var geometryFactory = new NetTopologySuite.Geometries.GeometryFactory();

                var gndviPoints = await IndicesDbContext.GndviPoints.AsNoTracking().Where(i => DateOnly.FromDateTime(i.CreateAt) == anomalyDate).ToListAsync(cancellationToken);
                var ndmiPoints = await IndicesDbContext.NdmiPoints.AsNoTracking().Where(i => DateOnly.FromDateTime(i.CreateAt) == anomalyDate).ToListAsync(cancellationToken);
                var ndviPoints = await IndicesDbContext.NdviPoints.AsNoTracking().Where(i => DateOnly.FromDateTime(i.CreateAt) == anomalyDate).ToListAsync(cancellationToken);
                var ndwiPoints = await IndicesDbContext.NdwiPoints.AsNoTracking().Where(i => DateOnly.FromDateTime(i.CreateAt) == anomalyDate).ToListAsync(cancellationToken);
                var orviPoints = await IndicesDbContext.OrviPoints.AsNoTracking().Where(i => DateOnly.FromDateTime(i.CreateAt) == anomalyDate).ToListAsync(cancellationToken);

                if (gndviPoints.Count == 0 || ndmiPoints.Count == 0 || ndviPoints.Count == 0 || ndwiPoints.Count == 0 || orviPoints.Count == 0) return;

                var gndviSeasons = gndviPoints.Where(p => p.SeasonId != null).GroupBy(i => i.SeasonId.Value).ToDictionary(x => x.Key, x => x.ToList());
                var ndmiSeasons = ndmiPoints.Where(p => p.SeasonId != null).GroupBy(i => i.SeasonId.Value).ToDictionary(x => x.Key, x => x.ToList());
                var ndviSeasons = ndviPoints.Where(p => p.SeasonId != null).GroupBy(i => i.SeasonId.Value).ToDictionary(x => x.Key, x => x.ToList());
                var ndwiSeasons = ndwiPoints.Where(p => p.SeasonId != null).GroupBy(i => i.SeasonId.Value).ToDictionary(x => x.Key, x => x.ToList());
                var orviSeasons = orviPoints.Where(p => p.SeasonId != null).GroupBy(i => i.SeasonId.Value).ToDictionary(x => x.Key, x => x.ToList());

                ProductSourceType productSourceType = ProductSourceType.Sentinel2;
                if (ndviPoints.Count > 0)
                {
                    productSourceType = ndviPoints[0].ProductSourceType;
                }

                HashSet<Guid> seasonsIds = [];
                seasonsIds.UnionWith(gndviSeasons.Keys);
                seasonsIds.UnionWith(ndmiSeasons.Keys);
                seasonsIds.UnionWith(ndviSeasons.Keys);
                seasonsIds.UnionWith(ndwiSeasons.Keys);
                seasonsIds.UnionWith(orviSeasons.Keys);

                List<AnomalyPoint> anomalyPoints = [];
                Dictionary<Guid, List<AnomalyPoint>> anomalyPointsBySeasonId = [];

                foreach (var seasonId in seasonsIds)
                {
                    List<AnomalyPoint> seasonPoints = [];
                    var validGndviPoints = gndviSeasons[seasonId].Where(p => p.Point != null && !double.IsNaN(p.Value)).Select(p => p.Point).ToList();
                    var validNdmiPoints = ndmiSeasons[seasonId].Where(p => p.Point != null && !double.IsNaN(p.Value)).Select(p => p.Point).ToList();
                    var validNdviPoints = ndviSeasons[seasonId].Where(p => p.Point != null && !double.IsNaN(p.Value)).Select(p => p.Point).ToList();
                    var validNdwiPoints = ndwiSeasons[seasonId].Where(p => p.Point != null && !double.IsNaN(p.Value)).Select(p => p.Point).ToList();
                    var validOrviPoints = orviSeasons[seasonId].Where(p => p.Point != null && !double.IsNaN(p.Value)).Select(p => p.Point).ToList();

                    HashSet<NetTopologySuite.Geometries.Point> validPoints = [];
                    validPoints.UnionWith(validGndviPoints);
                    validPoints.UnionWith(validNdmiPoints);
                    validPoints.UnionWith(validNdviPoints);
                    validPoints.UnionWith(validNdwiPoints);
                    validPoints.UnionWith(validOrviPoints);

                    if (validPoints.Count < _minValidPixels)
                    {
                        continue;
                    }

                    var gndviVals = gndviSeasons[seasonId].Select(p => (double)p.Value).ToArray();
                    var ndmiVals = ndmiSeasons[seasonId].Select(p => (double)p.Value).ToArray();
                    var ndviVals = ndviSeasons[seasonId].Select(p => (double)p.Value).ToArray();
                    var ndwiVals = ndwiSeasons[seasonId].Select(p => (double)p.Value).ToArray();
                    var orviVals = orviSeasons[seasonId].Select(p => (double)p.Value).ToArray();

                    if (gndviVals.Length == 0 || ndmiVals.Length == 0 || ndviVals.Length == 0 || ndwiVals.Length == 0 || orviVals.Length == 0) return;

                    var ndviLow = Quantile(ndviVals, _thresholds["ndvi"].low);
                    var ndviVeryLow = Quantile(ndviVals, _thresholds["ndvi"].low * 0.5);
                    var ndmiLow = Quantile(ndmiVals, _thresholds["ndmi"].low);
                    var ndmiHigh = Quantile(ndmiVals, _thresholds["ndmi"].high);
                    var ndwiLow = Quantile(ndwiVals, _thresholds["ndwi"].low);
                    var ndwiHigh = Quantile(ndwiVals, _thresholds["ndwi"].high);
                    var gndviLow = Quantile(gndviVals, _thresholds["gndvi"].low);
                    var orviLow = Quantile(orviVals, _thresholds["orvi"].low);

                    foreach (var p in validPoints)
                    {
                        var pGndvi = gndviPoints.FirstOrDefault(i => i.Point == p);
                        var pNdmi = ndmiPoints.FirstOrDefault(i => i.Point == p);
                        var pNdvi = ndviPoints.FirstOrDefault(i => i.Point == p);
                        var pNdwi = ndwiPoints.FirstOrDefault(i => i.Point == p);
                        var pOrvi = orviPoints.FirstOrDefault(i => i.Point == p);

                        var pGndviValue = pGndvi?.Value;
                        var pNdmiValue = pNdmi?.Value;
                        var pNdviValue = pNdvi?.Value;
                        var pNdwiValue = pNdwi?.Value;
                        var pOrviVallue = pOrvi?.Value;

                        bool ndviLowFlag = pNdviValue < ndviLow;
                        bool ndviVeryLowFlag = pNdviValue < ndviVeryLow;
                        bool ndmiLowFlag = pNdmiValue < ndmiLow;
                        bool ndmiHighFlag = pNdmiValue > ndmiHigh;
                        bool ndwiLowFlag = pNdwiValue < ndwiLow;
                        bool ndwiHighFlag = pNdwiValue > ndwiHigh;
                        bool chlLow = (pGndviValue < gndviLow) || (pOrviVallue < orviLow);

                        AnomalyType? anomalyType = null;

                        if (ndviLowFlag && ndwiHighFlag)
                        {
                            anomalyType = AnomalyType.Flooding;
                        }
                        else if (ndviLowFlag && ndmiHighFlag)
                        {
                            anomalyType = AnomalyType.Overwatering;
                        }
                        else if (ndviLowFlag && ndmiLowFlag && ndwiLowFlag)
                        {
                            anomalyType = AnomalyType.Drought;
                        }
                        else if (ndviVeryLowFlag && chlLow)
                        {
                            anomalyType = AnomalyType.SkippingOfSeedlings;
                        }
                        else if (ndviLowFlag && chlLow)
                        {
                            anomalyType = AnomalyType.PrematureAging;
                        }
                        else if (chlLow && !ndmiLowFlag)
                        {
                            anomalyType = AnomalyType.NutritionalStress;
                        }

                        var newAnomalyPoint = new AnomalyPoint()
                        {
                            AnomalyType = anomalyType,
                            Color = anomalyType == null ? null : _colors.GetValueOrDefault(anomalyType.Value, null),
                            Point = p,
                            RegionId = pNdvi.RegionId,
                            FieldId = pNdvi.FieldId,
                            SeasonId = pNdvi.SeasonId,
                            CreateAt = pNdvi.CreateAt,
                            ProductSourceType = pNdvi.ProductSourceType,
                            SatelliteProductId = pNdvi.SatelliteProductId
                        };

                        seasonPoints.Add(newAnomalyPoint);
                    }
                    anomalyPoints.AddRange(seasonPoints);
                    anomalyPointsBySeasonId[seasonId] = anomalyPoints;
                }

                foreach (var (seasonId, seasonPoints) in anomalyPointsBySeasonId)
                {
                    if (seasonPoints.Count == 0) continue;

                    var pointsWithoutAnomaly = seasonPoints.Where(p => p.AnomalyType == null).ToList();
                    await IndicesDbContext.AnomalyPoints.AddRangeAsync(pointsWithoutAnomaly, cancellationToken);
                    await IndicesDbContext.SaveChangesAsync(cancellationToken);

                    var pointsByAnomaly = seasonPoints.Where(p => p.AnomalyType != null).GroupBy(p => p.AnomalyType.Value).ToDictionary(x => x.Key, x => x.ToList());

                    foreach (var (anomalyType, points) in pointsByAnomaly)
                    {
                        var clusters = ClusterPointsByDistance(points.Where(p => p.AnomalyType != null).Select(p => p.Point).ToList(), 10.0);

                        List<NetTopologySuite.Geometries.Polygon> polygons = [];

                        foreach (var cluster in clusters)
                        {
                            if (cluster.Count == 1)
                            {
                                var pt = cluster[0];
                                var half = _singlePointSize / 2.0;
                                var square = geometryFactory.CreatePolygon(
                                [
                                    new NetTopologySuite.Geometries.Coordinate(pt.X - half, pt.Y - half),
                                    new NetTopologySuite.Geometries.Coordinate(pt.X + half, pt.Y - half),
                                    new NetTopologySuite.Geometries.Coordinate(pt.X + half, pt.Y + half),
                                    new NetTopologySuite.Geometries.Coordinate(pt.X - half, pt.Y + half),
                                    new NetTopologySuite.Geometries.Coordinate(pt.X - half, pt.Y - half)
                                ]);
                                polygons.Add(square);
                            }
                            else if (cluster.Count == 2)
                            {
                                var p1 = cluster[0];
                                var p2 = cluster[1];
                                var line = geometryFactory.CreateLineString([p1.Coordinate, p2.Coordinate]);

                                var buffered = line.Buffer(_lineBufferRadius, NetTopologySuite.Operation.Buffer.EndCapStyle.Flat);

                                if (buffered is NetTopologySuite.Geometries.Polygon poly && !poly.IsEmpty)
                                {
                                    polygons.Add(poly);
                                }
                                else if (buffered is NetTopologySuite.Geometries.MultiPolygon mp && mp.NumGeometries > 0)
                                {
                                    polygons.AddRange(mp.Geometries.Cast<NetTopologySuite.Geometries.Polygon>().ToList());
                                }
                            }
                            else
                            {
                                var poly = CreateConcaveHullFromPoints(cluster, 0.3);
                                if (poly != null && poly.Count > 0)
                                {
                                    polygons.AddRange(poly);
                                }
                            }

                        }

                        foreach (var p in polygons)
                        {
                            var newPolygon = new AnomalyPolygon()
                            {
                                OriginalDate = points[0].CreateAt,
                                ProductSourceType = productSourceType,
                                SatelliteProductId = points[0].SatelliteProductId,
                                AnomalyType = anomalyType,
                                Color = _colors.GetValueOrDefault(anomalyType, string.Empty),
                                Polygon = p,
                                RegionId = points[0].RegionId,
                                FieldId = points[0].FieldId,
                                SeasonId = points[0].SeasonId,
                                CreateAt = DateTime.UtcNow,
                                CreateUser = lastUpdateUser.Id,
                                LastUpdateUser = lastUpdateUser.Id,
                                LastUpdateAt = DateTime.UtcNow,
                                Status = StatusType.Active
                            };

                            await IndicesDbContext.AnomalyPolygons.AddAsync(newPolygon, cancellationToken);
                            await IndicesDbContext.SaveChangesAsync(cancellationToken);

                            var currentPolygonPoints = points.Where(p => newPolygon.Polygon.Covers(p.Point)).ToList();
                            foreach (var pn in currentPolygonPoints)
                            {
                                pn.AnomalyPolygonId = newPolygon.Id;
                            }

                            await IndicesDbContext.AnomalyPoints.AddRangeAsync(currentPolygonPoints, cancellationToken);
                            await IndicesDbContext.SaveChangesAsync(cancellationToken);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                throw ex;
            }
        }

        private static double Quantile(double[] data, double q)
        {
            if (data.Length == 0)
            {
                return double.NaN;
            }

            return Accord.Statistics.Measures.Quantile(data, q);
        }

        private static List<List<NetTopologySuite.Geometries.Point>> ClusterPointsByDistance(List<NetTopologySuite.Geometries.Point> points, double maxDistance)
        {
            if (points.Count == 0) return new();

            var clusters = new List<List<NetTopologySuite.Geometries.Point>>();
            var visited = new bool[points.Count];
            var tree = new NetTopologySuite.Index.Strtree.STRtree<NetTopologySuite.Geometries.Point>();

            for (int i = 0; i < points.Count; i++)
                tree.Insert(points[i].EnvelopeInternal, points[i]);

            for (int i = 0; i < points.Count; i++)
            {
                if (visited[i]) continue;

                var cluster = new List<NetTopologySuite.Geometries.Point>();
                var queue = new Queue<NetTopologySuite.Geometries.Point>();
                queue.Enqueue(points[i]);
                visited[i] = true;

                while (queue.TryDequeue(out var current))
                {
                    cluster.Add(current);
                    var env = new NetTopologySuite.Geometries.Envelope(
                        current.X - maxDistance, current.X + maxDistance,
                        current.Y - maxDistance, current.Y + maxDistance);

                    var neighbors = tree.Query(env)
                        .OfType<NetTopologySuite.Geometries.Point>()
                        .Where(p => !visited[points.IndexOf(p)] && p.Distance(current) <= maxDistance);

                    foreach (var neighbor in neighbors)
                    {
                        int idx = points.IndexOf(neighbor);
                        if (!visited[idx])
                        {
                            visited[idx] = true;
                            queue.Enqueue(neighbor);
                        }
                    }
                }

                clusters.Add(cluster);
            }

            return clusters;
        }

        private static List<NetTopologySuite.Geometries.Polygon>? CreateConcaveHullFromPoints(List<NetTopologySuite.Geometries.Point> points, double ratio = 0.25)
        {
            if (points.Count < 3)
            {
                return null;
            }

            var geometryFactory = new NetTopologySuite.Geometries.GeometryFactory();
            var multiPoint = geometryFactory.CreateMultiPoint(points.ToArray());

            var concaveHull = NetTopologySuite.Algorithm.Hull.ConcaveHull.ConcaveHullByLengthRatio(multiPoint, ratio);

            if (concaveHull is NetTopologySuite.Geometries.Polygon polygon)
            {
                polygon.SRID = _srid;
                return [polygon];
            }

            if (concaveHull is NetTopologySuite.Geometries.MultiPolygon mp)
            {
                var pols = mp.Geometries.Cast<NetTopologySuite.Geometries.Polygon>().ToList();
                foreach (var p in pols)
                {
                    p.SRID = _srid;
                }

                return pols;
            }

            return null;
        }
    }
}
