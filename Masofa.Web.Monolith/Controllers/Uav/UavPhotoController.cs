using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.BusinessLogic.Uav;
using Masofa.BusinessLogic.Uav.Commands;
using Masofa.BusinessLogic.Uav.Queries;
using Masofa.Common.Models;
using Masofa.Common.Models.Uav;
using Masofa.Common.Resources;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Masofa.Web.Monolith.Controllers.Uav
{
    /// <summary>
    /// Контроллер для работы с UAVFlyPath и снимками БПЛА
    /// </summary>
    [Route("uav/[controller]")]
    [ApiExplorerSettings(GroupName = "Uav")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, SystemAdmin, ModuleAdmin, Operator")]
    public class UavPhotoController : BaseCrudController<UAVFlyPath, MasofaUAVDbContext>
    {

        public UavPhotoController(
            IFileStorageProvider fileStorageProvider,
            MasofaUAVDbContext dbContext,
            ILogger<UavPhotoController> logger,
            IConfiguration configuration,
            IMediator mediator,
            IBusinessLogicLogger businessLogicLogger,
            IHttpContextAccessor httpContextAccessor
            ) : base(
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
        /// Получение списка вылетов (FlyPath)
        /// </summary>
        [HttpPost]
        [Route("[action]")]
        public override async Task<ActionResult<List<UAVFlyPath>>> GetByQuery([FromBody] BaseGetQuery<UAVFlyPath> query)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetByQuery)}";

            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                var command = new GetAllUavSurveysQuery
                {
                    QueryOptions = query ?? new BaseGetQuery<UAVFlyPath>()
                };

                var result = await Mediator.Send(command);

                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, result.Count.ToString()), requestPath);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var msg = $"Error getting surveys: {ex.Message}";
                Logger.LogError(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        /// <summary>
        /// Получение количества вылетов с учетом фильтров
        /// </summary>
        [HttpPost]
        [Route("[action]")]
        public override async Task<ActionResult<int>> GetTotalCount([FromBody] BaseGetQuery<UAVFlyPath> queryOptions)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetTotalCount)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                var query = new GetUavSurveysTotalCountQuery
                {
                    QueryOptions = queryOptions
                };
                var result = await Mediator.Send(query);
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, result.ToString()), requestPath);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var msg = $"Error inside GetTotalCount: {ex.Message}";
                Logger.LogError(ex, msg);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost]
        [Route("UploadSurveyArchive")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        public async Task<ActionResult<Guid>> UploadSurveyArchive([FromForm] UploadUavSurveyArchiveCommand command, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(UploadSurveyArchive)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                if (command.ZipFile == null || command.ZipFile.Length == 0)
                    return BadRequest("Zip file is empty");
                if (!Path.GetExtension(command.ZipFile.FileName).Equals(".zip", StringComparison.OrdinalIgnoreCase))
                    return BadRequest("File must be a .zip archive");
                var result = await Mediator.Send(command, cancellationToken);
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, result.ToString()), requestPath);
                return Ok(result);
            }
            catch (OperationCanceledException)
            {
                await BusinessLogicLogger.LogWarningAsync("Request cancelled by user", requestPath);
                return StatusCode(499, "Client Closed Request");
            }
            catch (Exception ex)
            {
                var msg = $"Error uploading survey archive: {ex.Message}";
                Logger.LogError(ex, msg);
                await BusinessLogicLogger.LogCriticalAsync(LogMessageResource.GenericError(requestPath, ex.Message), requestPath);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing the archive.");
            }
        }

        /// <summary>
        /// Получает изображение для указанного снимка (UAVPhoto).
        /// </summary>
        [HttpGet]
        [Route("GetImage/{photoId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetImage(Guid photoId)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetImage)}";

            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                var query = new GetUavImageQuery { PhotoId = photoId };
                var result = await Mediator.Send(query);
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, "FileStream returned"), requestPath);
                return new FileStreamResult(result.FileStream, result.ContentType)
                {
                };
            }
            catch (KeyNotFoundException ex)
            {
                Logger.LogWarning(ex, $"Image not found: {photoId}");
                return NotFound(ex.Message);
            }
            catch (FileNotFoundException ex)
            {
                Logger.LogWarning(ex, $"File not found on disk: {photoId}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                var msg = $"Error getting image: {ex.Message}";
                Logger.LogCritical(ex, msg);
                return StatusCode(500, "Internal Server Error");
            }
        }

        /// <summary>
        /// Обновление полей карточки съемки (UAVFlyPath)
        /// </summary>
        [HttpPut("UpdateSurvey")]
        [ProducesResponseType(typeof(UAVFlyPath), StatusCodes.Status200OK)]
        public async Task<ActionResult<UAVFlyPath>> UpdateSurvey([FromBody] UpdateUavSurveyCommand command)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(UpdateSurvey)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                var author = HttpContext.User?.Identity?.Name;
                if (string.IsNullOrEmpty(author))
                {
                    return Unauthorized("User not authenticated");
                }
                command.UserName = author;
                var result = await Mediator.Send(command);
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, result.Id.ToString()), requestPath);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        /// <summary>
        /// Удаление съемки (UAVSurvey).
        /// </summary>
        /// <param name="id">Идентификатор съемки (UAVSurvey ID)</param>
        [HttpDelete("DeleteSurvey/{id}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<bool>> DeleteSurvey(Guid id)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(DeleteSurvey)}";

            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                var author = HttpContext.User?.Identity?.Name;
                if (string.IsNullOrEmpty(author))
                {
                    return Unauthorized("User not authenticated");
                }

                var command = new DeleteUavSurveyCommand
                {
                    SurveyId = id,
                    UserName = author
                };

                var result = await Mediator.Send(command);

                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, result.ToString()), requestPath);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        /// <summary>
        /// Получает список полей, связанных с вылетом через коллекции фотографий
        /// </summary>
        [HttpGet("GetFieldRelations/{surveyId}")]
        [ProducesResponseType(typeof(List<UavSurveyFieldRelationDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<UavSurveyFieldRelationDto>>> GetFieldRelations(Guid surveyId)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetFieldRelations)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                var query = new GetUavFieldRelationsQuery
                {
                    SurveyId = surveyId
                };
                var result = await Mediator.Send(query);
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, $"Count: {result.Count}"), requestPath);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error inside GetFieldRelations for survey {surveyId}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        /// <summary>
        /// Удаление связи поля с конкретным вылетом (Soft Delete).
        /// Принимает ID вылета и ID поля. Находит все коллекции этого вылета и удаляет связи с указанным полем.
        /// </summary>
        /// <param name="surveyId">Идентификатор вылета (UAVFlyPath ID)</param>
        /// <param name="fieldId">Идентификатор поля (Field ID)</param>
        [HttpDelete("DeleteFieldRelation/{surveyId}/{fieldId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteFieldRelation(Guid surveyId, Guid fieldId)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(DeleteFieldRelation)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                var author = HttpContext.User?.Identity?.Name;
                if (string.IsNullOrEmpty(author))
                {
                    return Unauthorized("User not authenticated");
                }
                var command = new DeleteUavFieldRelationCommand
                {
                    SurveyId = surveyId,
                    FieldId = fieldId,
                    UserName = author
                };
                var result = await Mediator.Send(command);
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, result.ToString()), requestPath);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        /// <summary>
        /// Автоматический пересчет пространственных связей для всех фотографий вылета.
        /// Удаляет текущие связи и создает новые на основе координат фотографий.
        /// </summary>
        /// <param name="surveyId">Идентификатор вылета (UAVFlyPath ID)</param>
        [HttpPost("RecalculateSpatialRelations/{surveyId}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> RecalculateSpatialRelations(Guid surveyId)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(RecalculateSpatialRelations)}";

            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                var author = HttpContext.User?.Identity?.Name;
                if (string.IsNullOrEmpty(author))
                {
                    return Unauthorized("User not authenticated");
                }

                var command = new RecalculateUavSurveyRelationsCommand
                {
                    SurveyId = surveyId,
                    UserName = author
                };

                var result = await Mediator.Send(command);

                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, result.ToString()), requestPath);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        /// <summary>
        /// Добавление связи с полем (UAVSurveyFieldRelation).
        /// </summary>
        [HttpPost("AddFieldRelation")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> AddFieldRelation([FromBody] AddUavFieldRelationCommand command)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(AddFieldRelation)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                var author = HttpContext.User?.Identity?.Name;
                if (string.IsNullOrEmpty(author))
                {
                    return Unauthorized("User not authenticated");
                }
                command.UserName = author;
                var result = await Mediator.Send(command);
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, result.ToString()), requestPath);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }
    }
}