using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.BusinessLogic.Era;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Era;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using Masofa.Web.Monolith.ViewModels.WeatherReport;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using SkiaSharp;

namespace Masofa.Web.Monolith.Controllers.WeatherReport
{
    [Route("weatherReports/[controller]")]
    [ApiExplorerSettings(GroupName = "WeatherReports")]
    public class Era5HourWeatherForecastController : BaseEra5WeatherReportController<Era5HourWeatherForecast, MasofaEraDbContext>
    {
        public Era5HourWeatherForecastController(IFileStorageProvider fileStorageProvider, 
            MasofaEraDbContext dbContext, 
            ILogger<Era5HourWeatherForecastController> logger, 
            IConfiguration configuration, 
            IMediator mediator, 
            IBusinessLogicLogger businessLogicLogger, 
            IHttpContextAccessor httpContextAccessor) 
            : base(fileStorageProvider, dbContext, logger, configuration, mediator, businessLogicLogger, httpContextAccessor)
        {

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
        public virtual async Task<ActionResult<Era5HourWeatherForecast>> GetByCoordinatesAndDate([FromBody] CoordinatesAndDateViewModel query)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetByCoordinatesAndDate)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                if (!ModelState.IsValid)
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelValidationFailed(requestPath,Newtonsoft.Json.JsonConvert.SerializeObject(query)), requestPath);
                    return BadRequest(ModelState);
                }

                var closestFutureReport = await Mediator.Send(new GetByCoordinatesAndDateRequest()
                {
                    InputDate = query.InputDate,
                    Latitude = query.Latitude,
                    Longitude = query.Longitude
                });

                if (closestFutureReport == null)
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
    }
}
