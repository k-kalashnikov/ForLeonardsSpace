using Masofa.BusinessLogic;
using Masofa.BusinessLogic.Index;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using Masofa.Common.Models.Satellite.Indices;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Masofa.Web.Monolith.Controllers.Satellite.Indices
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, SystemAdmin, ModuleAdmin, Operator")]
    [Microsoft.AspNetCore.Mvc.Route("[controller]")]
    public class BaseIndexesController<TIndexDB, TIndexTiff, TIndexSeasonReport, TIndexSharedReport> : BaseController
        where TIndexDB : BaseIndexPoint
        where TIndexTiff : BaseIndexPolygon
        where TIndexSeasonReport : IndexReportSeason
        where TIndexSharedReport : IndexReportShared
    {
        private MasofaIndicesDbContext MasofaIndicesDbContext { get; set; }
        protected IFileStorageProvider FileStorageProvider { get; set; }
        protected IBusinessLogicLogger BusinessLogicLogger { get; set; }
        protected IHttpContextAccessor HttpContextAccessor { get; set; }
        public BaseIndexesController(ILogger logger, IConfiguration configuration, IMediator mediator, MasofaIndicesDbContext masofaIndicesDbContext, IFileStorageProvider fileStorageProvider, IBusinessLogicLogger businessLogicLogger, IHttpContextAccessor httpContextAccessor) : base(logger, configuration, mediator)
        {
            MasofaIndicesDbContext = masofaIndicesDbContext;
            FileStorageProvider = fileStorageProvider;
            BusinessLogicLogger = businessLogicLogger;
            HttpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        public async Task<ActionResult<List<TIndexDB>>> GetIndexDbByQuery([FromBody] BaseGetQuery<TIndexDB> query)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetIndexDbByQuery)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                if (!ModelState.IsValid)
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelValidationFailed(requestPath, Newtonsoft.Json.JsonConvert.SerializeObject(query)), requestPath);
                    return BadRequest(ModelState);
                }
                var getRequest = new BaseIndexesGetRequest<TIndexDB>()
                {
                    Query = query
                };

                return await Mediator.Send(getRequest);
            }
            catch (Exception ex)
            {
                var msg = $"Something wrong in {GetType().FullName}=>{nameof(GetIndexDbByQuery)}";
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        public async Task<ActionResult<List<TIndexTiff>>> GetIndexTiffByQuery([FromBody] BaseGetQuery<TIndexTiff> query)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetIndexTiffByQuery)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                if (!ModelState.IsValid)
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelValidationFailed(requestPath, Newtonsoft.Json.JsonConvert.SerializeObject(query)), requestPath);
                    return BadRequest(ModelState);
                }
                var getRequest = new BaseIndexesGetRequest<TIndexTiff>()
                {
                    Query = query
                };

                return await Mediator.Send(getRequest);
            }
            catch (Exception ex)
            {
                var msg = $"Something wrong in {GetType().FullName}=>{nameof(GetIndexTiffByQuery)}";
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        public async Task<ActionResult<List<TIndexSeasonReport>>> GetIndexSeasonReports([FromBody] BaseGetQuery<TIndexSeasonReport> query)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetIndexTiffByQuery)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                if (!ModelState.IsValid)
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelValidationFailed(requestPath,Newtonsoft.Json.JsonConvert.SerializeObject(query)), requestPath);
                    return BadRequest(ModelState);
                }
                var getRequest = new BaseGetIndexReportRequest<TIndexSeasonReport>()
                {
                    Query = query
                };

                return await Mediator.Send(getRequest);
            }
            catch (Exception ex)
            {
                var msg = $"Something wrong in {GetType().FullName}=>{nameof(GetIndexTiffByQuery)}";
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        public async Task<ActionResult<List<TIndexSharedReport>>> GetIndexSharedReports([FromBody] BaseGetQuery<TIndexSharedReport> query)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetIndexTiffByQuery)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                if (!ModelState.IsValid)
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelValidationFailed(requestPath,Newtonsoft.Json.JsonConvert.SerializeObject(query)), requestPath);
                    return BadRequest(ModelState);
                }
                var getRequest = new BaseGetIndexReportRequest<TIndexSharedReport>()
                {
                    Query = query
                };

                return await Mediator.Send(getRequest);
            }
            catch (Exception ex)
            {
                var msg = $"Something wrong in {GetType().FullName}=>{nameof(GetIndexTiffByQuery)}";
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        public async Task<ActionResult<List<DateOnly>>> GetIndexAvailableDates([FromBody] IndexAvailableDatesRequestDto request)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetIndexAvailableDates)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                if (!ModelState.IsValid)
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelValidationFailed(requestPath, JsonConvert.SerializeObject(request)), requestPath);
                    return BadRequest(ModelState);
                }

                var mediatorRequest = new GetIndexAvailableDatesRequest<TIndexSharedReport>()
                {
                    CropId = request.CropId,
                    RegionId = request.RegionId,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate
                };

                var dates = await Mediator.Send(mediatorRequest);
                return Ok(dates);
            }
            catch (Exception ex)
            {
                var msg = $"Something wrong in {GetType().FullName}=>{nameof(GetIndexAvailableDates)}";
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Получает среднее значение индекса по региону за период времени.
        /// Возвращает Dictionary где ключ - дата (DateOnly), значение - средний индекс по ВСЕМ культурам в регионе.
        /// </summary>
        /// <param name="startDate">Начальная дата периода</param>
        /// <param name="endDate">Конечная дата периода</param>
        /// <param name="regionId">Идентификатор региона (nullable - если null, то по всем регионам)</param>
        /// <returns>Dictionary где ключ - дата, значение - средний индекс</returns>
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        public async Task<ActionResult<Dictionary<DateOnly, double>>> GetIndexAverageByRegionAndDateRange(
            [FromQuery] DateOnly startDate,
            [FromQuery] DateOnly endDate,
            [FromQuery] Guid? regionId = null)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetIndexAverageByRegionAndDateRange)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(
                    $"Start request in {requestPath}: StartDate={startDate}, EndDate={endDate}, RegionId={regionId}",
                    requestPath);

                if (startDate > endDate)
                {
                    await BusinessLogicLogger.LogErrorAsync(
                        $"StartDate ({startDate}) cannot be greater than EndDate ({endDate}) in {requestPath}",
                        requestPath);
                    return BadRequest("StartDate cannot be greater than EndDate");
                }

                var request = new GetIndexAverageByRegionAndDateRangeRequest<TIndexSharedReport>()
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    RegionId = regionId
                };

                var result = await Mediator.Send(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var msg = $"Something wrong in {GetType().FullName}=>{nameof(GetIndexAverageByRegionAndDateRange)}";
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Returns available index dates based on GeoServer folder structure.
        /// The method scans GeoServer data directories and extracts dates
        /// from folders named in the format: {INDEX}_{yyyyMMdd}.
        /// Only dates containing at least one TIFF file are returned.
        /// </summary>
        /// <param name="request">
        /// Request DTO containing:
        /// - IndexName: name of the index (e.g. NDVI, EVI, NDMI)
        /// - Year: year to filter available index dates
        /// </param>
        /// <returns>
        /// List of available dates for the specified index and year.
        /// </returns>
        /// <response code="200">List of available index dates</response>
        /// <response code="400">Invalid request parameters</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        public async Task<ActionResult<List<DateOnly>>> GetIndexDatesFromGeoServerFolders([FromBody] IndexFolderDatesRequestViewModel request)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetIndexDatesFromGeoServerFolders)}";

            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                if (!ModelState.IsValid)
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelValidationFailed(requestPath, JsonConvert.SerializeObject(request)), requestPath);

                    return BadRequest(ModelState);
                }

                var indexName = request.IndexName.Trim();

                if (!Regex.IsMatch(indexName, "^[A-Za-z0-9_]+$"))
                {
                    return BadRequest("IndexName contains invalid characters.");
                }

                var rootPath = Configuration["GeoServerDataRoot"] ?? "/deploy/prod/data-geoserver-prod";

                var indexRootPath = Path.Combine(rootPath, indexName);

                if (!Directory.Exists(indexRootPath))
                {
                    return Ok(new List<DateOnly>());
                }

                var prefix = $"{indexName}_{request.Year}";
                var directories = Directory.EnumerateDirectories(indexRootPath, prefix + "*", SearchOption.TopDirectoryOnly);

                var result = new List<DateOnly>();

                foreach (var directory in directories)
                {
                    var folderName = Path.GetFileName(directory);

                    if (folderName.Length < indexName.Length + 1 + 8)
                        continue;

                    var datePart = folderName.Substring(indexName.Length + 1, 8);

                    if (!DateTime.TryParseExact(datePart, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
                    {
                        continue;
                    }

                    var hasTiff = Directory.EnumerateFiles(directory, "*.tif").Any() || Directory.EnumerateFiles(directory, "*.tiff").Any();

                    if (hasTiff)
                    {
                        result.Add(DateOnly.FromDateTime(parsedDate));
                    }
                }

                return Ok(result.Distinct().OrderBy(d => d).ToList());
            }
            catch (Exception ex)
            {
                var message = $"Something went wrong in {GetType().FullName}=>{nameof(GetIndexDatesFromGeoServerFolders)}";

                await BusinessLogicLogger.LogCriticalAsync(message, requestPath);
                Logger.LogCritical(ex, message);

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }

    public class IndexFolderDatesRequestViewModel
    {
        [Required]
        public string IndexName { get; set; } = default!;

        public int Year { get; set; }
    }

    public class IndexAvailableDatesRequestDto
    {
        [Required]
        public Guid CropId { get; set; }

        public Guid? RegionId { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }
    }
}
