using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.SystemCrical;
using Masofa.DataAccess;
using Masofa.Web.Monolith.Controllers.Identity;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Masofa.Web.Monolith.Controllers.Common
{
    /// <summary>
    /// Provides endpoints for managing and retrieving business logic logging data.
    /// </summary>
    /// <remarks>This controller is part of the "Common" API group and is secured with JWT-based
    /// authentication. It allows administrators to query call stacks, retrieve log messages, and obtain logging
    /// statistics.</remarks>
    [Route("common/[controller]")]
    [ApiExplorerSettings(GroupName = "Common")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, SystemAdmin, Internal")]

    public class BusinessLogicLoggerController : BaseController
    {
        private MasofaCommonDbContext CommonDbContext { get; set; }
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }

        public BusinessLogicLoggerController(
            ILogger<AccountController> logger,
            IConfiguration configuration,
            IMediator mediator,
            MasofaCommonDbContext masofaCommonDbContext,
            IBusinessLogicLogger businessLogicLogger) : base(
                logger,
                configuration,
                mediator)
        {
            CommonDbContext = masofaCommonDbContext;
            BusinessLogicLogger = businessLogicLogger;
        }

        /// <summary>
        /// Получает список всех стеков вызовов из системы логирования
        /// </summary>
        /// <returns>Список стеков вызовов</returns>
        /// <response code="200">Список стеков вызовов успешно получен</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<List<BusinessLogic.Logging.Queries.CallStackDto>>> GetByQuery([FromBody] LoggingRequest request)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetByQuery)}";
            try
            {
                // Логируем входящий запрос для отладки
                Logger.LogInformation($"GetByQuery request: Take={request.Take}, Offset={request.Offset}, Filters count={request.Filters?.Count ?? 0}");
                if (request.Filters != null)
                {
                    foreach (var filter in request.Filters)
                    {
                        Logger.LogInformation($"Filter: {filter.Name} = {filter.Value}");
                    }
                }

                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                var result = await Mediator.Send(new BusinessLogic.Logging.Queries.GetCallStacksByQueryQuery
                {
                    Take = request.Take,
                    Offset = request.Offset,
                    Filters = request.Filters
                });
                return Ok(result);
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
        [Route("[action]")]
        public async Task<ActionResult<int>> GetTotalCount([FromBody] LoggingRequest request)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetTotalCount)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                var result = await Mediator.Send(new BusinessLogic.Logging.Queries.GetCallStacksTotalCountQuery
                {
                    Filters = request.Filters
                });
                return Ok(result);
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
        /// Получает список лог-сообщений для указанного стека вызовов
        /// </summary>
        /// <param name="id">Идентификатор стека вызовов</param>
        /// <returns>Список лог-сообщений</returns>
        /// <response code="200">Список лог-сообщений успешно получен</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet]
        [Route("[action]/{id}")]
        public async Task<ActionResult<List<BusinessLogic.Logging.Queries.LogMessageDto>>> GetLogMessagesByCallStack(Guid id)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetLogMessagesByCallStack)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                var result = await Mediator.Send(new BusinessLogic.Logging.Queries.GetLogMessagesByCallStackQuery
                {
                    Id = id
                });
                return Ok(result);
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
        /// Получает список уникальных подсистем (модулей) из логов
        /// </summary>
        /// <returns>Список уникальных подсистем</returns>
        /// <response code="200">Список подсистем успешно получен</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<string>>> GetSubsystems()
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetSubsystems)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                var result = await Mediator.Send(new BusinessLogic.Logging.Queries.GetSubsystemsRequest());
                await BusinessLogicLogger.LogInformationAsync($"Finish request in {requestPath} with result: {result.Count} subsystems", requestPath);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        public sealed class LoggingRequest
        {
            public int Take { get; set; } = 10;
            public int Offset { get; set; } = 0;
            public List<BusinessLogic.Logging.Queries.FilterItem>? Filters { get; set; }
        }
    }
}
