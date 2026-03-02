using Masofa.Common.Resources;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.BusinessLogic.Satellite;
using Masofa.Common.Models.Dictionaries;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Masofa.Web.Monolith.Controllers.Common
{
    /// <summary>
    /// Предоставляет API-методы для управления связями тегов с объектами
    /// </summary>
    [Route("common/[controller]")]
    [ApiExplorerSettings(GroupName = "Common")]
    public class TagRelationController : BaseCrudController<TagRelation, MasofaCommonDbContext>
    {
        /// <summary>
        /// Инициализирует новый экземпляр <see cref="TagRelationController"/>.
        /// </summary>
        /// <param name="fileStorageProvider">Провайдер для работы с хранилищем файлов</param>
        /// <param name="dbContext">Контекст базы данных</param>
        /// <param name="logger">Логгер</param>
        /// <param name="configuration">Конфигурация приложения</param>
        /// <param name="mediator">Медиатор для CQRS</param>
        /// <param name="businessLogicLogger">Логгер бизнес-логики</param>
        /// <param name="httpContextAccessor">Доступ к HTTP контексту</param>
        public TagRelationController(
            IFileStorageProvider fileStorageProvider,
            MasofaCommonDbContext dbContext,
            ILogger<TagRelationController> logger,
            IConfiguration configuration,
            IMediator mediator,
            IBusinessLogicLogger businessLogicLogger,
            IHttpContextAccessor httpContextAccessor)
            : base(
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
        /// Получает теги для указанного объекта
        /// </summary>
        /// <param name="ownerId">ID объекта-владельца</param>
        /// <param name="ownerTypeFullName">Полное имя типа объекта-владельца</param>
        /// <returns>Список тегов с полными данными</returns>
        /// <response code="200">Теги успешно получены</response>
        /// <response code="400">Некорректные параметры запроса</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet]
        [Route("[action]")]
        [AllowAnonymous]
        public async Task<ActionResult<List<TagDto>>> GetByOwner([FromQuery] Guid ownerId, [FromQuery] string ownerTypeFullName)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetByOwner)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                // Получаем связи тегов из MasofaCommonDbContext
                var tagRelations = await DbContext.Set<TagRelation>()
                    .AsNoTracking()
                    .Where(tr => tr.OwnerId == ownerId && tr.OwnerTypeFullName == ownerTypeFullName)
                    .ToListAsync();

                if (!tagRelations.Any())
                {
                    await BusinessLogicLogger.LogInformationAsync(LogMessageResource.NoTagRelationsFound(ownerId.ToString()), requestPath);
                    return Ok(new List<TagDto>());
                }

                // Получаем ID тегов
                var tagIds = tagRelations.Select(tr => tr.TagId).ToList();

                // Получаем теги из MasofaDictionariesDbContext через MediatR
                var tags = await Mediator.Send(new GetAllTagsRequest());
                
                // Фильтруем теги по найденным ID
                var resultTags = tags.Where(t => tagIds.Contains(t.Id)).ToList();

                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, resultTags.Count.ToString()), requestPath);
                return Ok(resultTags);
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
