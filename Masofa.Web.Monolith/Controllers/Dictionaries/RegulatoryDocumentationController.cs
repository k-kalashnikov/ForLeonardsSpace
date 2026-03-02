using Masofa.Common.Resources;
using Masofa.BusinessLogic.Common;
using Masofa.BusinessLogic.Dictionaries;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using Masofa.Common.Models.Dictionaries;
using Masofa.Common.Models.SystemCrical;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Masofa.Common.Services.FileStorage;

namespace Masofa.Web.Monolith.Controllers.Dictionaries
{
    [Route("dictionaries/[controller]")]
    [ApiExplorerSettings(GroupName = "Dictionaries")]
    public class RegulatoryDocumentationController : BaseDictionaryController<RegulatoryDocumentation, MasofaDictionariesDbContext>
    {
        /// <summary>
        /// Инициализирует новый экземпляр <see cref="RegulatoryDocumentationController"/>.
        /// </summary>
        public RegulatoryDocumentationController(
            IFileStorageProvider fileStorageProvider,
            MasofaDictionariesDbContext dbContext,
            ILogger<RegulatoryDocumentationController> logger,
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
        /// Получает информацию о файле
        /// </summary>
        /// <param name="id">Идентификатор файла</param>
        /// <returns>Информация о файле</returns>
        [HttpGet]
        [Route("[action]/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<FileStorageItem>> GetFileInfo(Guid id)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetFileInfo)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                var request = new GetFileInfoRequest { Id = id };
                var fileStorageItem = await Mediator.Send(request);

                return Ok(fileStorageItem);
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath,ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Получает список нормативной документации с размером файлов по запросу с фильтрацией, сортировкой и пагинацией
        /// </summary>
        /// <param name="query">Объект запроса с параметрами фильтрации, сортировки и пагинации</param>
        /// <returns>Список найденных документов с информацией о размере файлов</returns>
        /// <response code="200">Список успешно получен</response>
        /// <response code="400">Некорректные параметры запроса</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost]
        [Route("CustomGetByQuery")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<List<RegulatoryDocumentationWithFileSize>>> CustomGetByQuery([FromBody] BaseGetQuery<RegulatoryDocumentation> query)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(CustomGetByQuery)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                if (!ModelState.IsValid)
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelValidationFailed(requestPath, query.ToString()), requestPath);
                    return BadRequest(ModelState);
                }

                var customRequest = new GetRegulatoryDocumentationCustomRequest
                {
                    Query = query
                };

                var result = await Mediator.Send(customRequest);

                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, result.Count.ToString()), requestPath);

                return Ok(result);
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath,ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Создает или обновляет нормативную документацию. Принимает файл опционально.
        /// </summary>
        /// <param name="file">Опциональный файл для загрузки</param>
        /// <param name="model">Модель нормативной документации (передается как JSON в FormData)</param>
        /// <returns>Идентификатор документа</returns>
        /// <response code="200">Операция успешно выполнена</response>
        /// <response code="400">Некорректные данные</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost]
        [Route("[action]")]
        [Consumes("multipart/form-data")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, SystemAdmin, ModuleAdmin, Operator")]
        public async Task<ActionResult<Guid>> CreateWithFile(IFormFile file, [FromForm] string model)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(CreateWithFile)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                if (string.IsNullOrEmpty(model))
                {
                    await BusinessLogicLogger.LogErrorAsync("LogMessageResource.ModelIsEmptyOrNull()", requestPath);
                    return BadRequest("Model is required");
                }

                RegulatoryDocumentation regulatoryDocumentation;
                try
                {
                    regulatoryDocumentation = Newtonsoft.Json.JsonConvert.DeserializeObject<RegulatoryDocumentation>(model);
                    if (regulatoryDocumentation == null)
                    {
                        return BadRequest("Invalid model format");
                    }
                }
                catch (Exception ex)
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelDeserializationError(ex.Message), requestPath);
                    return BadRequest($"Invalid model format: {ex.Message}");
                }

                var command = new RegulatoryDocumentationCreateWithFileCommand
                {
                    Model = regulatoryDocumentation,
                    File = file,
                    Author = User?.Identity?.Name ?? string.Empty
                };

                var result = await Mediator.Send(command);

                await BusinessLogicLogger.LogInformationAsync($"Regulatory documentation created/updated successfully with ID: {result}", requestPath);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var msg = $"Error creating/updating regulatory documentation with file in {requestPath}: {ex.Message}";
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}

