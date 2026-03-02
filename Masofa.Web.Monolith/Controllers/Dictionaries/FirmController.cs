using Masofa.BusinessLogic.Dictionaries;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using Masofa.Common.Models.Dictionaries;
using Masofa.Common.Resources;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Masofa.Web.Monolith.Controllers.Dictionaries
{
    /// <summary>
    /// Предоставляет API-методы для управления юридическими лицами
    /// </summary>
    [Route("dictionaries/[controller]")]
    [ApiExplorerSettings(GroupName = "Dictionaries")]
    public class FirmController : BaseDictionaryController<Firm, MasofaDictionariesDbContext>
    {
        /// <summary>
        /// Инициализирует новый экземпляр <see cref="FirmController"/>.
        /// </summary>
        /// <param name="fileStorageProvider">Провайдер для работы с хранилищем файлов</param>
        /// <param name="dbContext">Контекст базы данных</param>

        /// <param name="logger">Логгер</param>
        /// <param name="configuration">Конфигурация приложения</param>
        public FirmController(
            IFileStorageProvider fileStorageProvider,
            MasofaDictionariesDbContext dbContext,
            ILogger<FirmController> logger,
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
        public async Task<ActionResult<List<Firm>>> CustomGetByQuery([FromBody] BaseGetQuery<Firm> query)
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

                var request = new FirmCustomQueryRequest();
                if (query.Filters.Count != 0)
                {
                    foreach (var filter in query.Filters)
                    {
                        if (filter.FilterField == "businessTypeId")
                        {
                            if (filter.FilterValue != null)
                            {
                                request.BusinessTypeId = new Guid(filter.FilterValue.ToString() ?? string.Empty);
                            }
                        }
                        if (filter.FilterField == "inn")
                        {
                            if (filter.FilterValue != null)
                            {
                                request.Inn = filter.FilterValue.ToString();
                            }
                        }
                        if (filter.FilterField == "egrpo")
                        {
                            if (filter.FilterValue != null)
                            {
                                request.Egrpo = filter.FilterValue.ToString();
                            }
                        }
                        if (filter.FilterField == "mainRegionId")
                        {
                            if (filter.FilterValue != null)
                            {
                                request.RegionId = new Guid(filter.FilterValue.ToString() ?? string.Empty);
                            }
                        }

                    }
                }

                return await Mediator.Send(request);
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
        /// Retrieves the total count of items that match the specified query criteria.
        /// </summary>
        /// <remarks>This method processes the query using the mediator pattern to retrieve the total
        /// count of matching items. Ensure that the query object is properly constructed and that the model state is
        /// valid before calling this method.</remarks>
        /// <param name="query">The query object containing the criteria for filtering the items. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the total count of items
        /// matching the query criteria. Returns a <see cref="BadRequestObjectResult"/> if the model state is invalid,
        /// or a <see cref="StatusCodeResult"/> with status 500 if an internal server error occurs.</returns>
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        public async Task<ActionResult<int>> CustomGetTotalCount([FromBody] BaseGetQuery<Firm> query)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetTotalCount)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                if (!ModelState.IsValid)
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelValidationFailed(requestPath, Newtonsoft.Json.JsonConvert.SerializeObject(query)), requestPath);
                    return BadRequest(ModelState);
                }

                var request = new FirmCustomQueryRequest();
                if (query.Filters.Count != 0)
                {
                    foreach (var filter in query.Filters)
                    {
                        if (filter.FilterField == "businessTypeId")
                        {
                            if (filter.FilterValue != null)
                            {
                                request.BusinessTypeId = new Guid(filter.FilterValue.ToString() ?? string.Empty);
                            }
                        }
                        if (filter.FilterField == "inn")
                        {
                            if (filter.FilterValue != null)
                            {
                                request.Inn = filter.FilterValue.ToString();
                            }
                        }
                        if (filter.FilterField == "egrpo")
                        {
                            if (filter.FilterValue != null)
                            {
                                request.Egrpo = filter.FilterValue.ToString();
                            }
                        }
                        if (filter.FilterField == "mainRegionId")
                        {
                            if (filter.FilterValue != null)
                            {
                                request.RegionId = new Guid(filter.FilterValue.ToString() ?? string.Empty);
                            }
                        }

                    }
                }

                return (await Mediator.Send(request)).Count();
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