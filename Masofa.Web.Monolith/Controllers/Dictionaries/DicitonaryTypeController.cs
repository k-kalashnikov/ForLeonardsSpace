using Masofa.Common.Resources;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Dictionaries;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Masofa.BusinessLogic;

namespace Masofa.Web.Monolith.Controllers.Dictionaries
{
    /// <summary>
    /// Предоставляет API-методы для управления типами справочников
    /// </summary>
    [Route("dictionaries/[controller]")]
    [ApiExplorerSettings(GroupName = "Dictionaries")]
    public class DicitonaryTypeController : BaseDictionaryController<DicitonaryType, MasofaDictionariesDbContext>
    {
        /// <summary>
        /// Инициализирует новый экземпляр <see cref="DicitonaryTypeController"/>.
        /// </summary>
        /// <param name="fileStorageProvider">Провайдер для работы с хранилищем файлов</param>
        /// <param name="dbContext">Контекст базы данных</param>
        /// <param name="logger">Логгер</param>
        /// <param name="configuration">Конфигурация приложения</param>
        /// <param name="mediator">MediatR для обработки команд и запросов</param>
        /// <param name="businessLogicLogger">Логгер бизнес-логики</param>
        /// <param name="httpContextAccessor">Доступ к HTTP контексту</param>
        public DicitonaryTypeController(
            IFileStorageProvider fileStorageProvider, 
            MasofaDictionariesDbContext dbContext, 
            ILogger<DicitonaryTypeController> logger, 
            IConfiguration configuration, 
            IMediator mediator,
            IBusinessLogicLogger businessLogicLogger,
            IHttpContextAccessor httpContextAccessor) : base(
                fileStorageProvider, 
                dbContext, 
                logger, 
                configuration, 
                mediator,
                businessLogicLogger,
                httpContextAccessor)
        {
        }

        /// <summary>
        /// Получает сводную информацию о всех справочниках
        /// </summary>
        /// <returns>Сводная информация о справочниках</returns>
        [HttpGet("summary")]
        public async Task<ActionResult<DictionarySummaryResponse>> GetSummary()
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetSummary)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                
                var response = await Mediator.Send(new DictionarySummaryRequest());
                
                await BusinessLogicLogger.LogInformationAsync($"Successfully retrieved dictionary summary in {requestPath}", requestPath);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var msg = $"Something wrong in {GetType().FullName}=>{nameof(GetSummary)}";
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
