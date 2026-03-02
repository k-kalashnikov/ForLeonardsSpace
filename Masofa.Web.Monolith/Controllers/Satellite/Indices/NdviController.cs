using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.CropMonitoring;
using Masofa.Common.Models.Satellite;
using Masofa.Common.Models.Satellite.Indices;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MaxRev.Gdal.Core;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using OSGeo.GDAL;
using OSGeo.OSR;

namespace Masofa.Web.Monolith.Controllers.Satellite.Indices
{
    [Route("satellite/[controller]")]
    [ApiExplorerSettings(GroupName = "SatelliteIndices")]
    public class NdviController : BaseIndexesController<NdviPoint, NdviPolygon, NdviSeasonReport, NdviSharedReport>
    {

        public NdviController(ILogger<NdviController> logger, 
            IConfiguration configuration, 
            IMediator mediator, 
            MasofaIndicesDbContext masofaIndicesDbContext, 
            IFileStorageProvider fileStorageProvider, 
            IBusinessLogicLogger businessLogicLogger, 
            IHttpContextAccessor httpContextAccessor) : base(logger, configuration, mediator, masofaIndicesDbContext, fileStorageProvider, businessLogicLogger, httpContextAccessor)
        {

        }
        ///// <summary>
        ///// Запускает обработку NDVI для всех полигонов и доступных снимков Sentinel-2
        ///// </summary>
        //[HttpPost("process")]
        //public async Task<IActionResult> Process()
        //{
        //    try
        //    {
        //        _logger.LogInformation("Starting NDVI processing via API...");

        //        string sentinelFolder = @"C:\Users\eminm\Downloads\00665d12-3feb-4703-a147-58a387200f03\S2B_MSIL1C_20250604T060629_N0511_R134_T42SWF_20250604T070121.SAFE\GRANULE\L1C_T42SWF_A043060_20250604T060630\IMG_DATA";

        //        var b04Files = Directory.GetFiles(sentinelFolder, "*B04.jp2", SearchOption.AllDirectories).ToList();
        //        var b08Files = Directory.GetFiles(sentinelFolder, "*B08.jp2", SearchOption.AllDirectories).ToList();

        //        if (!b04Files.Any() || !b08Files.Any())
        //        {
        //            _logger.LogWarning("B04 or B08 files not found.");
        //            return BadRequest("B04 or B08 files not found in the specified folder.");
        //        }

        //        var granules = MatchGranules(b04Files, b08Files);
        //        if (granules.Count == 0)
        //        {
        //            _logger.LogWarning("No matching B04/B08 granules found.");
        //            return BadRequest("No matching B04/B08 granules found.");
        //        }

        //        var fields = await MasofaCropMonitoringDbContext.Set<Field>()
        //            .Where(f => f.Polygon != null)
        //            .ToListAsync();

        //        if (!fields.Any())
        //        {
        //            _logger.LogWarning("No fields with geometry found in database.");
        //            return BadRequest("No fields with geometry found in database.");
        //        }

        //        _logger.LogInformation($"Loaded {fields.Count} fields from database.");

        //        int totalPointsProcessed = 0;

        //        foreach (var granule in granules)
        //        {
        //            _logger.LogInformation($"Processing granule: {Path.GetFileName(granule.B04Path)}");

        //            try
        //            {
        //                using var b04Dataset = Gdal.Open(granule.B04Path, Access.GA_ReadOnly);
        //                using var b08Dataset = Gdal.Open(granule.B08Path, Access.GA_ReadOnly);

        //                if (b04Dataset == null || b08Dataset == null)
        //                {
        //                    _logger.LogWarning($"Failed to open datasets for {granule.B04Path}");
        //                    continue;
        //                }

        //                int width = b04Dataset.RasterXSize;
        //                int height = b04Dataset.RasterYSize;

        //                double[] geoTransform = new double[6];
        //                b04Dataset.GetGeoTransform(geoTransform);

        //                var b04Band = b04Dataset.GetRasterBand(1);
        //                var b08Band = b08Dataset.GetRasterBand(1);

        //                var b04Buffer = new float[width * height];
        //                var b08Buffer = new float[width * height];

        //                b04Band.ReadRaster(0, 0, width, height, b04Buffer, width, height, 0, 0);
        //                b08Band.ReadRaster(0, 0, width, height, b08Buffer, width, height, 0, 0);

        //                string wkt = b04Dataset.GetProjection();
        //                var rasterSrs = new SpatialReference(wkt);
        //                var wgs84Srs = new SpatialReference(string.Empty);
        //                wgs84Srs.SetWellKnownGeogCS("WGS84");

        //                foreach (var field in fields)
        //                {
        //                    _logger.LogInformation($"Processing field: {field.Name}");

        //                    var polygonWgs84 = field.Polygon;
        //                    var env = polygonWgs84.EnvelopeInternal;
        //                    var (minX, minY) = TransformCoord(env.MinX, env.MinY, wgs84Srs, rasterSrs);
        //                    var (maxX, maxY) = TransformCoord(env.MaxX, env.MaxY, wgs84Srs, rasterSrs);

        //                    int xStart = (int)Math.Floor((minX - geoTransform[0]) / geoTransform[1]);
        //                    int xEnd = (int)Math.Ceiling((maxX - geoTransform[0]) / geoTransform[1]);
        //                    int yStart = (int)Math.Floor((maxY - geoTransform[3]) / geoTransform[5]);
        //                    int yEnd = (int)Math.Ceiling((minY - geoTransform[3]) / geoTransform[5]);

        //                    xStart = Math.Max(0, xStart);
        //                    xEnd = Math.Min(width, xEnd);
        //                    yStart = Math.Max(0, yStart);
        //                    yEnd = Math.Min(height, yEnd);

        //                    var pointList = new List<NdviPoint>();
        //                    var polygonEntity = new NdviPolygon
        //                    {
        //                        Id = Guid.NewGuid(),
        //                        CreateAt = DateTime.UtcNow, // ← Замени на дату из имени файла!
        //                        ProductSourceType = ProductSourceType.Sentinel2,
        //                        SatelliteProductId = Guid.NewGuid(), // ← Получи реальный ID снимка
        //                        FileStorageItemId = Guid.NewGuid(), // ← Получи реальный ID файла
        //                        Polygon = polygonWgs84
        //                    };

        //                    for (int y = yStart; y < yEnd; y++)
        //                    {
        //                        for (int x = xStart; x < xEnd; x++)
        //                        {
        //                            double pixelCenterX = geoTransform[0] + (x + 0.5) * geoTransform[1];
        //                            double pixelCenterY = geoTransform[3] + (y + 0.5) * geoTransform[5];

        //                            var (lon, lat) = TransformCoord(pixelCenterX, pixelCenterY, rasterSrs, wgs84Srs);
        //                            var pointGeom = new Point(lon, lat);

        //                            if (!polygonWgs84.Contains(pointGeom))
        //                                continue;

        //                            int i = y * width + x;
        //                            float b04 = b04Buffer[i];
        //                            float b08 = b08Buffer[i];

        //                            if (b04 <= 0 || b08 <= 0)
        //                                continue;

        //                            var point = new NdviPoint
        //                            {
        //                                Id = Guid.NewGuid(),
        //                                CreateAt = polygonEntity.CreateAt,
        //                                ProductSourceType = ProductSourceType.Sentinel2,
        //                                SatelliteProductId = polygonEntity.SatelliteProductId,
        //                                Point = pointGeom,
        //                                BZero4 = b04,
        //                                BZero8 = b08,
        //                                EPS = 1e-9f,
        //                                RegionId = field.RegionId,
        //                                FieldId = field.Id,
        //                                SeasonId = field.SeasonId
        //                            };

        //                            pointList.Add(point);
        //                        }
        //                    }

        //                    _logger.LogInformation($"Found {pointList.Count} points for field '{field.Name}'.");

        //                    if (pointList.Count > 0)
        //                    {
        //                        await MasofaIndicesDbContext.AddAsync(polygonEntity);
        //                        await MasofaIndicesDbContext.AddRangeAsync(pointList);
        //                        await MasofaIndicesDbContext.SaveChangesAsync();

        //                        totalPointsProcessed += pointList.Count;

        //                        _logger.LogInformation($"Saved 1 polygon + {pointList.Count} points to database.");
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                _logger.LogError(ex, $"Error processing granule: {Path.GetFileName(granule.B04Path)}");
        //                Продолжаем обработку следующих гранул
        //            }
        //        }

        //        _logger.LogInformation($"✅ NDVI processing completed. Total points saved: {totalPointsProcessed}");
        //        return Ok(new { Message = "NDVI processing completed successfully.", TotalPoints = totalPointsProcessed });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Unexpected error in NDVI processing");
        //        return StatusCode(500, "Internal server error during NDVI processing.");
        //    }
        //}

        //private static (double X, double Y) TransformCoord(double x, double y, SpatialReference src, SpatialReference dst)
        //{
        //    var ct = new CoordinateTransformation(src, dst);
        //    double[] point = { x, y, 0 };
        //    ct.TransformPoint(point);
        //    return (point[0], point[1]);
        //}

        //private static List<(string B04Path, string B08Path)> MatchGranules(List<string> b04Files, List<string> b08Files)
        //{
        //    var granules = new List<(string B04Path, string B08Path)>();

        //    foreach (var b04 in b04Files)
        //    {
        //        string fileName = Path.GetFileName(b04);
        //        if (!fileName.EndsWith("_B04.jp2")) continue;

        //        string basePart = fileName.Replace("_B04.jp2", "");
        //        string expectedB08 = basePart + "_B08.jp2";

        //        string b08Match = b08Files.FirstOrDefault(f => Path.GetFileName(f) == expectedB08);

        //        if (b08Match != null)
        //        {
        //            granules.Add((b04, b08Match));
        //        }
        //    }

        //    return granules;
        //}

    }
}
