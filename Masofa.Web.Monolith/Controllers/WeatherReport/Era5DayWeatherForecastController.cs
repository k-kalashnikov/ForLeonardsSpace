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
using static Masofa.Web.Monolith.Controllers.WeatherReport.Era5DayWeatherReportController;

namespace Masofa.Web.Monolith.Controllers.WeatherReport
{
    [Route("weatherReports/[controller]")]
    [ApiExplorerSettings(GroupName = "WeatherReports")]
    public class Era5DayWeatherForecastController : BaseEra5WeatherReportController<Era5DayWeatherForecast, MasofaEraDbContext>
    {
        private readonly double _step;

        public Era5DayWeatherForecastController(IFileStorageProvider fileStorageProvider,
            ILogger<Era5DayWeatherForecastController> logger,
            IConfiguration configuration,
            IMediator mediator,
            IBusinessLogicLogger businessLogicLogger,
            IHttpContextAccessor httpContextAccessor,
            MasofaEraDbContext eraDbContext) : base(fileStorageProvider, eraDbContext, logger, configuration, mediator, businessLogicLogger, httpContextAccessor)
        {
            _step = configuration.GetValue<double>("CountryBoundaries:Step");
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
        public virtual async Task<ActionResult<Era5DayWeatherForecast>> GetByCoordinatesAndDate([FromBody] CoordinatesAndDateViewModel query)
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

                var closestFutureReport = await DbContext.Era5DayWeatherForecasts
                    .Where(x => x.WeatherStation == closestStation.Id && x.Date == inputDate)
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

                var result = new Era5DayWeatherForecastByCoordinates();
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
        public virtual async Task<ActionResult<Era5DayWeatherForecast>> GetInterpolatedByCoordinatesAndDate([FromBody] CoordinatesAndDateViewModel query)
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

                var forecasts = DbContext.Era5DayWeatherForecasts
                    .Where(f => f.WeatherStation != null && nearestStations.Select(s => s.Id).Contains(f.WeatherStation.Value)
                                && f.Date == inputDate)
                    .OrderBy(f => f.Date)
                    .GroupBy(f => f.Date)
                    .ToDictionary(x => x.Key, x => x.ToList());

                if (forecasts.Count == 0)
                {
                    return NotFound();
                }

                if (!forecasts.TryGetValue(inputDate, out List<Era5DayWeatherForecast>? closestFutureReports))
                {
                    return NotFound();
                }

                if (closestFutureReports is null)
                {
                    return NotFound();
                }

                var report = new Era5DayWeatherForecast()
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
                    StationIds = [closestStation.Id]
                };

                var sums = await Mediator.Send(newGetEraWeatherSumsCommand);

                var result = new Era5DayWeatherForecastByCoordinates();
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

        /// <summary>
        /// Получает прогноз на 8 дней (текущий + 7 следующих) и информацию о регионе по координатам
        /// </summary>
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        public virtual async Task<ActionResult<Era5DayWeatherForecastWithRegionViewModel>> GetForecastFor8DaysWithRegion([FromBody] CoordinatesAndDateViewModel query)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetForecastFor8DaysWithRegion)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                if (!ModelState.IsValid)
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelValidationFailed(requestPath, Newtonsoft.Json.JsonConvert.SerializeObject(query)), requestPath);
                    return BadRequest(ModelState);
                }

                var region = await Mediator.Send(new Masofa.BusinessLogic.Dictionaries.GetRegionByPointRequest()
                {
                    Latitude = query.Latitude,
                    Longitude = query.Longitude
                });

                var targetPoint = new Point(query.Longitude, query.Latitude) { SRID = 4326 };

                var closestStation = await DbContext.EraWeatherStations
                    .OrderBy(s => s.Point.Distance(targetPoint))
                    .FirstOrDefaultAsync();

                if (closestStation is null)
                {
                    return NotFound("Weather station not found for these coordinates.");
                }

                var startDate = DateOnly.FromDateTime(query.InputDate);
                var endDate = startDate.AddDays(7);

                var baseSumsDate = startDate.AddDays(-1);
                var baseSumsCommand = new GetEraWeatherSumsCommand()
                {
                    Date = baseSumsDate,
                    StationIds = [closestStation.Id]
                };

                var baseSums = await Mediator.Send(baseSumsCommand);

                var forecasts = await DbContext.Era5DayWeatherForecasts
                    .Where(x => x.WeatherStation == closestStation.Id && x.Date >= startDate && x.Date <= endDate)
                    .OrderBy(x => x.Date)
                    .ToListAsync();

                var startDateTime = DateTime.SpecifyKind(startDate.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
                var endDateTime = DateTime.SpecifyKind(endDate.AddDays(1).ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);

                var weatherData = await DbContext.EraWeatherData
                    .Where(d => d.EraWeatherStationId == closestStation.Id
                                && d.OriginalDateTimeUtc >= startDateTime
                                && d.OriginalDateTimeUtc < endDateTime)
                    .Select(d => new { d.OriginalDateTimeUtc, d.CloudCover, d.Precipitation })
                    .ToListAsync();

                var weatherDataByDate = weatherData
                    .GroupBy(d => DateOnly.FromDateTime(d.OriginalDateTimeUtc!.Value))
                    .ToDictionary(g => g.Key, g => new
                    {
                        AvgCloudCover = g.Average(d => d.CloudCover),
                        TotalPrecipitation = g.Sum(d => d.Precipitation ?? 0)
                    });

                var forecastsWithSums = new List<Era5DayWeatherForecastByCoordinates>(forecasts.Count);

                double currentSum5 = baseSums.SumOfActiveTemperaturesBase5 ?? 0;
                double currentSum7 = baseSums.SumOfActiveTemperaturesBase7 ?? 0;
                double currentSum10 = baseSums.SumOfActiveTemperaturesBase10 ?? 0;
                double currentSum12 = baseSums.SumOfActiveTemperaturesBase12 ?? 0;
                double currentSum15 = baseSums.SumOfActiveTemperaturesBase15 ?? 0;
                double currentSolar = baseSums.SumOfSolarRadiation ?? 0;
                double currentFallout = baseSums.SumOfFallout ?? 0;

                foreach (var forecast in forecasts)
                {
                    double tAvg = (forecast.TemperatureMin + forecast.TemperatureMax) / 2.0;

                    if (tAvg > 5) currentSum5 += (tAvg - 5);
                    if (tAvg > 7) currentSum7 += (tAvg - 7);
                    if (tAvg > 10) currentSum10 += (tAvg - 10);
                    if (tAvg > 12) currentSum12 += (tAvg - 12);
                    if (tAvg > 15) currentSum15 += (tAvg - 15);

                    currentSolar += forecast.SolarRadiationInfluence;
                    currentFallout += forecast.Fallout;

                    var forecastResult = new Era5DayWeatherForecastByCoordinates();
                    forecastResult.CopyFrom(forecast);

                    forecastResult.SumOfActiveTemperaturesBase5 = currentSum5;
                    forecastResult.SumOfActiveTemperaturesBase7 = currentSum7;
                    forecastResult.SumOfActiveTemperaturesBase10 = currentSum10;
                    forecastResult.SumOfActiveTemperaturesBase12 = currentSum12;
                    forecastResult.SumOfActiveTemperaturesBase15 = currentSum15;
                    forecastResult.SumOfSolarRadiation = currentSolar;
                    forecastResult.SumOfFallout = currentFallout;

                    if (weatherDataByDate.TryGetValue(forecast.Date, out var dayData))
                    {
                        forecastResult.Condition = DetermineWeatherCondition(dayData.AvgCloudCover, dayData.TotalPrecipitation);
                    }
                    else
                    {
                        forecastResult.Condition = WeatherConditionType.Clear;
                    }

                    forecastsWithSums.Add(forecastResult);
                }

                var result = new Era5DayWeatherForecastWithRegionViewModel
                {
                    Region = region,
                    Forecasts = forecastsWithSums
                };

                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, $"Found Region: {region?.Id}, Forecasts: {forecastsWithSums.Count}"), requestPath);

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
        /// Определяет состояние погоды на основе суточной суммы осадков и облачности.
        /// Градация осадков взята из таблицы:
        /// < 1 мм: Ориентируемся на облачность
        /// 1 - 10 мм: Небольшой дождь
        /// 10 - 30 мм: Дождь
        /// > 30 мм: Сильный дождь
        /// </summary>
        private WeatherConditionType DetermineWeatherCondition(double? cloudCoverPercent, double totalPrecipitationMm)
        {
            // 1. Проверка на осадки (приоритет).

            // > 30 мм - Сильный дождь
            if (totalPrecipitationMm > 30)
            {
                return WeatherConditionType.HeavyRain;
            }

            // 10 - 30 мм - Умеренный дождь
            if (totalPrecipitationMm >= 10)
            {
                return WeatherConditionType.Rain;
            }

            // 1 - 10 мм - Небольшой дождь
            if (totalPrecipitationMm >= 1)
            {
                return WeatherConditionType.LightRain;
            }

            // 2. Если осадков меньше 1 мм ("Преимущественно сухо"), определяем по облачности.
            if (!cloudCoverPercent.HasValue) return WeatherConditionType.Clear;

            return cloudCoverPercent.Value switch
            {
                <= 10 => WeatherConditionType.Clear,
                <= 30 => WeatherConditionType.MostlyClear,
                <= 60 => WeatherConditionType.PartlyCloudy,
                <= 90 => WeatherConditionType.Cloudy,
                _ => WeatherConditionType.Overcast
            };
        }

        public class Era5DayWeatherForecastByCoordinates : Era5DayWeatherForecast, IWeatherReportWithSums
        {
            public double? SumOfActiveTemperaturesBase5 { get; set; }
            public double? SumOfActiveTemperaturesBase7 { get; set; }
            public double? SumOfActiveTemperaturesBase10 { get; set; }
            public double? SumOfActiveTemperaturesBase12 { get; set; }
            public double? SumOfActiveTemperaturesBase15 { get; set; }
            public double? SumOfSolarRadiation { get; set; }
            public double? SumOfFallout { get; set; }
            public WeatherConditionType? Condition { get; set; }
        }

        /// <summary>
        /// Общее перечисление состояний погоды (Облачность + Уровни осадков)
        /// </summary>
        public enum WeatherConditionType
        {
            // --- Облачность (если осадков < 1 мм) ---
            Clear = 0,          // Ясно
            MostlyClear = 1,    // Преимущественно ясно
            PartlyCloudy = 2,   // Переменная облачность
            Cloudy = 3,         // Облачно
            Overcast = 4,       // Пасмурно

            // --- Осадки (Приоритет, если осадков >= 1 мм) ---
            LightRain = 5,      // Небольшой дождь (1 - 10 мм)
            Rain = 6,           // Дождь (10 - 30 мм)
            HeavyRain = 7       // Сильный дождь (> 30 мм)
        }

        public class Era5DayWeatherForecastWithRegionViewModel
        {
            public Masofa.Common.Models.Dictionaries.Region? Region { get; set; }
            public List<Era5DayWeatherForecastByCoordinates> Forecasts { get; set; } = [];
        }
    }
}
