using Masofa.Common.Resources;
using Masofa.BusinessLogic.Satellite;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Satellite;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Masofa.Web.Monolith.Controllers.Satellite
{
    /// <summary>
    /// Контроллер для работы со статусом задач обработки спутниковых снимков
    /// </summary>
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, SystemAdmin, ModuleAdmin, Operator")]
    [Route("satellite/[controller]")]
    [ApiExplorerSettings(GroupName = "Satellite")]
    public class SatelliteTaskStatusController : BaseController
    {
        private readonly IBusinessLogicLogger _businessLogicLogger;

        public SatelliteTaskStatusController(
            ILogger<SatelliteTaskStatusController> logger,
            IConfiguration configuration,
            IMediator mediator,
            IBusinessLogicLogger businessLogicLogger) : base(logger, configuration, mediator)
        {
            _businessLogicLogger = businessLogicLogger;
        }

        /// <summary>
        /// Получить статус задач обработки спутниковых снимков
        /// </summary>
        /// <param name="request">Параметры запроса</param>
        /// <returns>Список задач с их статусами</returns>
        /// <response code="200">Задачи успешно получены</response>
        /// <response code="400">Некорректные параметры запроса</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<GetSatelliteTaskStatusResponse>> GetTaskStatus([FromBody] GetSatelliteTaskStatusRequest request)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetTaskStatus)}";
            try
            {
                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                // Нормализуем даты: если пришла дата без времени (00:00:00), оставляем как есть в UTC
                // Не преобразуем, чтобы не менять дату из-за часового пояса
                if (request.StartDate.HasValue)
                {
                    var startDate = request.StartDate.Value;
                    // Если время 00:00:00, значит это дата без времени - оставляем как есть
                    if (startDate.Hour == 0 && startDate.Minute == 0 && startDate.Second == 0 && startDate.Millisecond == 0)
                    {
                        request.StartDate = DateTime.SpecifyKind(new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0), DateTimeKind.Utc);
                    }
                    else if (startDate.Kind == DateTimeKind.Unspecified)
                    {
                        request.StartDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
                    }
                    else
                    {
                        request.StartDate = startDate.ToUniversalTime();
                    }
                }

                if (request.EndDate.HasValue)
                {
                    var endDate = request.EndDate.Value;
                    // Если время 00:00:00, значит это дата без времени - оставляем как есть
                    if (endDate.Hour == 0 && endDate.Minute == 0 && endDate.Second == 0 && endDate.Millisecond == 0)
                    {
                        request.EndDate = DateTime.SpecifyKind(new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59), DateTimeKind.Utc);
                    }
                    else if (endDate.Kind == DateTimeKind.Unspecified)
                    {
                        request.EndDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);
                    }
                    else
                    {
                        request.EndDate = endDate.ToUniversalTime();
                    }
                }

                var response = await Mediator.Send(request);

                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, response.Total.ToString()), requestPath);

                return Ok(response);
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await _businessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Получить статус задач обработки спутниковых снимков (GET версия для простых запросов)
        /// </summary>
        /// <param name="take">Количество записей</param>
        /// <param name="offset">Смещение</param>
        /// <param name="status">Фильтр по статусу</param>
        /// <param name="satelliteType">Фильтр по типу спутника</param>
        /// <param name="startDate">Начало периода</param>
        /// <param name="endDate">Конец периода</param>
        /// <param name="productId">Фильтр по ID продукта</param>
        /// <returns>Список задач с их статусами</returns>
        /// <response code="200">Задачи успешно получены</response>
        /// <response code="400">Некорректные параметры запроса</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<GetSatelliteTaskStatusResponse>> GetTaskStatusSimple(
            [FromQuery] int? take = 20,
            [FromQuery] int offset = 0,
            [FromQuery] int? status = null,
            [FromQuery] int? satelliteType = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? productId = null)
        {
            // Нормализуем даты: если пришла дата без времени (00:00:00), оставляем как есть в UTC
            DateTime? normalizedStartDate = null;
            if (startDate.HasValue)
            {
                var sd = startDate.Value;
                // Если время 00:00:00, значит это дата без времени - оставляем как есть
                if (sd.Hour == 0 && sd.Minute == 0 && sd.Second == 0 && sd.Millisecond == 0)
                {
                    normalizedStartDate = DateTime.SpecifyKind(new DateTime(sd.Year, sd.Month, sd.Day, 0, 0, 0), DateTimeKind.Utc);
                }
                else if (sd.Kind == DateTimeKind.Unspecified)
                {
                    normalizedStartDate = DateTime.SpecifyKind(sd, DateTimeKind.Utc);
                }
                else
                {
                    normalizedStartDate = sd.ToUniversalTime();
                }
            }

            DateTime? normalizedEndDate = null;
            if (endDate.HasValue)
            {
                var ed = endDate.Value;
                // Если время 00:00:00, значит это дата без времени - оставляем как есть
                if (ed.Hour == 0 && ed.Minute == 0 && ed.Second == 0 && ed.Millisecond == 0)
                {
                    normalizedEndDate = DateTime.SpecifyKind(new DateTime(ed.Year, ed.Month, ed.Day, 23, 59, 59), DateTimeKind.Utc);
                }
                else if (ed.Kind == DateTimeKind.Unspecified)
                {
                    normalizedEndDate = DateTime.SpecifyKind(ed, DateTimeKind.Utc);
                }
                else
                {
                    normalizedEndDate = ed.ToUniversalTime();
                }
            }

            var request = new GetSatelliteTaskStatusRequest
            {
                Take = take,
                Offset = offset,
                Status = status.HasValue ? (ProductQueueStatusType)status.Value : null,
                SatelliteType = satelliteType.HasValue ? (ProductSourceType)satelliteType.Value : null,
                StartDate = normalizedStartDate,
                EndDate = normalizedEndDate,
                ProductId = productId
            };

            return await GetTaskStatus(request);
        }
    }
}
