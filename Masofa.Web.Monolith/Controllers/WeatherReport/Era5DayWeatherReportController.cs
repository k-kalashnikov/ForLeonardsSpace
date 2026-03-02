using Masofa.BusinessLogic.Extentions;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.BusinessLogic.WeatherReport;
using Masofa.Common.Models.Era;
using Masofa.Common.Resources;
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
    public class Era5DayWeatherReportController : BaseEra5WeatherReportController<Era5DayWeatherReport, MasofaEraDbContext>
    {
        private readonly double _step;
        public Era5DayWeatherReportController(IFileStorageProvider fileStorageProvider,
            MasofaEraDbContext dbContext,
            ILogger<Era5DayWeatherReportController> logger,
            IConfiguration configuration,
            IMediator mediator,
            IBusinessLogicLogger businessLogicLogger,
            IHttpContextAccessor httpContextAccessor)
            : base(fileStorageProvider, dbContext, logger, configuration, mediator, businessLogicLogger, httpContextAccessor)
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
        public virtual async Task<ActionResult<Era5DayWeatherReportByCoordinates>> GetByCoordinatesAndDate([FromBody] CoordinatesAndDateViewModel query)
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

                var closestFutureReport = await DbContext.Era5DayWeatherReports
                    .Where(x => x.WeatherStation == closestStation.Id && x.Date >= inputDate)
                    .OrderBy(x => x.Date)
                    .FirstOrDefaultAsync();

                if (closestFutureReport is null)
                {
                    return NotFound();
                }

                var newGetEraWeatherSumsCommand = new GetEraWeatherSumsCommand()
                {
                    Date = closestFutureReport.Date,
                    StationIds = [closestStation.Id]
                };

                var sums = await Mediator.Send(newGetEraWeatherSumsCommand);

                var result = new Era5DayWeatherReportByCoordinates();
                result.CopyFrom(closestFutureReport);

                result.SumOfActiveTemperaturesBase5 = sums.SumOfActiveTemperaturesBase5;
                result.SumOfActiveTemperaturesBase7 = sums.SumOfActiveTemperaturesBase7;
                result.SumOfActiveTemperaturesBase10 = sums.SumOfActiveTemperaturesBase10;
                result.SumOfActiveTemperaturesBase12 = sums.SumOfActiveTemperaturesBase12;
                result.SumOfActiveTemperaturesBase15 = sums.SumOfActiveTemperaturesBase15;
                result.SumOfSolarRadiation = sums.SumOfSolarRadiation;
                result.SumOfFallout = sums.SumOfFallout;

                return result;
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
        public virtual async Task<ActionResult<Era5DayWeatherReport>> GetInterpolatedByCoordinatesAndDate([FromBody] CoordinatesAndDateViewModel query)
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

                var report = new Era5DayWeatherReport()
                {
                    Id = Guid.Empty,
                    Date = inputDate,
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

                var newGetEraWeatherSumsCommand = new GetEraWeatherSumsCommand()
                {
                    Date = inputDate,
                    StationIds = [.. nearestStations.Select(s => s.Id)]
                };

                var sums = await Mediator.Send(newGetEraWeatherSumsCommand);

                var result = new Era5DayWeatherReportByCoordinates();
                result.CopyFrom(report);

                result.SumOfActiveTemperaturesBase5 = sums.SumOfActiveTemperaturesBase5;
                result.SumOfActiveTemperaturesBase7 = sums.SumOfActiveTemperaturesBase7;
                result.SumOfActiveTemperaturesBase10 = sums.SumOfActiveTemperaturesBase10;
                result.SumOfActiveTemperaturesBase12 = sums.SumOfActiveTemperaturesBase12;
                result.SumOfActiveTemperaturesBase15 = sums.SumOfActiveTemperaturesBase15;
                result.SumOfSolarRadiation = sums.SumOfSolarRadiation;
                result.SumOfFallout = sums.SumOfFallout;

                return result;
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        public class Era5DayWeatherReportByCoordinates : Era5DayWeatherReport, IWeatherReportWithSums
        {
            public double? SumOfActiveTemperaturesBase5 { get; set; }
            public double? SumOfActiveTemperaturesBase7 { get; set; }
            public double? SumOfActiveTemperaturesBase10 { get; set; }
            public double? SumOfActiveTemperaturesBase12 { get; set; }
            public double? SumOfActiveTemperaturesBase15 { get; set; }
            public double? SumOfSolarRadiation { get; set; }
            public double? SumOfFallout { get; set; }
        }
    }
}
