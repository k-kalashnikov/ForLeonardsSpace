using Masofa.BusinessLogic;
using Masofa.BusinessLogic.CropMonitoring;
using Masofa.BusinessLogic.CropMonitoring.Fields;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using Masofa.Common.Models.CropMonitoring;
using Masofa.Common.Resources;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Masofa.Web.Monolith.Controllers.CropMonitoring
{
    /// <summary>
    /// Контроллер для работы с полями
    /// </summary>
    [Route("cropMonitoring/[controller]")]
    [ApiExplorerSettings(GroupName = "CropMonitoring")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, SystemAdmin, ModuleAdmin, Operator, Foreman, FieldWorker")]

    public class FieldController : BaseCrudController<Masofa.Common.Models.CropMonitoring.Field, MasofaCropMonitoringDbContext>
    {
        private MasofaDictionariesDbContext DictionariesDbContext { get; set; }

        IMediator Mediator { get; set; }
        private MasofaCropMonitoringDbContext MasofaCropMonitoringDbContext { get; set; }
        public FieldController(
            IFileStorageProvider fileStorageProvider,
            MasofaCropMonitoringDbContext dbContext,
            ILogger<FieldController> logger,
            IConfiguration configuration,
            IMediator mediator,
            IBusinessLogicLogger businessLogicLogger,
            IHttpContextAccessor httpContextAccessor,
            MasofaDictionariesDbContext dictionariesDbContext) : base(
                fileStorageProvider,
                dbContext,
                logger,
                configuration,
                mediator,
                businessLogicLogger,
                httpContextAccessor)
        {
            MasofaCropMonitoringDbContext = dbContext;
            Mediator = mediator;
            DictionariesDbContext = dictionariesDbContext;
        }

        /// <summary>
        /// Импортирует поля из GeoJson, KML, CSV или ESRI Shapefile файлов
        /// </summary>
        /// <param name="exportType">Тип экспорта полей (GeoJson = 1, Kml = 2, CSV = 3, ShapeFile = 4)</param>
        /// <param name="file">KML файл с данными полей</param>
        /// <returns>Результат импорта с количеством созданных полей и их ID</returns>
        /// <response code="200">Поля успешно импортированы</response>
        /// <response code="400">Файл пустой или некорректный</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost]
        [Route("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ImportFields(FieldExportType exportType, IFormFile file)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(ImportFields)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                if (file is null || file.Length == 0)
                {
                    var errorMsg = LogMessageResource.KmlFileIsEmpty();
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return BadRequest(errorMsg);
                }

                await using var s = file.OpenReadStream();
                using var memoryStream = new MemoryStream();
                await s.CopyToAsync(memoryStream);

                var fields = await Mediator.Send(new FieldImportRequest { Bytes = memoryStream.ToArray(), ExportType = exportType });

                var resultIds = new List<Guid>(fields.Count);
                foreach (var field in fields)
                {
                    var id = await Mediator.Send(
                        new BaseCreateCommand<Masofa.Common.Models.CropMonitoring.Field, MasofaCropMonitoringDbContext> { Model = field });
                    resultIds.Add(id);
                }

                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, resultIds.Count.ToString()), requestPath);
                return Ok(new { created = resultIds.Count, ids = resultIds });
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
        /// Экспортирует поля в формате GeoJson, KML, CSV или ESRI Shapefile в ZIP архиве
        /// </summary>
        /// <param name="viewModel">Параметры экспорта полей</param>
        /// <returns>ZIP файл с KML данными полей</returns>
        /// <response code="200">ZIP файл с экспортированными полями</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost]
        [Route("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ExportFields([FromBody] FieldExportRequest viewModel)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(ExportFields)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var request = new FieldOrTemplateExportRequest()
                {
                    FieldsQuery = viewModel.FieldsQuery,
                    FieldExportType = viewModel.FieldExportType,
                    IsTemplate = false
                };

                var zip = await Mediator.Send(request);
                return File(zip, "application/zip", $"fields_{GetExt(viewModel.FieldExportType)}.zip");
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
        /// Образец файла импрота в форматах: GeoJson, KML, CSV, ESRI Shapefile
        /// </summary>
        /// <param name="exportType">Тип экспорта полей (GeoJson = 1, Kml = 2, CSV = 3, ShapeFile = 4)</param>
        /// <returns>ZIP файл с KML данными полей</returns>
        /// <response code="200">ZIP файл с экспортированными полями</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet]
        [Route("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ImportFieldsTemplate(FieldExportType exportType)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(ImportFieldsTemplate)}";
            try
            {
                var templateRequest = new FieldOrTemplateExportRequest()
                {
                    FieldExportType = exportType,
                    IsTemplate = true
                };

                var zip = await Mediator.Send(templateRequest);
                return File(zip, "application/zip", $"fields_{GetExt(exportType)}.zip");
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
        /// Находит поле по географическим координатам точки
        /// </summary>
        /// <param name="x">Долгота (longitude)</param>
        /// <param name="y">Широта (latitude)</param>
        /// <param name="srid">Система координат (по умолчанию 4326)</param>
        /// <returns>Поле, содержащее указанную точку</returns>
        /// <response code="200">Поле найдено</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="404">Поле с указанными координатами не найдено</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet]
        [Route("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetByPoints([FromQuery] double x, [FromQuery] double y, [FromQuery] int srid = 4326)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetByPoints)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                var pt = new Point(x, y) { SRID = srid };

                var fieldsFromBd = await MasofaCropMonitoringDbContext.Fields
                    .Where(f => f.Polygon != null && f.Status == Masofa.Common.Models.StatusType.Active)
                    .ToListAsync();

                var fields = new List<Field>();

                foreach (var f in fieldsFromBd)
                {
                    if (f.Polygon != null && f.Polygon.SRID != srid)
                    {
                        f.Polygon.SRID = srid;
                    }

                    if (f is null)
                    {
                        return NotFound($"Entity {nameof(Field)} by coordinates not found");
                    }

                    fields.Add(f);
                }

                var field = fields.FirstOrDefault(f => f.Polygon != null && f.Polygon.Intersects(pt));

                if (field is null)
                {
                    var errorMsg = LogMessageResource.EntityNotFound(typeof(Field).FullName, $"lon {x}; lat {y}; srid {srid}");
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return NotFound(errorMsg);
                }

                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, field.ToString()), requestPath);
                return Ok(field);
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        public override async Task<ActionResult<List<Field>>> GetByQuery([FromBody] BaseGetQuery<Field> query)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Create)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var getRequest = new BaseGetRequest<Field, MasofaCropMonitoringDbContext>()
                {
                    Query = query
                };
                var fields = await Mediator.Send(getRequest);

                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, fields.Count.ToString()), requestPath);
                return Ok(fields);
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        public async Task<ActionResult<List<CustomFieldViewModel>>> CustomGetByQuery([FromBody] BaseGetQuery<Field> query, CancellationToken ct)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(CustomGetByQuery)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var fields = await Mediator.Send(new GetFieldByQueryRequest
                {
                    Query = query
                },
                ct);

                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, fields.Count.ToString()), requestPath);
                return Ok(fields);
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        public async Task<ActionResult<List<CustomFieldViewModel>>> CustomGetTotalCountByQuery([FromBody] BaseGetQuery<Field> query, CancellationToken ct)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(CustomGetByQuery)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var fieldsCount = await Mediator.Send(new GetTotalCountFieldByQueryRequest
                {
                    Query = query
                },
                ct);

                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, fieldsCount.ToString()), requestPath);
                return Ok(fieldsCount);
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("[action]/{id}")]
        public override async Task<ActionResult<Field>> GetById(Guid id)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Create)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                var getRequest = new BaseGetByIdRequest<Field, MasofaCropMonitoringDbContext>()
                {
                    Id = id
                };
                var result = await Mediator.Send(getRequest);
                if (result == null)
                {
                    return NotFound($"Entity with type {typeof(Field)} with Id = {id.ToString()} not found");
                }

                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, Newtonsoft.Json.JsonConvert.SerializeObject(result)), requestPath);
                return Ok(new
                {
                    Entity = result,
                });
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        public async Task<ActionResult<List<FieldSummaryItem>>> GetSummary()
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetSummary)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                var regionsL2 = await DictionariesDbContext.Regions
                    .Where(r => r.Level == 2 && r.Status == StatusType.Active && r.Visible)
                    .ToListAsync();

                var fields = await DbContext.Fields
                    .Where(f => f.Status == StatusType.Active && f.RegionId != null && regionsL2.Select(r => r.Id).ToList().Contains(f.RegionId.Value))
                    .Select(f => new FieldSummaryItem()
                    {
                        RegionId = f.RegionId,
                        Area = f.FieldArea ?? 0,
                        Count = 0
                    })
                .ToListAsync();

                var result = new Dictionary<Guid, FieldSummaryItem>();
                foreach (var field in fields)
                {
                    if ((field.RegionId == null) || (field.RegionId == Guid.Empty))
                    {
                        continue;
                    }
                    if (!result.ContainsKey(field.RegionId.Value))
                    {
                        result.Add(field.RegionId.Value, new FieldSummaryItem()
                        {
                            RegionId = field.RegionId,
                            Area = 0,
                            Count = 0
                        });
                    }
                    result[field.RegionId.Value].Area += field.Area;
                    result[field.RegionId.Value].Count += 1;
                }

                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, result.Count.ToString()), requestPath);

                return Ok(result.Select(m => m.Value).ToList());
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        public async Task<ActionResult<Dictionary<Guid, int>>> GetFieldsByRegions([FromBody] RegionIdsRequest<Field> viewModel)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetFieldsByRegions)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                var fieldsByFilter = await Mediator.Send(new BaseGetRequest<Masofa.Common.Models.CropMonitoring.Field, MasofaCropMonitoringDbContext>()
                {
                    Query = viewModel.Query
                });

                var fields = fieldsByFilter
                    .Where(f => f.RegionId != null && viewModel.RegionIds.Contains(f.RegionId.Value))
                    .ToList();

                Dictionary<Guid, int> result = [];
                foreach (var field in fields)
                {
                    if (field.RegionId == null)
                    {
                        continue;
                    }

                    if (!viewModel.RegionIds.Contains(field.RegionId.Value))
                    {
                        continue;
                    }

                    if (!result.ContainsKey(field.RegionId.Value))
                    {
                        result[field.RegionId.Value] = 0;
                    }

                    result[field.RegionId.Value] += 1;
                }

                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, result.Count.ToString()), requestPath);

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

        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        public async Task<ActionResult<List<Field>>> GetFieldsByCoordinates([FromBody] BoundingBoxRequest<Field> viewModel)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetFieldsByRegions)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                if (viewModel.West >= viewModel.East || viewModel.South >= viewModel.North)
                {
                    return BadRequest("Invalid bounding box coordinates.");
                }

                // Преобразуем decimal → double
                var west = (double)viewModel.West;
                var east = (double)viewModel.East;
                var south = (double)viewModel.South;
                var north = (double)viewModel.North;

                // Создаём углы полигона (по часовой стрелке)
                var coordinates = new[]
                {
                    new NetTopologySuite.Geometries.Coordinate(west, south),
                    new NetTopologySuite.Geometries.Coordinate(east, south),
                    new NetTopologySuite.Geometries.Coordinate(east, north),
                    new NetTopologySuite.Geometries.Coordinate(west, north),
                    new NetTopologySuite.Geometries.Coordinate(west, south)
                };

                var geometryFactory = new GeometryFactory(new PrecisionModel(), 4326);
                var boundingBox = geometryFactory.CreatePolygon(coordinates);

                var fieldsByQuery = await Mediator.Send(new BaseGetRequest<Field, MasofaCropMonitoringDbContext>()
                {
                    Query = viewModel.Query
                });

                var result = fieldsByQuery.Where(f => f.Polygon != null && f.Polygon.Intersects(boundingBox)).ToList();

                if (result == null)
                {
                    return NotFound();
                }

                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, result.Count.ToString()), requestPath);

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
        /// Создает новое поле с валидацией пересечений и вхождения контуров
        /// </summary>
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public override async Task<ActionResult<Guid>> Create([FromBody] Field model)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Create)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                if (!ModelState.IsValid)
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelValidationFailed(requestPath, Newtonsoft.Json.JsonConvert.SerializeObject(model)), requestPath);
                    return BadRequest(ModelState);
                }

                // Validate polygon if present
                if (model.Polygon != null && !model.Polygon.IsEmpty)
                {
                    var validationResult = await ValidateFieldPolygon(model.Polygon, model.Id);
                    if (!validationResult.IsValid)
                    {
                        await BusinessLogicLogger.LogErrorAsync(validationResult.ErrorMessage, requestPath);
                        return BadRequest(validationResult.ErrorMessage);
                    }
                }

                var createRequest = new BaseCreateCommand<Field, MasofaCropMonitoringDbContext>()
                {
                    Model = model,
                    Author = User.Identity?.Name ?? string.Empty
                };

                var result = await Mediator.Send(createRequest);
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, result.ToString()), requestPath);
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
        /// Валидирует полигон поля на пересечения и вхождения
        /// </summary>
        private async Task<(bool IsValid, string ErrorMessage)> ValidateFieldPolygon(Polygon newPolygon, Guid? excludeFieldId = null)
        {
            try
            {
                // Get all existing fields with polygons
                var existingFields = await MasofaCropMonitoringDbContext.Set<Field>()
                    .Where(f => f.Polygon != null && !f.Polygon.IsEmpty && (excludeFieldId == null || f.Id != excludeFieldId.Value))
                    .ToListAsync();

                foreach (var existingField in existingFields)
                {
                    if (existingField.Polygon == null || existingField.Polygon.IsEmpty)
                        continue;

                    // Check for intersection
                    if (newPolygon.Intersects(existingField.Polygon))
                    {
                        return (false, $"The new field polygon intersects with existing field: {existingField.Name ?? existingField.Id.ToString()}");
                    }

                    // Check if new polygon is inside existing polygon
                    if (newPolygon.Within(existingField.Polygon))
                    {
                        return (false, $"The new field polygon is inside existing field: {existingField.Name ?? existingField.Id.ToString()}");
                    }

                    // Check if existing polygon is inside new polygon
                    if (existingField.Polygon.Within(newPolygon))
                    {
                        return (false, $"An existing field is inside the new field polygon: {existingField.Name ?? existingField.Id.ToString()}");
                    }
                }

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error validating field polygon");
                return (false, $"Error validating polygon: {ex.Message}");
            }
        }

        private string GetExt(FieldExportType eft) =>
            eft switch
            {
                FieldExportType.GeoJson => "geojson",
                FieldExportType.Kml => "kml",
                FieldExportType.CSV => "csv",
                FieldExportType.ShapeFile => "shapefile",
                _ => string.Empty,
            };
    }

    public class FieldSummaryItem
    {
        public Guid? RegionId { get; set; }
        public double Area { get; set; }
        public int Count { get; set; }
    }
}
