using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.BusinessLogic;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.CropMonitoring;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Schema;

namespace Masofa.Web.Monolith.Controllers.CropMonitoring
{
    /// <summary>
    /// Provides endpoints for managing bid templates in the Crop Monitoring module.
    /// </summary>
    /// <remarks>This controller allows importing bid templates from JSON files and retrieving the JSON schema
    /// used for validating bid templates. It is part of the Crop Monitoring API and requires appropriate authorization
    /// for most operations.</remarks>
    [Route("cropMonitoring/[controller]")]
    [ApiExplorerSettings(GroupName = "CropMonitoring")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, SystemAdmin, ModuleAdmin, Operator, Foreman, FieldWorker")]
    public class BidTemplatesController : BaseCrudController<BidTemplate, MasofaCropMonitoringDbContext>
    {
        private MasofaDictionariesDbContext MasofaDictionariesDbContext { get; set; }
        public BidTemplatesController(
            IFileStorageProvider fileStorageProvider,
            MasofaCropMonitoringDbContext dbContext,
            ILogger<BidTemplatesController> logger,
            IConfiguration configuration,
            IMediator mediator,
            MasofaDictionariesDbContext masofaDictionariesDbContext,
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
            MasofaDictionariesDbContext = masofaDictionariesDbContext;
        }

        /// <summary>
        /// Импортирует шаблоны заявок из JSON файлов
        /// </summary>
        /// <param name="files">Список JSON файлов с шаблонами заявок</param>
        /// <returns>Список сообщений о результатах импорта</returns>
        /// <response code="200">Импорт шаблонов выполнен</response>
        /// <response code="400">Некорректные файлы</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<List<string>>> ImportFromFiles(List<IFormFile> files)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(ImportFromFiles)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                var messages = new List<string>();
                foreach (var file in files)
                {
                    using (var fileStream = new StreamReader(file.OpenReadStream()))
                    {
                        var templateText = await fileStream.ReadToEndAsync();
                        var templateObj = JsonConvert.DeserializeObject<Masofa.Common.Models.CropMonitoring.BidTemplateSchemaVersion3.BidTemplateSchemaVersion3>(templateText);
                        var crop = MasofaDictionariesDbContext.Crops.FirstOrDefault(m => m.Id.Equals(templateObj.CropId));

                        if (crop == null)
                        {
                            messages.Add($"Crop with Id = {templateObj.CropId} not found");
                            continue;
                        }

                        if (DbContext.BidTemplates.Any(bt =>
                            bt.ContentVersion.Equals(templateObj.ContentVersion)
                            && bt.SchemaVersion.Equals(templateObj.SchemaVersion)
                            && bt.CropId.Equals(templateObj.CropId)))
                        {
                            messages.Add($"Bid Templates with CropId = {templateObj.CropId} and ContentVersion = {templateObj.ContentVersion} and SchemaVersion = {templateObj.SchemaVersion} already exist");
                            continue;
                        }

                        var bidTemplate = new BidTemplate()
                        {
                            ContentVersion = templateObj.ContentVersion,
                            SchemaVersion = templateObj.SchemaVersion,
                            CropId = templateObj.CropId,
                            Data = templateObj
                        };

                        var btId = await Mediator.Send(new BaseCreateCommand<BidTemplate, MasofaCropMonitoringDbContext>()
                        {
                            Author = User.Identity.Name,
                            Model = bidTemplate
                        });
                        messages.Add($"Bid Templates with CropId = {templateObj.CropId} and ContentVersion = {templateObj.ContentVersion} and SchemaVersion = {templateObj.SchemaVersion} created with id {btId}");
                    }
                }
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, messages.Count.ToString()), requestPath);
                return messages;
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
        /// Получает JSON схему для шаблонов заявок
        /// </summary>
        /// <returns>JSON схема для валидации шаблонов заявок</returns>
        /// <response code="200">JSON схема успешно получена</response>
        [HttpGet]
        [Route("[action]")]
        [AllowAnonymous]
        public ActionResult<string> GetSchema()
        {
            JsonSerializerOptions options = JsonSerializerOptions.Default;
            JsonNode schema = options.GetJsonSchemaAsNode(typeof(Masofa.Common.Models.CropMonitoring.BidTemplateSchemaVersion3.BidTemplateSchemaVersion3));
            return Json(schema);
        }
    }
}
