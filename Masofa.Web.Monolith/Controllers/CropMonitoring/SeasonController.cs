using Masofa.BusinessLogic;
using Masofa.BusinessLogic.CropMonitoring;
using Masofa.BusinessLogic.CropMonitoring.Fields;
using Masofa.BusinessLogic.ImportedFIeld;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using Masofa.Common.Models.CropMonitoring;
using Masofa.Common.Resources;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using Masofa.Web.Monolith.ViewModels.ImportedField;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Masofa.Web.Monolith.Controllers.CropMonitoring
{
    /// <summary>
    /// Provides API endpoints for managing and processing crop monitoring seasons.
    /// </summary>
    /// <remarks>This controller includes functionality for importing and exporting season data in various
    /// formats  (e.g., KML, GeoJSON, Shape files) and supports batch operations. It is secured with role-based 
    /// authorization and requires the caller to have one of the following roles: Admin, SystemAdmin,  ModuleAdmin, or
    /// Operator.</remarks>
    [Route("cropMonitoring/[controller]")]
    [ApiExplorerSettings(GroupName = "CropMonitoring")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, SystemAdmin, ModuleAdmin, Operator")]
    public class SeasonController : BaseCrudController<Masofa.Common.Models.CropMonitoring.Season, MasofaCropMonitoringDbContext>
    {
        private MasofaCropMonitoringDbContext MasofaCropMonitoringDbContext { get; set; }
        private Dictionary<Guid, CancellationTokenSource> _cancellationTokenSources = new Dictionary<Guid, CancellationTokenSource>();
        public SeasonController(
            IFileStorageProvider fileStorageProvider,
            MasofaCropMonitoringDbContext dbContext,
            ILogger<SeasonController> logger,
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

        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        public async Task<ActionResult<Dictionary<Guid, int>>> GetSeasonsByRegions([FromBody] RegionIdsRequest<Season> viewModel)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetSeasonsByRegions)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                var fields = await DbContext.Fields.Where(f => f.Status == StatusType.Active && f.RegionId != null && viewModel.RegionIds.Contains(f.RegionId.Value)).ToListAsync();
                var fieldIds = fields.Select(f => f.Id).ToList();

                var seasonsByFilter = await Mediator.Send(new BaseGetRequest<Masofa.Common.Models.CropMonitoring.Season, MasofaCropMonitoringDbContext>()
                {
                    Query = viewModel.Query
                });

                var seasons = seasonsByFilter.Where(s => s.FieldId != null)
                    .GroupBy(s => s.FieldId.Value)
                    .ToDictionary(
                        s => s.Key,
                        s => s.ToList()
                    );

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

                    if (seasons.TryGetValue(field.Id, out var seasonsByField))
                    {
                        if (!result.ContainsKey(field.RegionId.Value))
                        {
                            result[field.RegionId.Value] = 0;
                        }

                        result[field.RegionId.Value] += seasonsByField.Count;
                    }
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
        public async Task<ActionResult<List<Masofa.Common.Models.CropMonitoring.Season>>> GetSeasonsByCoordinates([FromBody] BoundingBoxRequest<Season> viewModel)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetSeasonsByCoordinates)}";
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

                var seasonsByQuery = await Mediator.Send(new BaseGetRequest<Season, MasofaCropMonitoringDbContext>()
                {
                    Query = viewModel.Query
                });

                if (seasonsByQuery == null)
                {
                    return NotFound();
                }

                var result = seasonsByQuery.Where(s => s.Polygon != null && s.Polygon.Intersects(boundingBox)).ToList();

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

        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        public async Task<ActionResult<List<MigrationImportedFieldResult>>> MigrationFromImportedFields([FromBody] MigrationImportedFieldViewModel viewModel)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(MigrationFromImportedFields)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                if (!ModelState.IsValid)
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelValidationFailed(requestPath, Newtonsoft.Json.JsonConvert.SerializeObject(viewModel)), requestPath);
                    return BadRequest(ModelState);
                }

                var migrateRequest = new MigrationFromImportedFieldsCommand()
                {
                    ImportedFieldReportId = viewModel.ImportedFieldReportId,
                    FieldMappings = viewModel.FieldMappings,
                    DefaultSeason = viewModel.DefaultSeason,
                    Author = User.Identity.Name
                };

                return await Mediator.Send(migrateRequest);
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
        /// Экспортирует посевы в формате GeoJson, KML, CSV или ESRI Shapefile в ZIP архиве
        /// </summary>
        /// <param name="viewModel">Параметры экспорта посевов</param>
        /// <returns>ZIP файл с KML данными полей</returns>
        /// <response code="200">ZIP файл с экспортированными полями</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost]
        [Route("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ExportSeasons([FromBody] SeasonExportRequest viewModel)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(ExportSeasons)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var request = new SeasonOrTemplateExportRequest()
                {
                    SeasonsQuery = viewModel.SeasonsQuery,
                    FieldExportType = viewModel.FieldExportType,
                    IsTemplate = false
                };

                var zip = await Mediator.Send(request);
                return File(zip, "application/zip", $"seasons_{GetExt(viewModel.FieldExportType)}.zip");
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
        /// <param name="exportType">Тип импорта посевов (GeoJson = 1, Kml = 2, CSV = 3, ShapeFile = 4)</param>
        /// <returns>ZIP файл с KML данными полей</returns>
        /// <response code="200">ZIP файл с экспортированными посевами</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet]
        [Route("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ImportSeasonsTemplate(FieldExportType exportType)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(ImportSeasonsTemplate)}";
            try
            {
                var templateRequest = new SeasonOrTemplateExportRequest()
                {
                    FieldExportType = exportType,
                    IsTemplate = true
                };

                var zip = await Mediator.Send(templateRequest);
                return File(zip, "application/zip", $"seasons_{GetExt(exportType)}.zip");
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
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

        ///// <summary>
        ///// Экспорт сезонов выбранных полей в KML (zip).
        ///// </summary>
        //[HttpPost]
        //[Route("[action]")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public async Task<IActionResult> ExportSeasons([FromBody] IEnumerable<Guid> fieldIds)
        //{
        //    var requestPath = $"{GetType().FullName}=>{nameof(ExportSeasons)}";
        //    try
        //    {
        //        var operationGuid = Guid.NewGuid();
        //        var cts = new CancellationTokenSource();

        //        _cancellationTokenSources[operationGuid] = cts;
        //        var bytes = await KmlExport.ExportSeasonsKmlZipAsync(fieldIds ?? Enumerable.Empty<Guid>(), cts.Token);
        //        _cancellationTokenSources.Remove(operationGuid);
        //        var fileName = $"seasons_kml_{DateTime.UtcNow:yyyyMMddHHmmss}.zip";
        //        return File(bytes, "application/zip", fileName);
        //    }
        //    catch (Exception ex)
        //    {
        //        var msg = LogMessageResource.GenericError(requestPath,errorMessage);
        //        await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
        //        Logger.LogCritical(ex, msg);
        //        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //    }
        //}

        ///// <summary>
        ///// Импорт сезонов из одного KML в указанное поле.
        ///// </summary>
        //[HttpPost]
        //[Route("[action]/{fieldId}")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public async Task<IActionResult> ImportKml(
        //    [FromRoute, Required] Guid fieldId,
        //    IFormFile file)
        //{
        //    var requestPath = $"{GetType().FullName}=>{nameof(ImportKml)}";
        //    try
        //    {
        //        if (file == null || file.Length == 0)
        //        {
        //            var errorMsg = "File is empty";
        //            await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
        //            return BadRequest(errorMsg);
        //        }

        //        await using var stream = file.OpenReadStream();
        //        var operationGuid = Guid.NewGuid();
        //        var cts = new CancellationTokenSource();

        //        _cancellationTokenSources[operationGuid] = cts;
        //        var seasons = await KmlImportService.ImportSeasonsAsync(fieldId, stream, cts.Token);

        //        var createdIds = new List<Guid>();
        //        foreach (var season in seasons)
        //        {
        //            season.FieldId = fieldId;

        //            var id = await Mediator.Send(
        //                new BaseCreateCommand<Season, MasofaCropMonitoringDbContext>
        //                {
        //                    Model = season
        //                }, cts.Token);

        //            createdIds.Add(id);
        //        }
        //        _cancellationTokenSources.Remove(operationGuid);
        //        return Ok(new { created = createdIds.Count, ids = createdIds });
        //    }
        //    catch (Exception ex)
        //    {
        //        var msg = LogMessageResource.GenericError(requestPath,errorMessage);
        //        await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
        //        Logger.LogCritical(ex, msg);
        //        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //    }
        //}

        //[HttpPost]
        //[Route("[action]")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public async Task<IActionResult> ImportKmlBatchAuto(
        //IFormFile archive)
        //{
        //    var requestPath = $"{GetType().FullName}=>{nameof(ImportKmlBatchAuto)}";
        //    try
        //    {
        //        if (archive is null || archive.Length == 0)
        //        {
        //            var errorMsg = "Archive is empty.";
        //            await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
        //            return BadRequest(errorMsg);
        //        }

        //        await using var stream = archive.OpenReadStream();
        //        var operationGuid = Guid.NewGuid();
        //        var cts = new CancellationTokenSource();

        //        _cancellationTokenSources[operationGuid] = cts;
        //        var seasons = await KmlImportService.ImportSeasonsAsyncFromZip(stream, cts.Token);

        //        if (seasons.Count == 0)
        //        {
        //            return Ok(new { created = 0, ids = Array.Empty<Guid>() });
        //        }

        //        var createdIds = new List<Guid>(seasons.Count);

        //        foreach (var s in seasons)
        //        {
        //            if (s.FieldId is null || s.FieldId == Guid.Empty)
        //                continue;

        //            var id = await Mediator.Send(
        //                new BaseCreateCommand<Season, MasofaCropMonitoringDbContext>
        //                {
        //                    Model = s
        //                }, cts.Token);

        //            createdIds.Add(id);
        //        }
        //        _cancellationTokenSources.Remove(operationGuid);
        //        return Ok(new { created = createdIds.Count, ids = createdIds });
        //    }
        //    catch (Exception ex)
        //    {
        //        var msg = LogMessageResource.GenericError(requestPath,errorMessage);
        //        await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
        //        Logger.LogCritical(ex, msg);
        //        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //    }
        //}

        ///// <summary>
        ///// Массовый импорт посева: принимает GeoJson.
        ///// Все записи будут импортированы в одно поле (fieldId).
        ///// </summary>
        //[HttpPost]
        //[Route("[action]/{fieldId}")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public async Task<IActionResult> ImportSeasonsGeoJson([FromRoute] Guid fieldId, IFormFile file)
        //{
        //    var requestPath = $"{GetType().FullName}=>{nameof(ImportSeasonsGeoJson)}";
        //    try
        //    {
        //        if (file is null || file.Length == 0)
        //        {
        //            var errorMsg = "File is empty";
        //            await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
        //            return BadRequest(errorMsg);
        //        }
        //        await using var s = file.OpenReadStream();
        //        var operationGuid = Guid.NewGuid();
        //        var cts = new CancellationTokenSource();

        //        _cancellationTokenSources[operationGuid] = cts;
        //        var seasons = await GeoJsonImportService.ImportSeasonsAsync(fieldId, s, cts.Token);

        //        var created = 0;
        //        foreach (var season in seasons)
        //        {
        //            await Mediator.Send(new BaseCreateCommand<Masofa.Common.Models.CropMonitoring.Season, MasofaCropMonitoringDbContext>
        //            {
        //                Model = season
        //            }, cts.Token);
        //            created++;
        //        }
        //        _cancellationTokenSources.Remove(operationGuid);
        //        return Ok(new { created });
        //    }
        //    catch (Exception ex)
        //    {
        //        var msg = LogMessageResource.GenericError(requestPath,errorMessage);
        //        await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
        //        Logger.LogCritical(ex, msg);
        //        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //    }
        //}

        ///// <summary>
        ///// Массовый импорт сезонов: принимает ZIP с несколькими GeoJson.
        ///// Все записи будут импортированы в одно поле (fieldId).
        ///// </summary>
        //[HttpPost]
        //[Route("[action]/{fieldId}")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public async Task<IActionResult> ImportSeasonsGeoJsonZip(IFormFile zip)
        //{
        //    var requestPath = $"{GetType().FullName}=>{nameof(ImportSeasonsGeoJsonZip)}";
        //    try
        //    {
        //        if (zip is null || zip.Length == 0)
        //        {
        //            var errorMsg = "File is empty";
        //            await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
        //            return BadRequest(errorMsg);
        //        }
        //        await using var s = zip.OpenReadStream();
        //        var operationGuid = Guid.NewGuid();
        //        var cts = new CancellationTokenSource();

        //        _cancellationTokenSources[operationGuid] = cts;
        //        var seasons = await GeoJsonImportService.ImportSeasonsFromZipAsync(s, cts.Token);

        //        var created = 0;
        //        foreach (var season in seasons)
        //        {
        //            await Mediator.Send(new BaseCreateCommand<Masofa.Common.Models.CropMonitoring.Season, MasofaCropMonitoringDbContext>
        //            {
        //                Model = season
        //            }, cts.Token);
        //            created++;
        //        }
        //        _cancellationTokenSources.Remove(operationGuid);
        //        return Ok(new { created });
        //    }
        //    catch (Exception ex)
        //    {
        //        var msg = LogMessageResource.GenericError(requestPath,errorMessage);
        //        await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
        //        Logger.LogCritical(ex, msg);
        //        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //    }
        //}

        ///// <summary>
        ///// Экспорт полей в GeoJson
        ///// </summary>
        //[HttpPost]
        //[Route("[action]")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public async Task<IActionResult> ExportSeasonsGeoJson([FromBody] IEnumerable<Guid>? fieldIds)
        //{
        //    var requestPath = $"{GetType().FullName}=>{nameof(ExportSeasonsGeoJson)}";
        //    try
        //    {
        //        var operationGuid = Guid.NewGuid();
        //        var cts = new CancellationTokenSource();

        //        _cancellationTokenSources[operationGuid] = cts;
        //        var zip = await GeoJsonExportService.ExportSeasonsGeoJsonZipAsync(fieldIds ?? Enumerable.Empty<Guid>(), cts.Token);
        //        _cancellationTokenSources.Remove(operationGuid);
        //        return File(zip, "application/zip", "seasons_geojson.zip");
        //    }
        //    catch (Exception ex)
        //    {
        //        var msg = LogMessageResource.GenericError(requestPath,errorMessage);
        //        await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
        //        Logger.LogCritical(ex, msg);
        //        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //    }
        //}

        ///// <summary>
        ///// Импорт сезонов из Shape (zip) в конкретное поле.
        ///// </summary>
        //[HttpPost]
        //[Route("[action]/{fieldId}")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public async Task<IActionResult> ImportSeasonsShape(Guid fieldId, IFormFile file)
        //{
        //    var requestPath = $"{GetType().FullName}=>{nameof(ImportSeasonsShape)}";
        //    try
        //    {
        //        if (file is null || file.Length == 0)
        //        {
        //            var errorMsg = "File is empty";
        //            await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
        //            return BadRequest(errorMsg);
        //        }

        //        await using var s = file.OpenReadStream();
        //        var operationGuid = Guid.NewGuid();
        //        var cts = new CancellationTokenSource();

        //        _cancellationTokenSources[operationGuid] = cts;
        //        var seasons = await ShapeImportService.ImportSeasonsFromShapeZipAsync(fieldId, s, cts.Token);

        //        var ids = new List<Guid>(seasons.Count());
        //        foreach (var season in seasons)
        //        {
        //            var id = await Mediator.Send(
        //                new BaseCreateCommand<Season, MasofaCropMonitoringDbContext> { Model = season }, cts.Token);
        //            ids.Add(id);
        //        }
        //        _cancellationTokenSources.Remove(operationGuid);
        //        return Ok(new { created = ids.Count, ids });
        //    }
        //    catch (Exception ex)
        //    {
        //        var msg = LogMessageResource.GenericError(requestPath,errorMessage);
        //        await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
        //        Logger.LogCritical(ex, msg);
        //        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //    }
        //}

        ///// <summary>
        ///// Импорт сезонов из Shape (zip) с авто-привязкой к полям по геометрии.
        ///// </summary>
        //[HttpPost]
        //[Route("[action]")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public async Task<IActionResult> ImportSeasonsShapeAuto(IFormFile file)
        //{
        //    var requestPath = $"{GetType().FullName}=>{nameof(ImportSeasonsShapeAuto)}";
        //    try
        //    {
        //        if (file is null || file.Length == 0)
        //        {
        //            var errorMsg = "File is empty";
        //            await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
        //            return BadRequest(errorMsg);
        //        }

        //        await using var s = file.OpenReadStream();

        //        var operationGuid = Guid.NewGuid();
        //        var cts = new CancellationTokenSource();

        //        _cancellationTokenSources[operationGuid] = cts;
        //        var seasons = await ShapeImportService.ImportSeasonsFromShapeZipAndResolveFieldAsync(s, cts.Token);

        //        var ids = new List<Guid>(seasons.Count());
        //        foreach (var season in seasons)
        //        {
        //            var id = await Mediator.Send(
        //                new BaseCreateCommand<Season, MasofaCropMonitoringDbContext> { Model = season }, cts.Token);
        //            ids.Add(id);
        //        }
        //        _cancellationTokenSources.Remove(operationGuid);
        //        return Ok(new { created = ids.Count, ids });
        //    }
        //    catch (Exception ex)
        //    {
        //        var msg = LogMessageResource.GenericError(requestPath,errorMessage);
        //        await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
        //        Logger.LogCritical(ex, msg);
        //        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //    }
        //}

        ///// <summary>
        ///// Экспорт полей в Shape
        ///// </summary>
        //[HttpPost]
        //[Route("[action]")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public async Task<IActionResult> ExportSeasonsShape(IEnumerable<Guid>? fieldIds)
        //{
        //    var requestPath = $"{GetType().FullName}=>{nameof(ExportSeasonsShape)}";
        //    try
        //    {
        //        var operationGuid = Guid.NewGuid();
        //        var cts = new CancellationTokenSource();

        //        _cancellationTokenSources[operationGuid] = cts;
        //        var zip = await ShapeExportService.ExportSeasonsShapeZipAsync(fieldIds ?? Enumerable.Empty<Guid>(), cts.Token);
        //        _cancellationTokenSources.Remove(operationGuid);
        //        return File(zip, "application/zip", "seasons_shapefile.zip");
        //    }
        //    catch (Exception ex)
        //    {
        //        var msg = LogMessageResource.GenericError(requestPath,errorMessage);
        //        await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
        //        Logger.LogCritical(ex, msg);
        //        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //    }
        //}
    }
}
