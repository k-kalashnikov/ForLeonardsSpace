using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Ugm;
using Masofa.DataAccess;
using Masofa.Web.Monolith.ViewModels.WeatherReport;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Masofa.Web.Monolith.Controllers.Weather
{
    /// <summary>
    /// Предоставляет API-методы для управления погодными данными УГМ
    /// </summary>
    [Route("weather/[controller]")]
    [ApiExplorerSettings(GroupName = "Weather")]
    public class UgmWeatherDataController : BaseController
    {
        private MasofaUgmDbContext UgmDbContext { get; }
        private IBusinessLogicLogger BusinessLogicLogger { get; }

        private const double _maxDistanceMeters = 10000;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="UgmWeatherDataController"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных</param>
        /// <param name="logger">Логгер</param>
        /// <param name="configuration">Конфигурация приложения</param>
        /// <param name="mediator">Медиатор</param>
        /// <param name="businessLogicLogger">Бизнес логгер</param>
        public UgmWeatherDataController(
            MasofaUgmDbContext dbContext,
            ILogger<UgmWeatherDataController> logger,
            IConfiguration configuration,
            IMediator mediator,
            IBusinessLogicLogger businessLogicLogger) : base(
                logger,
                configuration,
                mediator)
        {
            UgmDbContext = dbContext;
            BusinessLogicLogger = businessLogicLogger;
        }

        /// <summary>
        /// Получает погодные данные УГМ на указанную дату по ближайшей к координате станции
        /// </summary>
        /// <param name="query">Объект запроса с координатами и датой</param>
        /// <returns>Список найденных сущностей</returns>
        /// <response code="200">Успешно получен список сущностей</response>
        /// <response code="400">Некорректные параметры запроса</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        public virtual async Task<ActionResult<UgmWeatherData>> GetByCoordinatesAndDate([FromBody] CoordinatesAndDateViewModel query)
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

                var sql = @"
                    SELECT *
                    FROM ""UgmWeatherStations""
                    WHERE ""Longitude"" IS NOT NULL 
                    AND ""Latitude"" IS NOT NULL
                    AND ST_DWithin(
                        ST_SetSRID(ST_MakePoint(""Longitude"", ""Latitude""), 4326)::geography,
                        ST_SetSRID(ST_MakePoint(@lon, @lat), 4326)::geography,
                        @maxDistanceMeters
                    )";

                var nearbyStations = await UgmDbContext.UgmWeatherStations
                    .FromSqlRaw(sql,
                        new NpgsqlParameter("@lon", query.Longitude),
                        new NpgsqlParameter("@lat", query.Latitude),
                        new NpgsqlParameter("@maxDistanceMeters", _maxDistanceMeters))
                    .Select(s => s.UgmRegionId)
                    .ToListAsync();

                if (nearbyStations.Count == 0)
                {
                    var errorMsg = LogMessageResource.NoWeatherStationsNearby();
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return NotFound(errorMsg);
                }

                var result = await UgmDbContext.UgmWeatherData
                    .Where(d => nearbyStations.Contains(d.RegionId) && d.Date >= DateOnly.FromDateTime(query.InputDate) && d.DayPart == DayPart.Day)
                    .OrderBy(d => d.Date)
                    .FirstOrDefaultAsync();

                if (result == null)
                {
                    var errorMsg = $"There is no weather data";
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return NotFound(errorMsg);
                }

                return result;
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath,ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
