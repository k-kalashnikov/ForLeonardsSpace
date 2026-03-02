using Masofa.BusinessLogic.CropMonitoring;
using Masofa.BusinessLogic.CropMonitoring.Fields;
using Masofa.BusinessLogic.Era;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using Masofa.Common.Models.Era;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Masofa.Web.Monolith.Controllers.Era
{
    /// <summary>
    /// Контроллер для работы с погодными точками Era5
    /// </summary>
    /// [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("weather/[controller]")]
    [ApiExplorerSettings(GroupName = "Weather")]
    //public class EraWeatherStationController : BaseCrudController<EraWeatherStation, MasofaEraDbContext>
    public class EraWeatherStationController : BaseController
    {
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }
        private MasofaEraDbContext EraDbContext { get; }

        public EraWeatherStationController(
            ILogger<EraWeatherStationController> logger,
            IConfiguration configuration,
            IMediator mediator,
            MasofaEraDbContext eraDbContext,
            IBusinessLogicLogger businessLogicLogger) : base(
                logger,
                configuration,
                mediator)
        {
            EraDbContext = eraDbContext;
            BusinessLogicLogger = businessLogicLogger;
        }

        [HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("[action]/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<EraWeatherStationReturnResult>> GetById(Guid id)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetById)}";
            try
            {
                var station = await EraDbContext.EraWeatherStations.FindAsync(id);
                if (station == null)
                {
                    return NotFound();
                }

                return new EraWeatherStationReturnResult(station);
            }
            catch (Exception ex)
            {
                var msg = $"Something wrong in {requestPath}. {ex.Message}";
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("[action]/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<List<EraWeatherStationReturnResult>>> GetByRegionId(Guid id)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetByRegionId)}";
            try
            {
                return await EraDbContext.EraWeatherStations.Where(s => s.RegionId == id).Select(s => new EraWeatherStationReturnResult(s)).ToListAsync();
            }
            catch (Exception ex)
            {
                var msg = $"Something wrong in {requestPath}. {ex.Message}";
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<List<EraWeatherStationReturnResult>>> GetAllStations()
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetAllStations)}";
            try
            {
                return await EraDbContext.EraWeatherStations.Select(s => new EraWeatherStationReturnResult(s)).ToListAsync();
            }
            catch (Exception ex)
            {
                var msg = $"Something wrong in {requestPath}. {ex.Message}";
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<EraWeatherStationReturnResult>> GetByCoordinates([FromQuery] double latitude, [FromQuery] double longitude)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetByCoordinates)}";
            try
            {
                var station = await Mediator.Send(new EraWeatherStationGetByCoordinatesCommand()
                {
                    Latitude = latitude,
                    Longitude = longitude
                });

                if (station == null)
                {
                    return NotFound();
                }

                return new EraWeatherStationReturnResult(station);
            }
            catch (Exception ex)
            {
                var msg = $"Something wrong in {requestPath}. {ex.Message}";
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        public async Task<ActionResult<Dictionary<Guid, int>>> GetEraWeatherStationsByRegions([FromBody] RegionIdsRequest<EraWeatherStation> viewModel)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetEraWeatherStationsByRegions)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync($"Start request in {requestPath}", requestPath);

                var stationsByQuery = EraDbContext.EraWeatherStations;
                foreach (var item in viewModel.Query?.Filters ?? new List<FieldFilter>())
                {
                    stationsByQuery.ApplyFiltering(item);
                }

                stationsByQuery.Where(s => viewModel.RegionIds.Contains(s.RegionId));
                var stations = await stationsByQuery.ToListAsync();
                var result = new Dictionary<Guid, int>();
                foreach (var station in stations)
                {
                    if (!result.ContainsKey(station.RegionId))
                    {
                        result[station.RegionId] = 0;
                    }

                    result[station.RegionId] += 1;
                }

                await BusinessLogicLogger.LogInformationAsync($"Finish request in {requestPath} with result: {Newtonsoft.Json.JsonConvert.SerializeObject(result)}", requestPath);

                return Ok(result);
            }
            catch (Exception ex)
            {
                var msg = $"Something wrong in {requestPath}. {ex.Message}";
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        public async Task<ActionResult<List<EraWeatherStationReturnResult>>> GetStationsByCoordinates([FromBody] BoundingBoxRequest<EraWeatherStation> viewModel)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetStationsByCoordinates)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync($"Start request in {requestPath}", requestPath);

                if (viewModel.West >= viewModel.East || viewModel.South >= viewModel.North)
                {
                    return BadRequest("Invalid bounding box coordinates.");
                }

                // Преобразуем decimal → double
                var west = (double)viewModel.West;
                var east = (double)viewModel.East;
                var south = (double)viewModel.South;
                var north = (double)viewModel.North;

                // Создаём углы полигона (по часовой стрелке)
                var coordinates = new[]
                {
                    new NetTopologySuite.Geometries.Coordinate(west, south),
                    new NetTopologySuite.Geometries.Coordinate(east, south),
                    new NetTopologySuite.Geometries.Coordinate(east, north),
                    new NetTopologySuite.Geometries.Coordinate(west, north),
                    new NetTopologySuite.Geometries.Coordinate(west, south)
                };

                var geometryFactory = new NetTopologySuite.Geometries.GeometryFactory(new NetTopologySuite.Geometries.PrecisionModel(), 4326);
                var boundingBox = geometryFactory.CreatePolygon(coordinates);

                var stationsByQuery = EraDbContext.EraWeatherStations;
                foreach (var item in viewModel.Query?.Filters ?? new List<FieldFilter>())
                {
                    stationsByQuery.ApplyFiltering(item);
                }

                stationsByQuery.Where(s => boundingBox.Covers(s.Point));
                var result = await stationsByQuery.Select(s => new EraWeatherStationReturnResult(s)).ToListAsync();

                if (result == null)
                {
                    return NotFound();
                }

                await BusinessLogicLogger.LogInformationAsync($"Finish request in {requestPath} with result: {Newtonsoft.Json.JsonConvert.SerializeObject(result)}", requestPath);

                return Ok(result);
            }
            catch (Exception ex)
            {
                var msg = $"Something wrong in {requestPath}. {ex.Message}";
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }

    public class EraWeatherStationReturnResult(EraWeatherStation station)
    {
        public Guid Id { get; set; } = station.Id;
        public DateTime CreateAt { get; set; } = station.CreateAt;
        public StatusType Status { get; set; } = station.Status;
        public DateTime LastUpdateAt { get; set; } = station.LastUpdateAt;
        public Guid CreateUser { get; set; } = station.CreateUser;
        public Guid LastUpdateUser { get; set; } = station.LastUpdateUser;
        public double Latitude { get; set; } = station.Point.Y;
        public double Longitude { get; set; } = station.Point.X;
        public Guid RegionId { get; set; } = station.RegionId;
    }
}
