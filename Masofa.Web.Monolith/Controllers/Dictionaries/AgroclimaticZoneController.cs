using Masofa.BusinessLogic.Dictionaries;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Dictionaries;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Masofa.Web.Monolith.Controllers.Dictionaries
{
    /// <summary>
    /// Предоставляет API-методы для управления агроклиматическими зонами
    /// </summary>
    [Route("dictionaries/[controller]")]
    [ApiExplorerSettings(GroupName = "Dictionaries")]
    public class AgroclimaticZoneController : BaseDictionaryController<AgroclimaticZone, MasofaDictionariesDbContext>
    {
        /// <summary>
        /// Инициализирует новый экземпляр <see cref="AgroclimaticZoneController"/>.
        /// </summary>
        /// <param name="fileStorageProvider">Провайдер для работы с хранилищем файлов</param>
        /// <param name="dbContext">Контекст базы данных</param>
        /// <param name="logger">Логгер</param>
        /// <param name="configuration">Конфигурация приложения</param>
        /// <param name="mediator">Mediator</param>
        /// <param name="businessLogicLogger">Логгер бизнес-логики</param>
        /// <param name="httpContextAccessor">HttpContextAccessor</param>
        public AgroclimaticZoneController(
            IFileStorageProvider fileStorageProvider,
            MasofaDictionariesDbContext dbContext,
            ILogger<AgroclimaticZoneController> logger,
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
        /// Получает список агроклиматических зон с расчетными полями по заданному запросу
        /// </summary>
        /// <param name="query">Объект запроса с параметрами фильтрации, сортировки и пагинации</param>
        /// <returns>Список агроклиматических зон с расчетными полями</returns>
        /// <response code="200">Успешно получен список агроклиматических зон</response>
        /// <response code="400">Некорректные параметры запроса</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost]
        [Route("CustomGetByQuery")]
        public async Task<ActionResult<List<AgroclimaticZoneDto>>> CustomGetByQuery([FromBody] AgroclimaticZoneQueryRequest query)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(CustomGetByQuery)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync($"Request started: {requestPath}", requestPath);

                if (!ModelState.IsValid)
                {
                    await BusinessLogicLogger.LogErrorAsync($"Model validation failed: {requestPath}", requestPath);
                    return BadRequest(ModelState);
                }

                var request = new AgroclimaticZoneGetByQueryRequest
                {
                    Query = query,
                    Year = query.Year,
                    Month = query.Month,
                    Day = query.Day
                };

                var result = await Mediator.Send(request);

                await BusinessLogicLogger.LogInformationAsync($"Request finished: {requestPath}", requestPath);
                return result;
            }
            catch (Exception ex)
            {
                var msg = $"Error in {requestPath}: {ex.Message}";
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
