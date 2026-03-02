using Masofa.BusinessLogic.Reports;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Reports;
using Masofa.Common.Resources;
using Masofa.Common.Services.FileStorage;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Masofa.Web.Monolith.Controllers.Reports
{
    /// <summary>
    /// Базовый контроллер для всех отчетов
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TReportModel"></typeparam>
    /// <typeparam name="TDbContext"></typeparam>
    [ApiController]
    public class BaseReportCrudController<TModel, TReportModel, TDbContext> : BaseCrudController<TModel, TDbContext>
        where TModel : BaseReportEntity<TReportModel>, new()
        where TReportModel : class, new()
        where TDbContext : DbContext
    {
        /// <summary>
        /// Конструктор базового контроллера для всех отчетов
        /// </summary>
        /// <param name="fileStorageProvider"></param>
        /// <param name="dbContext"></param>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        /// <param name="mediator"></param>
        /// <param name="businessLogicLogger"></param>
        /// <param name="httpContextAccessor"></param>
        public BaseReportCrudController(
            IFileStorageProvider fileStorageProvider,
            TDbContext dbContext,
            ILogger logger,
            IConfiguration configuration,
            IMediator mediator,
            IBusinessLogicLogger businessLogicLogger,
            IHttpContextAccessor httpContextAccessor) : base(fileStorageProvider, dbContext, logger, configuration, mediator, businessLogicLogger, httpContextAccessor)
        {
        }

        /// <summary>
        /// Получает html представление отчета
        /// </summary>
        /// <param name="query">Запрос на печать отчета</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Найденная сущность</returns>
        /// <response code="200">Сущность успешно найдена</response>
        /// <response code="404">Сущность с указанным ID не найдена</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        public async Task<IActionResult> Print([FromBody] BaseReportPrintQuery<TModel> query, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Print)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                if (!ModelState.IsValid)
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelValidationFailed(requestPath, Newtonsoft.Json.JsonConvert.SerializeObject(query)), requestPath);
                    return BadRequest(ModelState);
                }

                var printRequest = new BaseReportPrintCommand<TModel, TReportModel, TDbContext>()
                {
                    Query = query,
                };

                var result = await Mediator.Send(printRequest, cancellationToken);

                var fileName = $"{typeof(TModel).Name}_{DateTime.Now:yyyy-MM-dd_HH-mm}.html";

                return File(result, "text/html", fileName);
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
