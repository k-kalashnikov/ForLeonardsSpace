using Masofa.BusinessLogic.Dictionaries;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Dictionaries;
using Masofa.Common.Models.Era;
using Masofa.Common.Resources;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Masofa.Web.Monolith.Controllers.Dictionaries
{
    /// <summary>
    /// Предоставляет API-методы для управления регионами
    /// </summary>
    [Route("dictionaries/[controller]")]
    [ApiExplorerSettings(GroupName = "Dictionaries")]
    public class RegionController : BaseDictionaryController<Region, MasofaDictionariesDbContext>
    {
        private MasofaCropMonitoringDbContext CropMonitoringDbContext { get; set; }
        private MasofaDictionariesDbContext DictionariesDbContext { get; set; }
        private MasofaEraDbContext EraDbContext { get; set; }

        private readonly int _srid = 4326;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="RegionController"/>.
        /// </summary>
        /// <param name="fileStorageProvider">Провайдер для работы с хранилищем файлов</param>
        /// <param name="dbContext">Контекст базы данных</param>
        /// <param name="logger">Логгер</param>
        /// <param name="configuration">Конфигурация приложения</param>
        public RegionController(
            IFileStorageProvider fileStorageProvider,
            MasofaDictionariesDbContext dbContext,
            MasofaCropMonitoringDbContext cropMonitoringDbContext,
            ILogger<RegionController> logger,
            IConfiguration configuration,
            IMediator mediator,
            IBusinessLogicLogger businessLogicLogger,
            IHttpContextAccessor httpContextAccessor,
            MasofaEraDbContext eraDbContext) : base(
                fileStorageProvider,
                dbContext,
                logger,
                configuration,
                mediator,
                businessLogicLogger,
                httpContextAccessor)
        {
            CropMonitoringDbContext = cropMonitoringDbContext;
            DictionariesDbContext = dbContext;
            EraDbContext = eraDbContext;
        }

        [HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<Region>> GetRegionByPoint([FromQuery] double longitude, [FromQuery] double latitude)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetRegionByPoint)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                var region = await Mediator.Send(new GetRegionByPointRequest()
                {
                    Latitude = latitude,
                    Longitude = longitude
                });

                if (region == null)
                {
                    return NotFound();
                }

                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, Newtonsoft.Json.JsonConvert.SerializeObject(region)), requestPath);

                return region;
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<List<Region>>> GetRegionsByCropIdAndPlantingDate([FromQuery] Guid cropId, [FromQuery] DateOnly plantingDate)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetRegionsByCropIdAndPlantingDate)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                var fieldIds = await CropMonitoringDbContext.Seasons
                    .Where(s => s.CropId == cropId && s.PlantingDate == plantingDate)
                    .Select(s => s.FieldId)
                    .ToListAsync();

                var regionIds = await CropMonitoringDbContext.Fields
                    .Where(f => fieldIds.Contains(f.Id))
                    .Select(f => f.RegionId)
                    .ToListAsync();

                var regions = await DictionariesDbContext.Regions
                    .Where(r => regionIds.Contains(r.Id))
                    .ToListAsync();

                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, regions.Count.ToString()), requestPath);

                return regions;
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("[action]/{date}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<List<Region>>> GetRegionsWithFrostDanger(DateOnly date)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetRegionsWithFrostDanger)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                var reports = await EraDbContext.Era5DayWeatherReports
                    .Where(r => r.IsFrostDanger && r.Date == date)
                    .ToListAsync();

                List<BaseEra5WeatherReport> baseReports = [];
                if (reports.Count == 0)
                {
                    var forecasts = await EraDbContext.Era5DayWeatherForecasts
                        .Where(r => r.IsFrostDanger && r.Date == date)
                        .ToListAsync();

                    foreach (var forecast in forecasts)
                    {
                        baseReports.Add(forecast);
                    }
                }
                else
                {
                    foreach (var report in reports)
                    {
                        baseReports.Add(report);
                    }
                }

                if (baseReports.Count == 0)
                {
                    return NotFound();
                }

                List<NetTopologySuite.Geometries.Point> stationPoints = [];
                var stations = await EraDbContext.EraWeatherStations
                    .Where(s => s.Point != null && baseReports.Select(r => r.WeatherStation).Contains(s.Id))
                    .ToListAsync();

                foreach (var station in stations)
                {
                    if (station.Point.SRID != _srid) station.Point.SRID = _srid;
                    stationPoints.Add(station.Point);
                }

                var regionMaps = await DbContext.RegionMaps
                    .Where(rm => rm.Polygon != null && rm.Polygon.SRID == _srid && stationPoints.Any(p => rm.Polygon.Covers(p)))
                    .ToListAsync();

                var regions = await DbContext.Regions
                    .Where(r => r.RegionMapId != null && regionMaps.Select(rm => rm.Id).Contains(r.RegionMapId.Value))
                    .ToListAsync();

                foreach (var region in regions)
                {
                    var regionMap = regionMaps.FirstOrDefault(rm => rm.Id == region.RegionMapId);
                    if (regionMap == null) continue;
                    if (regionMap.Polygon == null) continue;

                    List<NetTopologySuite.Geometries.Polygon> polygons = [];
                    if (regionMap.Polygon is NetTopologySuite.Geometries.Polygon polygon)
                    {
                        polygons.Add(polygon);
                    }
                    else if (regionMap.Polygon is NetTopologySuite.Geometries.MultiPolygon multiPolygon)
                    {
                        polygons.AddRange(multiPolygon.Geometries.Cast<NetTopologySuite.Geometries.Polygon>().ToArray());
                    }

                    region.RegionSquare = CalculateArea(polygons);
                }

                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, regions.Count.ToString()), requestPath);

                return regions;
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        private static decimal CalculateArea(List<NetTopologySuite.Geometries.Polygon> polygons)
        {
            decimal result = 0;
            foreach (var polygon in polygons)
            {
                var c = polygon.Centroid;
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

                var coordinates = polygon.ExteriorRing.Coordinates
                    .Select(coord =>
                    {
                        var point = new[] { coord.X, coord.Y };
                        var transformed = transformer.MathTransform.Transform(point);
                        return new NetTopologySuite.Geometries.Coordinate(transformed[0], transformed[1]);
                    })
                    .ToArray();

                var linearRing = geometryFactory.CreateLinearRing(coordinates);
                var transformedPolygon = geometryFactory.CreatePolygon(linearRing);

                result += (decimal)(transformedPolygon.Area / 1_000_000);
            }

            return result;
        }
    }
}
