using Masofa.BusinessLogic.Satellite;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.Resources;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using Masofa.Web.Monolith.Controllers.Dictionaries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Masofa.Web.Monolith.Controllers.Common
{
    /// <summary>
    /// Предоставляет API-методы для управления тегами
    /// </summary>
    [Route("common/[controller]")]
    [ApiExplorerSettings(GroupName = "Common")]
    public class TagController : BaseDictionaryController<Tag, MasofaDictionariesDbContext>
    {
        private MasofaCommonDbContext CommonDbContext { get; set; }

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="TagController"/>.
        /// </summary>
        /// <param name="fileStorageProvider">Провайдер для работы с хранилищем файлов</param>
        /// <param name="dbContext">Контекст базы данных</param>
        /// <param name="logger">Логгер</param>
        /// <param name="configuration">Конфигурация приложения</param>
        /// <param name="mediator">Медиатор для CQRS</param>
        /// <param name="businessLogicLogger">Логгер бизнес-логики</param>
        /// <param name="httpContextAccessor">Доступ к HTTP контексту</param>
        /// <param name="commonDbContext">Контекст общей БД</param>
        public TagController(
            IFileStorageProvider fileStorageProvider,
            MasofaDictionariesDbContext dbContext,
            ILogger<TagController> logger,
            IConfiguration configuration,
            IMediator mediator,
            IBusinessLogicLogger businessLogicLogger,
            IHttpContextAccessor httpContextAccessor,
            MasofaCommonDbContext commonDbContext) : base(
                fileStorageProvider,
                dbContext,
                logger,
                configuration,
                mediator,
                businessLogicLogger,
                httpContextAccessor)
        {
            CommonDbContext = commonDbContext;
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
        public override async Task<ActionResult<List<Tag>>> GetByQuery([FromBody] BaseGetQuery<Tag> query)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetByQuery)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                if (!ModelState.IsValid)
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelValidationFailed(requestPath, Newtonsoft.Json.JsonConvert.SerializeObject(query)), requestPath);
                    return BadRequest(ModelState);
                }

                var tags = (await base.GetByQuery(query)).Value;
                if (tags == null)
                {
                    NotFound();
                }

                var relationsCount = await CommonDbContext.TagRelations.Where(r => tags.Select(t => t.Id).Contains(r.TagId) && r.Status == StatusType.Active)
                    .GroupBy(r => r.TagId)
                    .ToDictionaryAsync(x => x.Key, x => x.ToList().Count);

                foreach (var tag in tags)
                {
                    tag.RelationCount = relationsCount.GetValueOrDefault(tag.Id, 0);
                }

                return tags;
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
        /// Получает сущность по уникальному идентификатору
        /// </summary>
        /// <param name="id">Уникальный идентификатор сущности</param>
        /// <returns>Найденная сущность</returns>
        /// <response code="200">Сущность успешно найдена</response>
        /// <response code="404">Сущность с указанным ID не найдена</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("[action]/{id}")]
        public override async Task<ActionResult<Tag>> GetById(Guid id)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetById)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                var result = (await base.GetById(id)).Value;
                if (result == null)
                {
                    var errorMsg = LogMessageResource.EntityNotFound(typeof(Tag), id.ToString());
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return NotFound(errorMsg);
                }

                var relationsCount = (await CommonDbContext.TagRelations.Where(r => r.TagId == result.Id && r.Status == StatusType.Active).ToListAsync()).Count;

                result.RelationCount = relationsCount;

                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, Newtonsoft.Json.JsonConvert.SerializeObject(result)), requestPath);
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
        /// Получает все теги из БД (для фильтра)
        /// </summary>
        /// <param name="searchQuery">Поисковый запрос (опционально)</param>
        /// <returns>Список всех тегов</returns>
        /// <response code="200">Теги успешно получены</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet]
        [Route("[action]")]
        [AllowAnonymous]
        public async Task<ActionResult<List<TagDto>>> GetAll([FromQuery] string? searchQuery = null)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetAll)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync($"Start GetAll tags request in {requestPath}", requestPath);

                var result = await Mediator.Send(new GetAllTagsRequest
                {
                    SearchQuery = searchQuery
                });

                return Ok(result);
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

