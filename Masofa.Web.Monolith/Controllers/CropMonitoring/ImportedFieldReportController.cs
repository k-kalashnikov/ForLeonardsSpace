using Masofa.BusinessLogic.CropMonitoring.ImportedFields;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using Masofa.Common.Resources;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using Masofa.Web.Monolith.ViewModels.ImportedField;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Masofa.Web.Monolith.Controllers.CropMonitoring
{
    /// <summary>
    /// Контроллер для работы с отчетами загружаемых полей
    /// </summary>
    [Route("cropMonitoring/[controller]")]
    [ApiExplorerSettings(GroupName = "CropMonitoring")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, SystemAdmin, ModuleAdmin, Operator, Foreman, FieldWorker")]
    public class ImportedFieldReportController : BaseCrudController<Masofa.Common.Models.CropMonitoring.ImportedFieldReport, MasofaCropMonitoringDbContext>
    {
        /// <summary>
        /// Конструктор контроллера для работы с отчетами загружаемых полей
        /// </summary>
        public ImportedFieldReportController(
            IFileStorageProvider fileStorageProvider,
            MasofaCropMonitoringDbContext dbContext,
            ILogger<ImportedFieldReportController> logger,
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
        /// Импорт полей
        /// </summary>
        /// <param name="viewModel">Объект запроса со списком файлов</param>
        /// <returns>Список найденных сущностей</returns>
        /// <response code="200">Успешно загружены поля</response>
        /// <response code="400">Некорректные параметры запроса</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        public virtual async Task<ActionResult<Dictionary<Guid, List<Guid>>>> ImportFields([FromForm] ImportFiledsViewModel viewModel)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(ImportFields)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                if (!ModelState.IsValid)
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelValidationFailed(requestPath, Newtonsoft.Json.JsonConvert.SerializeObject(viewModel)), requestPath);
                    return BadRequest(ModelState);
                }

                var importFieldsrequest = new ImportFiledsRequest()
                {
                    Comment = viewModel.Comment,
                    Files = viewModel.Files,
                    Author = User.Identity?.Name ?? string.Empty,
                };

                return await Mediator.Send(importFieldsrequest);
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
        /// Получения списка атрибутов на загружаемых полях
        /// </summary>
        /// <param name="id">Идентификатор сессии</param>
        /// <returns>Список атрибутов</returns>
        /// <response code="200">Успешно составлен список</response>
        /// <response code="400">Некорректные параметры запроса</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("[action]/{id}")]
        public virtual async Task<ActionResult<List<string>>> GetAttributes(Guid id)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetAttributes)}";
            try
            {
                var importedFields = await DbContext.ImportedFields
                    .Where(f => f.ImportedFieldReportId == id && f.DataJson != null && f.Status == StatusType.Active)
                    .ToListAsync();

                var keys = new HashSet<string>();

                foreach (var importedField in importedFields)
                {
                    if (string.IsNullOrWhiteSpace(importedField.DataJson)) continue;

                    try
                    {
                        using var doc = JsonDocument.Parse(importedField.DataJson);
                        if (doc.RootElement.ValueKind == JsonValueKind.Object)
                        {
                            foreach (var property in doc.RootElement.EnumerateObject())
                            {
                                keys.Add(property.Name);
                            }
                        }
                    }
                    catch { }
                }

                return keys.ToList();
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
