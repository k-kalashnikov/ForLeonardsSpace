using Masofa.BusinessLogic;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.CropMonitoring;
using Masofa.Common.Resources;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Masofa.Web.Monolith.Controllers.CropMonitoring
{
    /// <summary>
    /// Контроллер для работы с загружаемыми полями
    /// </summary>
    [Route("cropMonitoring/[controller]")]
    [ApiExplorerSettings(GroupName = "CropMonitoring")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, SystemAdmin, ModuleAdmin, Operator, Foreman, FieldWorker")]
    public class ImportedFieldController : BaseCrudController<Masofa.Common.Models.CropMonitoring.ImportedField, MasofaCropMonitoringDbContext>
    {
        /// <summary>
        /// Конструктор контроллера для работы с загружаемыми полями
        /// </summary>
        public ImportedFieldController(
            IFileStorageProvider fileStorageProvider,
            MasofaCropMonitoringDbContext dbContext,
            ILogger<ImportedFieldController> logger,
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
        { }

        /// <summary>
        /// Метод для сохранения загружаемого поля в Season
        /// </summary>
        /// <param name="model">Объект запроса со списком файлов</param>
        /// <returns>Список найденных сущностей</returns>
        /// <response code="200">Успешно загружены поля</response>
        /// <response code="400">Некорректные параметры запроса</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        public virtual async Task<ActionResult<MergeFieldResult>> MergeImportedFieldToSeason([FromBody] Masofa.Common.Models.CropMonitoring.Season model)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(MergeImportedFieldToSeason)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                if (!ModelState.IsValid)
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelValidationFailed(requestPath, Newtonsoft.Json.JsonConvert.SerializeObject(model)), requestPath);
                    return BadRequest(ModelState);
                }
                var createRequest = new BaseCreateCommand<Masofa.Common.Models.CropMonitoring.Season, MasofaCropMonitoringDbContext>()
                {
                    Model = model,
                    Author = User.Identity.Name,
                };

                var newSeasonId = await Mediator.Send(createRequest);

                var result = new MergeFieldResult()
                {
                    Id = newSeasonId,
                };

                var getRequest = new BaseGetByIdRequest<Masofa.Common.Models.CropMonitoring.Season, MasofaCropMonitoringDbContext>()
                {
                    Id = newSeasonId,
                };

                var newSeason = await Mediator.Send(getRequest);

                if (newSeason != null)
                {
                    var inputPolygon = newSeason.Polygon;

                    if (inputPolygon == null || inputPolygon.IsEmpty)
                    {
                        throw new ArgumentException("Input polygon is null or empty.");
                    }

                    if (inputPolygon.SRID == 0)
                    {
                        inputPolygon.SRID = 4326;
                    }

                    var seasons = await DbContext.Seasons
                        .Where(s => s.Polygon != null && (s.Polygon.Intersects(inputPolygon)
                                                       || s.Polygon.Covers(inputPolygon)
                                                       || inputPolygon.Covers(s.Polygon)
                                                       || s.Polygon.EqualsTopologically(inputPolygon)))
                        .ToListAsync();

                    var newLog = new ImportedFieldLog()
                    {
                        SeasonId = newSeasonId,
                        IntersectedSeasons = seasons.Where(s => s.Polygon != null && s.Polygon.Intersects(inputPolygon)).Select(s => s.Id).ToList(),
                        CoveredSeasons = seasons.Where(s => s.Polygon != null && s.Polygon.Covers(inputPolygon)).Select(s => s.Id).ToList(),
                        CoveredBySeasons = seasons.Where(s => s.Polygon != null && inputPolygon.Covers(s.Polygon)).Select(s => s.Id).ToList(),
                        EqualPolygonsSeasons = seasons.Where(s => s.Polygon != null && s.Polygon.EqualsTopologically(inputPolygon)).Select(s => s.Id).ToList(),
                    };

                    var logCreateRequest = new BaseCreateCommand<Masofa.Common.Models.CropMonitoring.ImportedFieldLog, MasofaCropMonitoringDbContext>()
                    {
                        Model = newLog,
                        Author = User.Identity.Name,
                    };

                    await Mediator.Send(logCreateRequest);

                    var collisionCount = newLog.IntersectedSeasons.Count + newLog.CoveredSeasons.Count + newLog.CoveredBySeasons.Count + newLog.EqualPolygonsSeasons.Count;
                    if (collisionCount > 0)
                    {
                        result.Errors.Add($"New Season has {collisionCount} with existing Seasons");
                    }
                }

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

        public class MergeFieldResult
        {
            public Guid Id { get; set; }
            public List<string> Errors { get; set; } = [];
        }
    }
}