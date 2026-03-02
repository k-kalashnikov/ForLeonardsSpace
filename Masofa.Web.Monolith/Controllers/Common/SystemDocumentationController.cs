using Masofa.Common.Resources;
using Masofa.BusinessLogic.Common;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using Masofa.Common.Models.SystemDocumentation;
using Masofa.Common.Models.SystemCrical;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Masofa.Common.Services.FileStorage;
using Microsoft.EntityFrameworkCore;

namespace Masofa.Web.Monolith.Controllers.Common
{
    [Route("common/[controller]")]
    [ApiExplorerSettings(GroupName = "Common")]
    public class SystemDocumentationController : BaseCrudController<SystemDocumentation, MasofaCommonDbContext>
    {
        /// <summary>
        /// Инициализирует новый экземпляр <see cref="SystemDocumentationController"/>.
        /// </summary>
        public SystemDocumentationController(
            IFileStorageProvider fileStorageProvider,
            MasofaCommonDbContext dbContext,
            ILogger<SystemDocumentationController> logger,
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
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Получает список системной документации с размером файлов по запросу с фильтрацией, сортировкой и пагинацией
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
        public async Task<ActionResult<List<SystemDocumentationWithFileSize>>> CustomGetByQuery([FromBody] BaseGetQuery<SystemDocumentation> query)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(CustomGetByQuery)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                if (!ModelState.IsValid)
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelValidationFailed(requestPath, ModelState.ToString()), requestPath);
                    return BadRequest(ModelState);
                }

                var customRequest = new GetSystemDocumentationCustomRequest
                {
                    Query = query
                };

                var result = await Mediator.Send(customRequest);

                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath,result.Count.ToString()), requestPath);

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
        /// Создает или обновляет системную документацию. Принимает файл опционально.
        /// </summary>
        /// <param name="file">Опциональный файл для загрузки</param>
        /// <param name="model">Модель системной документации (передается как JSON в FormData)</param>
        /// <param name="language">Язык для файла (например, "ru-RU", "en-US", "uz-Latn-UZ")</param>
        /// <returns>Идентификатор документа</returns>
        /// <response code="200">Операция успешно выполнена</response>
        /// <response code="400">Некорректные данные</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost]
        [Route("[action]")]
        [Consumes("multipart/form-data")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, SystemAdmin, ModuleAdmin, Operator")]
        public async Task<ActionResult<Guid>> CreateWithFile(IFormFile? file, [FromForm] string model, [FromForm] string? language)
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

                SystemDocumentation systemDocumentation;
                try
                {
                    systemDocumentation = Newtonsoft.Json.JsonConvert.DeserializeObject<SystemDocumentation>(model);
                    if (systemDocumentation == null)
                    {
                        return BadRequest("Invalid model format");
                    }
                }
                catch (Exception ex)
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelDeserializationError(ex.Message), requestPath);
                    return BadRequest($"Invalid model format: {ex.Message}");
                }

                var command = new SystemDocumentationCreateWithFileCommand
                {
                    Model = systemDocumentation,
                    File = file,
                    Language = language,
                    Author = User?.Identity?.Name ?? string.Empty
                };

                var result = await Mediator.Send(command);

                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, result.ToString()), requestPath);
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
        /// Получает публичную документацию, сгруппированную по блокам (без авторизации)
        /// </summary>
        /// <returns>Список блоков документации с документами</returns>
        /// <response code="200">Список успешно получен</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet]
        [Route("[action]")]
        [AllowAnonymous]
        public async Task<ActionResult<List<SystemDocumentationBlockDto>>> GetPublicByBlocks()
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetPublicByBlocks)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                var request = new GetPublicSystemDocumentationByBlocksRequest();
                var result = await Mediator.Send(request);

                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath,result.Count.ToString()), requestPath);

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
        /// Получает файл публичной документации (без авторизации)
        /// </summary>
        /// <param name="documentationId">Идентификатор документации</param>
        /// <param name="language">Язык файла (например, "ru-RU", "en-US", "uz-Latn-UZ"). Если не указан, используется текущий язык из Accept-Language заголовка или первый доступный</param>
        /// <returns>Файл документации</returns>
        /// <response code="200">Файл успешно получен</response>
        /// <response code="404">Документация не найдена или не является публичной</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet]
        [Route("[action]/{documentationId}")]
        [AllowAnonymous]
        public async Task<ActionResult> GetPublicFile(Guid documentationId, [FromQuery] string? language = null)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetPublicFile)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                // Проверяем, что документация существует и является публичной
                var documentation = await DbContext.Set<SystemDocumentation>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(d => d.Id == documentationId && d.Status == StatusType.Active && d.Visible);

                if (documentation == null)
                {
                    await BusinessLogicLogger.LogWarningAsync(LogMessageResource.DocumentationNotFoundOrNotPublic(documentationId.ToString(),requestPath), requestPath);
                    return NotFound("Documentation not found or not public");
                }

                // Определяем язык для файла
                if (string.IsNullOrWhiteSpace(language))
                {
                    // Пытаемся получить язык из заголовка Accept-Language
                    var acceptLanguage = Request.Headers["Accept-Language"].FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(acceptLanguage))
                    {
                        // Парсим Accept-Language (например, "ru-RU,ru;q=0.9,en-US;q=0.8")
                        var languages = acceptLanguage.Split(',')
                            .Select(l => l.Split(';')[0].Trim())
                            .ToList();

                        // Сопоставляем с поддерживаемыми языками
                        var supportedLanguages = new[] { "ru-RU", "en-US", "uz-Latn-UZ", "uz-Cyrl-UZ", "ar-LB" };
                        foreach (var lang in languages)
                        {
                            var matchedLang = supportedLanguages.FirstOrDefault(sl => 
                                lang.StartsWith(sl, StringComparison.OrdinalIgnoreCase) || 
                                sl.StartsWith(lang.Split('-')[0], StringComparison.OrdinalIgnoreCase));
                            if (matchedLang != null && documentation.FileStorageIds[matchedLang].HasValue)
                            {
                                language = matchedLang;
                                break;
                            }
                        }
                    }
                }

                // Если язык не определен, пробуем найти любой доступный файл
                Guid? fileStorageId = null;
                if (!string.IsNullOrWhiteSpace(language))
                {
                    fileStorageId = documentation.FileStorageIds[language];
                }

                if (!fileStorageId.HasValue)
                {
                    // Пробуем найти файл для любого доступного языка
                    var supportedLanguages = new[] { "uz-Latn-UZ", "en-US", "ru-RU", "uz-Cyrl-UZ", "ar-LB" };
                    foreach (var lang in supportedLanguages)
                    {
                        var fileId = documentation.FileStorageIds[lang];
                        if (fileId.HasValue)
                        {
                            fileStorageId = fileId;
                            language = lang;
                            break;
                        }
                    }
                }

                if (!fileStorageId.HasValue)
                {
                    await BusinessLogicLogger.LogWarningAsync(LogMessageResource.DocumentationHasNoFile(documentationId.ToString(),requestPath), requestPath);
                    return NotFound("Documentation has no file for the requested language");
                }

                // Получаем информацию о файле
                var fileRequest = new GetFileInfoRequest { Id = fileStorageId.Value };
                var fileStorageItem = await Mediator.Send(fileRequest);

                if (fileStorageItem == null)
                {
                    await BusinessLogicLogger.LogWarningAsync(LogMessageResource.FileStorageItemNotFound(fileStorageId.Value.ToString(), requestPath), requestPath);
                    return NotFound("File not found");
                }

                // Получаем файл
                var fileBytes = await FileStorageProvider.GetFileBytesAsync(fileStorageItem);

                // Определяем тип контента
                var contentType = GetContentType(fileStorageItem.FileStoragePath);

                // Извлекаем имя файла
                var fileName = ExtractFileName(fileStorageItem.FileStoragePath) ?? "document";

                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath,"result"), requestPath);

                return File(fileBytes, contentType, fileName);
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
        /// Извлекает имя файла из пути
        /// </summary>
        private static string? ExtractFileName(string? filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return null;

            var parts = filePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length > 0 ? parts[^1] : null;
        }

        /// <summary>
        /// Определяет тип контента по расширению файла
        /// </summary>
        private static string GetContentType(string? filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return "application/octet-stream";

            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return extension switch
            {
                ".pdf" => "application/pdf",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".md" => "text/markdown",
                ".txt" => "text/plain",
                ".html" => "text/html",
                ".csv" => "text/csv",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".xls" => "application/vnd.ms-excel",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => "application/octet-stream"
            };
        }
    }
}

