using Masofa.Common.Models.CropMonitoring;
using Masofa.Common.Models.Satellite;
using Masofa.Common.Models.Satellite.Indices;
using Masofa.DataAccess;
using Masofa.Web.Monolith.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using OSGeo.GDAL;
using OSGeo.OSR;

namespace MasofaIndices
{
    public class IndexProcessor
    {
        private readonly GdalInitializer GdalInitializer;
        private readonly MasofaCropMonitoringDbContext MasofaCropMonitoringdbContext;
        private readonly MasofaIndicesDbContext MasofaIndicesDbContext;
        private readonly ILogger<IndexProcessor> _logger;

        public IndexProcessor(MasofaCropMonitoringDbContext dbContext, MasofaIndicesDbContext masofaIndicesDbContext, ILogger<IndexProcessor> logger, GdalInitializer gdalInitializer)
        {
            MasofaIndicesDbContext = masofaIndicesDbContext;
            MasofaCropMonitoringdbContext = dbContext;
            _logger = logger;
            GdalInitializer = gdalInitializer;
        }

        public async Task ProcessAsync()
        {
            string sentinelFolder = @"C:\Users\eminm\Downloads\00665d12-3feb-4703-a147-58a387200f03\S2B_MSIL1C_20250604T060629_N0511_R134_T42SWF_20250604T070121.SAFE\GRANULE\L1C_T42SWF_A043060_20250604T060630\IMG_DATA";

            var b04Files = Directory.GetFiles(sentinelFolder, "*B04.jp2", SearchOption.AllDirectories).ToList();
            var b08Files = Directory.GetFiles(sentinelFolder, "*B08.jp2", SearchOption.AllDirectories).ToList();

            if (!b04Files.Any() || !b08Files.Any())
            {
                _logger.LogWarning("B04 or B08 not found.");
                return;
            }

            var granules = MatchGranules(b04Files, b08Files);
            if (granules.Count == 0)
            {
                _logger.LogWarning("No matching granules.");
                return;
            }

            var fields = await MasofaCropMonitoringdbContext.Set<Field>()
                .Where(f => f.Polygon != null)
                .ToListAsync();

            _logger.LogInformation($"Loaded {fields.Count} fields from database.");

            foreach (var granule in granules)
            {
                _logger.LogInformation($"Processing granule: {Path.GetFileName(granule.B04Path)}");

                try
                {
                    using var b04Dataset = GdalInitializer.Open(granule.B04Path, Access.GA_ReadOnly);
                    using var b08Dataset = GdalInitializer.Open(granule.B08Path, Access.GA_ReadOnly);

                    if (b04Dataset == null || b08Dataset == null)
                    {
                        throw new Exception("Can't open datasets");
                    }

                    var acqUtc = ExtractAcquisitionUtcFromPath(granule.B04Path);

                    await EnsureMonthlyPartitionsAsync(MasofaIndicesDbContext, acqUtc);

                    int width = b04Dataset.RasterXSize;
                    int height = b04Dataset.RasterYSize;

                    double[] geoTransform = new double[6];
                    b04Dataset.GetGeoTransform(geoTransform);

                    var b04Band = b04Dataset.GetRasterBand(1);
                    var b08Band = b08Dataset.GetRasterBand(1);

                    var b04Buffer = new float[width * height];
                    var b08Buffer = new float[width * height];

                    b04Band.ReadRaster(0, 0, width, height, b04Buffer, width, height, 0, 0);
                    b08Band.ReadRaster(0, 0, width, height, b08Buffer, width, height, 0, 0);

                    string wkt = b04Dataset.GetProjection();
                    var rasterSrs = new SpatialReference(wkt);
                    var wgs84Srs = new SpatialReference(string.Empty);
                    wgs84Srs.SetWellKnownGeogCS("WGS84");

                    foreach (var field in fields)
                    {
                        _logger.LogInformation($"Processing field: {field.Name}");

                        var polygonWgs84 = field.Polygon;
                        var env = polygonWgs84.EnvelopeInternal;
                        var (minX, minY) = TransformCoord(env.MinX, env.MinY, wgs84Srs, rasterSrs);
                        var (maxX, maxY) = TransformCoord(env.MaxX, env.MaxY, wgs84Srs, rasterSrs);

                        int xStart = (int)Math.Floor((minX - geoTransform[0]) / geoTransform[1]);
                        int xEnd = (int)Math.Ceiling((maxX - geoTransform[0]) / geoTransform[1]);
                        int yStart = (int)Math.Floor((maxY - geoTransform[3]) / geoTransform[5]);
                        int yEnd = (int)Math.Ceiling((minY - geoTransform[3]) / geoTransform[5]);

                        xStart = Math.Max(0, xStart);
                        xEnd = Math.Min(width, xEnd);
                        yStart = Math.Max(0, yStart);
                        yEnd = Math.Min(height, yEnd);

                        var pointList = new List<NdviPoint>();
                        var polygonEntity = new NdviPolygon
                        {
                            CreateAt = acqUtc, // TODO: Заменить на дату снимка!
                            ProductSourceType = ProductSourceType.Sentinel2,
                            SatelliteProductId = Guid.NewGuid(), // TODO: Получить реальный ID
                            FileStorageItemId = Guid.NewGuid(), // TODO: Получить реальный ID файла
                            Polygon = polygonWgs84
                        };

                        for (int y = yStart; y < yEnd; y++)
                        {
                            for (int x = xStart; x < xEnd; x++)
                            {
                                double pixelCenterX = geoTransform[0] + (x + 0.5) * geoTransform[1];
                                double pixelCenterY = geoTransform[3] + (y + 0.5) * geoTransform[5];

                                var (lon, lat) = TransformCoord(pixelCenterX, pixelCenterY, rasterSrs, wgs84Srs);
                                var pointGeom = new Point(lon, lat);

                                if (!polygonWgs84.Contains(pointGeom))
                                    continue;

                                int i = y * width + x;
                                float b04 = b04Buffer[i];
                                float b08 = b08Buffer[i];

                                if (b04 <= 0 || b08 <= 0)
                                    continue;

                                var point = new NdviPoint
                                {
                                    Id = Guid.NewGuid(),
                                    CreateAt = acqUtc,
                                    ProductSourceType = ProductSourceType.Sentinel2,
                                    SatelliteProductId = polygonEntity.SatelliteProductId,
                                    Point = pointGeom,
                                    BZero4 = b04,
                                    BZero8 = b08,
                                    EPS = 1e-9f,
                                    RegionId = field.RegionId,
                                    FieldId = field.Id,
                                    //SeasonId = field.SeasonId
                                };

                                pointList.Add(point);
                            }
                        }

                        _logger.LogInformation($"Found {pointList.Count} points for field '{field.Name}'.");

                        if (pointList.Count > 0)
                        {
                            await MasofaIndicesDbContext.AddAsync(polygonEntity);
                            await MasofaIndicesDbContext.AddRangeAsync(pointList);

                            var relations = pointList.Select(p => new NdviPolygonRelation
                            {
                                Id = Guid.NewGuid(),
                                NdViId = p.Id,
                                RegionId = p.RegionId,
                                FieldId = p.FieldId,
                                SeasonId = p.SeasonId
                            }).ToList();
                            await MasofaIndicesDbContext.AddRangeAsync(relations);

                            await MasofaIndicesDbContext.SaveChangesAsync();

                            _logger.LogInformation($"Saved 1 polygon + {pointList.Count} points to database.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error processing granule: {Path.GetFileName(granule.B04Path)}");
                }
            }

            _logger.LogInformation("Processing completed!");
        }

        private static (double X, double Y) TransformCoord(double x, double y, SpatialReference src, SpatialReference dst)
        {
            var ct = new CoordinateTransformation(src, dst);
            double[] point = { x, y, 0 };
            ct.TransformPoint(point);
            return (point[0], point[1]);
        }

        private static List<(string B04Path, string B08Path)> MatchGranules(List<string> b04Files, List<string> b08Files)
        {
            var granules = new List<(string B04Path, string B08Path)>();

            foreach (var b04 in b04Files)
            {
                string fileName = Path.GetFileName(b04);
                if (!fileName.EndsWith("_B04.jp2")) continue;

                string basePart = fileName.Replace("_B04.jp2", "");
                string expectedB08 = basePart + "_B08.jp2";

                string b08Match = b08Files.FirstOrDefault(f => Path.GetFileName(f) == expectedB08);

                if (b08Match != null)
                {
                    granules.Add((b04, b08Match));
                }
            }

            return granules;
        }


        public static async Task EnsureMonthlyPartitionsAsync(DbContext db, DateTime utc)
        {
            var start = new DateTime(utc.Year, utc.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var next = start.AddMonths(1);

            var suffix = $"{start:yyyy_MM}";

            var sqlPoints = $@"
            CREATE TABLE IF NOT EXISTS ""NdviPoints_{suffix}"" PARTITION OF ""NdviPoints""
            FOR VALUES FROM ('{start:yyyy-MM-dd}') TO ('{next:yyyy-MM-dd}');
            ";

            var sqlPolygons = $@"
            CREATE TABLE IF NOT EXISTS ""NdviPolygons_{suffix}"" PARTITION OF ""NdviPolygons""
            FOR VALUES FROM ('{start:yyyy-MM-dd}') TO ('{next:yyyy-MM-dd}');
            ";

            await db.Database.ExecuteSqlRawAsync(sqlPoints);
            await db.Database.ExecuteSqlRawAsync(sqlPolygons);
        }

        private static DateTime ExtractAcquisitionUtcFromPath(string path)
        {
            // Ищем токен вида 20250604T060629
            var m = System.Text.RegularExpressions.Regex.Match(path, @"\b(\d{8}T\d{6})\b");
            if (!m.Success) return DateTime.UtcNow; // запасной вариант, но лучше логировать

            var s = m.Groups[1].Value; // 20250604T060629
                                       // Парсим как UTC
            return DateTime.SpecifyKind(
                DateTime.ParseExact(s, "yyyyMMdd'T'HHmmss", null),
                DateTimeKind.Utc);
        }
    }
}
