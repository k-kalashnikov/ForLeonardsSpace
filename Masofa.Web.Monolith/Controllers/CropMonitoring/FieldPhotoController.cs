using Masofa.Common.Resources;
using Masofa.BusinessLogic.FieldPhotoRequest;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.CropMonitoring;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using Masofa.Web.Monolith.ViewModels.FieldPhoto;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Masofa.Web.Monolith.Controllers.CropMonitoring
{
    /// <summary>
    /// Контроллер для работы со снимками полей
    /// </summary>
    [Route("cropMonitoring/[controller]")]
    [ApiExplorerSettings(GroupName = "CropMonitoring")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, SystemAdmin, ModuleAdmin, Operator, Foreman, FieldWorker")]
    public class FieldPhotoController : BaseCrudController<FieldPhoto, MasofaCropMonitoringDbContext>
    {
        private readonly int _fieldPhotosMaxCount;

        public FieldPhotoController(IFileStorageProvider fileStorageProvider, MasofaCropMonitoringDbContext dbContext, ILogger<FieldPhotoController> logger, IConfiguration configuration, IMediator mediator, IBusinessLogicLogger businessLogicLogger, IHttpContextAccessor httpContextAccessor) : base(fileStorageProvider, dbContext, logger, configuration, mediator, businessLogicLogger, httpContextAccessor)
        {
            _fieldPhotosMaxCount = configuration.GetValue<int>("FieldPhotosMaxCount", 500);
        }

        /// <summary>
        /// Загружает фото для поля
        /// </summary>
        /// <param name="file">Файл изображения</param>
        /// <param name="model">Модель с данными для сохранения снимка поля</param>
        /// <returns>Идентификатор загруженного файла</returns>
        /// <response code="200">Изображение успешно загружено</response>
        /// <response code="400">Файл пустой или некорректный</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="404">Снимок БПЛА с указанным ID не найден</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost]
        [Route("[action]")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<Guid>> UploadImage(IFormFile file, [FromForm] FieldPhotoCreateViewModel model)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(UploadImage)}";

            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                if (!ModelState.IsValid)
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelValidationFailed(requestPath, model.ToString()), requestPath);
                    return BadRequest(ModelState);
                }

                if (file == null || file.Length == 0)
                {
                    var errorMsg = LogMessageResource.FileIsEmptyOrNull();
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return BadRequest(errorMsg);
                }

                if (model.FieldId.HasValue && _fieldPhotosMaxCount > 0)
                {
                    var currentFieldPhotosCount = DbContext.FieldPhotos
                        .Where(p => p.FieldId == model.FieldId && p.Status == Masofa.Common.Models.StatusType.Active)
                        .Count();

                    if (currentFieldPhotosCount >= _fieldPhotosMaxCount)
                    {
                        var errorMsg = $"This field has max of {_fieldPhotosMaxCount} photos";
                        await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                        return BadRequest(errorMsg);
                    }
                }

                // Извлекаем TagIds из FormCollection, так как ASP.NET Core может не правильно биндить массивы из FormData
                var tagIds = new List<Guid>();
                if (Request.Form.ContainsKey("TagIds"))
                {
                    foreach (var tagIdString in Request.Form["TagIds"])
                    {
                        if (Guid.TryParse(tagIdString, out var tagId))
                        {
                            tagIds.Add(tagId);
                        }
                    }
                }
                else if (model.TagIds != null)
                {
                    tagIds = model.TagIds.ToList();
                }
                
                var authorName = User?.Identity?.Name;
                if (string.IsNullOrWhiteSpace(authorName))
                {
                    authorName = HttpContext?.User?.Identity?.Name;
                }

                if (string.IsNullOrWhiteSpace(authorName))
                {
                    var errorMsg = "Author is not defined for upload request";
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return Unauthorized(errorMsg);
                }

                var command = new UploadFieldPhotoCommand
                {
                    File = file,
                    Title = model.Title,
                    FieldId = model.FieldId,
                    RegionId = model.RegionId,
                    ParentRegionId = model.ParentRegionId,
                    CaptureDateUtc = model.CaptureDateUtc,
                    Description = model.Description,
                    PointJson = model.PointJson,
                    TagIds = tagIds,
                    Author = authorName
                };

                var result = await Mediator.Send(command);

                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, result.ToString()), requestPath);
                return Ok(result);
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
        /// Определяет поле и регион по координате
        /// </summary>
        /// <param name="latitude">Широта</param>
        /// <param name="longitude">Долгота</param>
        /// <returns>Информация о найденном поле и регионе</returns>
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<FieldLocationMatchDto>> LookupFieldByPoint([FromQuery] double latitude, [FromQuery] double longitude)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(LookupFieldByPoint)}";

            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                var request = new FindFieldByPointRequest
                {
                    Latitude = latitude,
                    Longitude = longitude
                };

                var result = await Mediator.Send(request);
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
        /// Возвращает список снимков с фильтрацией и пагинацией
        /// </summary>
        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<FieldPhotoGalleryResponse>> GetGallery([FromBody] GetFieldPhotoGalleryRequest request)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetGallery)}";

            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                if (!ModelState.IsValid)
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelValidationFailed(requestPath, request.ToString()), requestPath);
                    return BadRequest(ModelState);
                }

                var result = await Mediator.Send(request);
                return Ok(result);
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
        /// Обновляет снимок поля
        /// </summary>
        /// <param name="viewModel">Данные для обновления, включая tagIds</param>
        /// <returns>Обновленный снимок</returns>
        [HttpPut]
        [Route("UpdateFieldPhoto")]
        public async Task<ActionResult<FieldPhoto>> UpdateFieldPhoto([FromBody] FieldPhotoUpdateViewModel viewModel)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(UpdateFieldPhoto)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync($"Start request in {requestPath}", requestPath);

                if (!ModelState.IsValid)
                {
                    await BusinessLogicLogger.LogErrorAsync($"Model is not valid in {requestPath}. Model: {Newtonsoft.Json.JsonConvert.SerializeObject(viewModel)}", requestPath);
                    return BadRequest(ModelState);
                }

                var authorName = User?.Identity?.Name;
                if (string.IsNullOrWhiteSpace(authorName))
                {
                    authorName = HttpContext?.User?.Identity?.Name;
                }

                if (string.IsNullOrWhiteSpace(authorName))
                {
                    var errorMsg = "Author is not defined for update request";
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return Unauthorized(errorMsg);
                }

                var command = new UpdateFieldPhotoCommand
                {
                    Id = viewModel.Id,
                    Title = viewModel.Title,
                    Description = viewModel.Description,
                    CaptureDateUtc = viewModel.CaptureDateUtc,
                    TagIds = viewModel.TagIds ?? Enumerable.Empty<Guid>(),
                    Author = authorName
                };

                var result = await Mediator.Send(command);
                await BusinessLogicLogger.LogInformationAsync($"Finish request in {requestPath} with result: {result.Id}", requestPath);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var msg = $"Something wrong in {requestPath}. {ex.Message}";
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Возвращает справочную информацию для фильтров страницы
        /// </summary>
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<FieldPhotoMetadataResponse>> GetMetadata()
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetMetadata)}";

            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                var response = await Mediator.Send(new GetFieldPhotoMetadataRequest());
                return Ok(response);
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
