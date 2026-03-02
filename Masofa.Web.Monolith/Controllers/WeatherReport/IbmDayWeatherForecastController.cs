using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.IBMWeather;
using Masofa.Common.Resources;
using Masofa.DataAccess;
using Masofa.Web.Monolith.ViewModels.WeatherReport;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Masofa.Web.Monolith.Controllers.WeatherReport
{
    [Route("weatherReports/[controller]")]
    [ApiExplorerSettings(GroupName = "WeatherReports")]
    public class IbmDayWeatherForecastController : BaseController
    {
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }
        private MasofaIBMWeatherDbContext DbContext { get; set; }

        private readonly double _step;

        public IbmDayWeatherForecastController(
            ILogger<IbmDayWeatherForecastController> logger,
            IConfiguration configuration,
            IMediator mediator,
            IBusinessLogicLogger businessLogicLogger,
            MasofaIBMWeatherDbContext dbContext) : base(logger, configuration, mediator)
        {
            _step = configuration.GetValue<double>("IBMWeather:Step");
            BusinessLogicLogger = businessLogicLogger;
            DbContext = dbContext;
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
        public virtual async Task<ActionResult<IbmDayWeatherForecast>> GetByCoordinatesAndDate([FromBody] CoordinatesAndDateViewModel query)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetByCoordinatesAndDate)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                if (!ModelState.IsValid)
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelValidationFailed(requestPath, query.ToString()), requestPath);
                    return BadRequest(ModelState);
                }

                var targetPoint = new NetTopologySuite.Geometries.Point(query.Longitude, query.Latitude) { SRID = 4326 };

                var closestStation = await DbContext.IBMMeteoStations
                    .OrderBy(s => s.Point.Distance(targetPoint))
                    .FirstOrDefaultAsync();

                if (closestStation is null)
                {
                    return NotFound();
                }

                var inputDate = DateOnly.FromDateTime(query.InputDate);

                var closestFutureReport = await DbContext.IbmDayWeatherForecasts
                    .Where(x => x.WeatherStation == closestStation.Id && x.Date >= inputDate)
                    .OrderBy(x => x.Date)
                    .FirstOrDefaultAsync();

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
        public virtual async Task<ActionResult<IbmDayWeatherForecast>> GetInterpolatedByCoordinatesAndDate([FromBody] CoordinatesAndDateViewModel query)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetByCoordinatesAndDate)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                if (!ModelState.IsValid)
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelValidationFailed(requestPath, query.ToString()), requestPath);
                    return BadRequest(ModelState);
                }

                var targetPoint = new NetTopologySuite.Geometries.Point(query.Longitude, query.Latitude) { SRID = 4326 };

                var closestStation = await DbContext.IBMMeteoStations
                    .OrderBy(s => s.Point.Distance(targetPoint))
                    .FirstOrDefaultAsync();

                if (closestStation is null)
                {
                    return NotFound();
                }

                var nearestStations = await DbContext.IBMMeteoStations.Where(s =>
                        (s.Point.X == closestStation.Point.X && s.Point.Y == closestStation.Point.Y + _step) ||
                        (s.Point.X == closestStation.Point.X && s.Point.Y == closestStation.Point.Y - _step) ||
                        (s.Point.X == closestStation.Point.X + _step && s.Point.Y == closestStation.Point.Y) ||
                        (s.Point.X == closestStation.Point.X - _step && s.Point.Y == closestStation.Point.Y))
                    .ToListAsync();

                var inputDate = DateOnly.FromDateTime(query.InputDate);

                var forecasts = DbContext.IbmDayWeatherForecasts
                    .Where(f => f.WeatherStation != null && nearestStations.Select(s => s.Id).Contains(f.WeatherStation.Value)
                                && f.Date >= inputDate)
                    .OrderBy(f => f.Date)
                    .GroupBy(f => f.Date)
                    .ToDictionary(x => x.Key, x => x.ToList());

                while (!forecasts.ContainsKey(inputDate) && inputDate < DateOnly.FromDateTime(query.InputDate).AddMonths(1))
                {
                    inputDate = inputDate.AddDays(1);
                }

                var closestFutureReports = forecasts[inputDate];

                if (closestFutureReports is null)
                {
                    return NotFound();
                }

                return new IbmDayWeatherForecast()
                {
                    Id = Guid.Empty,
                    Date = inputDate,
                    TemperatureMin = closestFutureReports.Average(r => r.TemperatureMin),
                    TemperatureMax = closestFutureReports.Average(r => r.TemperatureMax),
                    TemperatureMinTotal = closestFutureReports.Average(r => r.TemperatureMinTotal),
                    TemperatureMaxTotal = closestFutureReports.Average(r => r.TemperatureMaxTotal),
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
