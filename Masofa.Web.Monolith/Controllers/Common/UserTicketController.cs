using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.BusinessLogic.Common.UserTickets;
using Masofa.BusinessLogic.CropMonitoring.Bids;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using Masofa.Common.Models.Notifications;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto.Prng;

namespace Masofa.Web.Monolith.Controllers.Common
{
    /// <summary>
    /// Provides endpoints for managing user tickets and their associated messages.
    /// </summary>
    /// <remarks>This controller includes functionality for creating user tickets, adding messages to tickets,
    /// and retrieving ticket messages with support for filtering and pagination. It is part of the  "Common" API group
    /// and requires authorization for most operations.</remarks>
    [Route("common/[controller]")]
    [ApiExplorerSettings(GroupName = "Common")]
    public class UserTicketController : BaseCrudController<UserTicket, MasofaCommonDbContext>
    {
        private MasofaIdentityDbContext MasofaIdentityDbContext { get; set; }
        private MasofaCommonDbContext MasofaCommonDbContext { get; set; }
        public UserTicketController(IFileStorageProvider fileStorageProvider, MasofaCommonDbContext dbContext, MasofaIdentityDbContext masofaIdentityDbContext, ILogger<UserTicketController> logger, IConfiguration configuration, IMediator mediator, IBusinessLogicLogger businessLogicLogger, IHttpContextAccessor httpContextAccessor) : base(fileStorageProvider, dbContext, logger, configuration, mediator, businessLogicLogger, httpContextAccessor)
        {
            MasofaIdentityDbContext = masofaIdentityDbContext;
            MasofaCommonDbContext = dbContext;
        }

        /// <summary>
        /// Создает новую заявку с автоматическим созданием начального сообщения
        /// </summary>
        /// <param name="requestBody">Запрос на создание заявки с дополнительными полями</param>
        /// <returns>Уникальный идентификатор созданной заявки</returns>
        /// <response code="200">Заявка успешно создана</response>
        /// <response code="400">Некорректные данные модели</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        [Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public override async Task<ActionResult<Guid>> Create([FromBody] UserTicket userTicket)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Create)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                
                if (!ModelState.IsValid)
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelValidationFailed(requestPath, userTicket.ToString()), requestPath);
                    return BadRequest(ModelState);
                }

                var createRequest = new UserTicketCreateCommand()
                {
                    Model = userTicket,
                    Author = User?.Identity?.Name ?? string.Empty,
                    InitialMessage = userTicket.ExceptionJson // Use the exceptionJson as initial message
                };

                var result = await Mediator.Send(createRequest);
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, result.ToString()), requestPath);
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
        /// Создает новое сообщение в системе тикетов
        /// </summary>
        /// <param name="cmd">Команда создания сообщения</param>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns>Идентификатор созданного сообщения</returns>
        /// <response code="200">Сообщение успешно создано</response>
        /// <response code="400">Некорректные данные команды</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<Guid>> CreateMessage(
        [FromBody] CreateMessageCommand cmd,
        CancellationToken ct)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetByQuery)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                var id = await Mediator.Send(cmd, ct);
                return Ok(id);
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                throw ex;
                
            }
        }

        /// <summary>
        /// Загружает вложение для тикета
        /// </summary>
        /// <param name="command">Команда загрузки вложения</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Информация о загруженном файле</returns>
        [HttpPost]
        [Route("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [RequestSizeLimit(50 * 1024 * 1024)]
        public async Task<ActionResult<UploadUserTicketAttachmentResult>> UploadAttachment(
            [FromForm] UploadUserTicketAttachmentCommand command,
            CancellationToken ct)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(UploadAttachment)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                var result = await Mediator.Send(command, ct);
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, result.FileName), requestPath);
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
        /// Получает список сообщений тикетов по заданному запросу с фильтрацией и пагинацией
        /// </summary>
        /// <param name="query">Объект запроса с параметрами фильтрации, сортировки и пагинации</param>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns>Список сообщений тикетов</returns>
        /// <response code="200">Список сообщений успешно получен</response>
        /// <response code="400">Некорректные параметры запроса</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<List<UserTicketMessage>>> GetMessageByQuery([FromBody] MessageGetQuery<UserTicketMessage> query, CancellationToken ct)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetMessageByQuery)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                var bidViewModelList = await Mediator.Send(new GetMessagesByQueryRequest
                {
                    Query = query

                },
                ct);

                return bidViewModelList;
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //[HttpGet]
        //[Route("[action]")]
        //public async Task<ActionResult<object>> GetMessageByQuery(
        //    [FromQuery] Guid? userTicketId,
        //    [FromQuery] bool? onlyWithEmailId,
        //    [FromQuery] string? search,
        //    [FromQuery] DateTime? from,
        //    [FromQuery] DateTime? to,
        //    [FromQuery] int page = 1,
        //    [FromQuery] int pageSize = 50,
        //    [FromQuery] bool desc = true,
        //    CancellationToken ct = default)
        //{
        //    var (items, total) = await Mediator.Send(
        //        new GetUserTicketMessagesQuery(
        //            UserTicketId: userTicketId,
        //            OnlyWithEmailId: onlyWithEmailId,
        //            Search: search,
        //            From: from,
        //            To: to,
        //            Page: page,
        //            PageSize: pageSize,
        //            Desc: desc),
        //        ct);

        //    return Ok(new { total, page, pageSize, items });
        //}
    }
}
