using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Era;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using Masofa.Web.Monolith.ViewModels.WeatherReport;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Masofa.Web.Monolith.Controllers.WeatherReport
{
    [Route("weatherReports/[controller]")]
    [ApiExplorerSettings(GroupName = "WeatherReports")]
    public class Era5DayNormalizedWeatherController : BaseEra5WeatherReportController<Era5DayNormalizedWeather, MasofaEraDbContext>
    {
        private readonly double _step;

        public Era5DayNormalizedWeatherController(
            IFileStorageProvider fileStorageProvider,
            MasofaEraDbContext dbContext,
            ILogger<Era5DayNormalizedWeatherController> logger,
            IConfiguration configuration,
            IMediator mediator,
            IBusinessLogicLogger businessLogicLogger,
            IHttpContextAccessor httpContextAccessor) : base(fileStorageProvider, dbContext, logger, configuration, mediator, businessLogicLogger, httpContextAccessor)
        {
            _step = configuration.GetValue<double>("CountryBoundaries:Step");
        }

        /// <summary>
        /// Получает список сущностей по заданному запросу с фильтрацией, сортировкой и пагинацией
        /// </summary>
        /// <param name="query">Объект запроса с параметрами фильтрации, сортировки и пагинации</param>
        /// <returns>Список найденных сущностей</returns>
        /// <response code="200">Успешно получен список сущностей</response>
        /// <response code="400">Некорректные параметры запроса</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        public virtual async Task<ActionResult<Era5DayNormalizedWeather>> GetByCoordinatesAndDate([FromBody] CoordinatesAndDateViewModel query)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetByCoordinatesAndDate)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                if (!ModelState.IsValid)
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelValidationFailed(requestPath, Newtonsoft.Json.JsonConvert.SerializeObject(query)), requestPath);
                    return BadRequest(ModelState);
                }

                var targetPoint = new Point(query.Longitude, query.Latitude)
                {
                    SRID = 4326
                };

                var closestStation = await DbContext.EraWeatherStations
                    .OrderBy(s => s.Point.Distance(targetPoint))
                    .FirstOrDefaultAsync();

                if (closestStation is null)
                {
                    return NotFound();
                }

                var inputDate = DateOnly.FromDateTime(query.InputDate);

                var closestFutureReport = await DbContext.Era5DayNormalizedWeather
                    .Where(x => x.WeatherStation == closestStation.Id && x.Day >= inputDate.Day && x.Month >= inputDate.Month)
                    .OrderBy(x => x.Month).ThenBy(x => x.Day)
                    .FirstOrDefaultAsync();

                if (closestFutureReport is null)
                {
                    return NotFound();
                }

                return closestFutureReport;
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath,ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Получает список сущностей по заданному запросу с фильтрацией, сортировкой и пагинацией
        /// </summary>
        /// <param name="query">Объект запроса с параметрами фильтрации, сортировки и пагинации</param>
        /// <returns>Список найденных сущностей</returns>
        /// <response code="200">Успешно получен список сущностей</response>
        /// <response code="400">Некорректные параметры запроса</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        public virtual async Task<ActionResult<List<Era5DayNormalizedWeather>>> GetListByCoordinates([FromBody] ListCoordinatesViewModel query)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetByCoordinatesAndDate)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                if (!ModelState.IsValid)
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelValidationFailed(requestPath, Newtonsoft.Json.JsonConvert.SerializeObject(query)), requestPath);
                    return BadRequest(ModelState);
                }

                var targetPoint = new Point(query.Longitude, query.Latitude)
                {
                    SRID = 4326
                };

                var closestStation = await DbContext.EraWeatherStations
                    .OrderBy(s => s.Point.Distance(targetPoint))
                    .FirstOrDefaultAsync();

                if (closestStation is null)
                {
                    return NotFound();
                }

                var closestFutureReport = await DbContext.Era5DayNormalizedWeather
                    .Where(x => x.WeatherStation == closestStation.Id)
                    .OrderBy(x => x.Month).ThenBy(x => x.Day)
                    .ToListAsync();

                if (closestFutureReport is null)
                {
                    return NotFound();
                }

                return closestFutureReport;
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Получает прогноз на день на ближайшую точку по координатам и времени
        /// </summary>
        /// <param name="query">Объект запроса с параметрами фильтрации, сортировки и пагинации</param>
        /// <returns>Список найденных сущностей</returns>
        /// <response code="200">Успешно получен список сущностей</response>
        /// <response code="400">Некорректные параметры запроса</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        public virtual async Task<ActionResult<Era5DayNormalizedWeather>> GetInterpolatedByCoordinatesAndDate([FromBody] CoordinatesAndDateViewModel query)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetByCoordinatesAndDate)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                if (!ModelState.IsValid)
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelValidationFailed(requestPath, Newtonsoft.Json.JsonConvert.SerializeObject(query)), requestPath);
                    return BadRequest(ModelState);
                }

                var targetPoint = new Point(query.Longitude, query.Latitude) { SRID = 4326 };

                var closestStation = await DbContext.EraWeatherStations
                    .OrderBy(s => s.Point.Distance(targetPoint))
                    .FirstOrDefaultAsync();

                if (closestStation is null)
                {
                    return NotFound();
                }

                var nearestStations = await DbContext.EraWeatherStations.Where(s =>
                        (s.Point.X == closestStation.Point.X && s.Point.Y == closestStation.Point.Y + _step) ||
                        (s.Point.X == closestStation.Point.X && s.Point.Y == closestStation.Point.Y - _step) ||
                        (s.Point.X == closestStation.Point.X + _step && s.Point.Y == closestStation.Point.Y) ||
                        (s.Point.X == closestStation.Point.X - _step && s.Point.Y == closestStation.Point.Y))
                    .ToListAsync();

                var inputDate = DateOnly.FromDateTime(query.InputDate);

                var reports = DbContext.Era5DayWeatherReports
                    .Where(f => f.WeatherStation != null && nearestStations.Select(s => s.Id).Contains(f.WeatherStation.Value)
                                && f.Date >= inputDate)
                    .OrderBy(f => f.Date)
                    .GroupBy(f => f.Date)
                    .ToDictionary(x => x.Key, x => x.ToList());

                while (!reports.ContainsKey(inputDate) && inputDate < DateOnly.FromDateTime(query.InputDate).AddMonths(1))
                {
                    inputDate = inputDate.AddDays(1);
                }

                var closestFutureReports = reports[inputDate];

                if (closestFutureReports is null)
                {
                    return NotFound();
                }

                return new Era5DayNormalizedWeather()
                {
                    Id = Guid.Empty,
                    Day = inputDate.Day,
                    Month = inputDate.Month,
                    TemperatureMin = closestFutureReports.Average(r => r.TemperatureMin),
                    TemperatureMax = closestFutureReports.Average(r => r.TemperatureMax),
                    TemperatureMinTotal = closestFutureReports.Average(r => r.TemperatureMinTotal),
                    TemperatureMaxTotal = closestFutureReports.Average(r => r.TemperatureMaxTotal),
                    SolarRadiationInfluence = closestFutureReports.Average(r => r.SolarRadiationInfluence),
                    Fallout = closestFutureReports.Average(r => r.Fallout),
                    Humidity = closestFutureReports.Average(r => r.Humidity),
                    WindSpeed = closestFutureReports.Average(r => r.WindSpeed),
                    WindDerection = closestFutureReports.Average(r => r.WindDerection)
                };
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
