using Masofa.BusinessLogic.Index;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using Masofa.Common.Models.Satellite.Indices;
using Masofa.Common.Resources;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Masofa.Web.Monolith.Controllers.Satellite.Indices
{
    /// <summary>
    /// Anomaly Polygons Controller
    /// </summary>
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, SystemAdmin, ModuleAdmin, Operator")]
    [Route("satellite/[controller]")]
    [ApiExplorerSettings(GroupName = "SatelliteIndices")]
    public class AnomalyPolygonsController : BaseCrudController<AnomalyPolygon, MasofaIndicesDbContext>
    {
        /// <summary>
        /// Constructor for Anomaly Polygons Controller
        /// </summary>
        public AnomalyPolygonsController(
            IFileStorageProvider fileStorageProvider,
            MasofaIndicesDbContext dbContext,
            ILogger<AnomalyPolygonsController> logger,
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
        /// Obtaining polygons by field and date
        /// </summary>
        /// <param name="request">Model with Field ID and Date</param>
        /// <returns>Polygons for field and date</returns>
        /// <response code="200">Found polygons</response>
        /// <response code="401">Invalid login or password</response>
        /// <response code="404">Polygons for this field or date not found</response>
        /// <response code="500">Internal Server Error. Unhandled.</response>
        [HttpPost]
        [Route("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<List<AnomalyPolygon>>> GetAnomalyPolygonsByFieldAndDate([FromBody] GetAnomalyPolygonsByFieldAndDateCommand request)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetAnomalyPolygonsByFieldAndDate)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await Mediator.Send(request);

                if (result is null || result.Count == 0)
                {
                    var errorMsg = LogMessageResource.EntityNotFound(typeof(AnomalyPolygon).FullName, $"FieldId {request.FieldId}; Date {request.Date}");
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return NotFound(errorMsg);
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

        /// <summary>
        /// Obtaining polygons by season and date
        /// </summary>
        /// <param name="request">Model with Season ID and Date</param>
        /// <returns>Polygons for season and date</returns>
        /// <response code="200">Found polygons</response>
        /// <response code="401">Invalid login or password</response>
        /// <response code="404">Polygons for this season or date not found</response>
        /// <response code="500">Internal Server Error. Unhandled.</response>
        [HttpPost]
        [Route("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<List<AnomalyPolygon>>> GetAnomalyPolygonsBySeasonAndDate([FromBody] GetAnomalyPolygonsBySeasonAndDateCommand request)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetAnomalyPolygonsBySeasonAndDate)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await Mediator.Send(request);

                if (result is null || result.Count == 0)
                {
                    var errorMsg = LogMessageResource.EntityNotFound(typeof(AnomalyPolygon).FullName, $"SeasonId {request.SeasonId}; Date {request.Date}");
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return NotFound(errorMsg);
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

        /// <summary>
        /// Obtaining polygons by coordinates and date
        /// </summary>
        /// <param name="request">Model with coordinates and Date</param>
        /// <returns>Polygons for coordinates and date</returns>
        /// <response code="200">Found polygons</response>
        /// <response code="401">Invalid login or password</response>
        /// <response code="404">Polygons for this coordinates or date not found</response>
        /// <response code="500">Internal Server Error. Unhandled.</response>
        [HttpPost]
        [Route("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<List<AnomalyPolygon>>> GetAnomalyPolygonsByCoordinatesAndDate([FromBody] GetAnomalyPolygonsByCoordinatesAndDateCommand request)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetAnomalyPolygonsByCoordinatesAndDate)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (request.West >= request.East || request.South >= request.North)
                {
                    return BadRequest("Invalid bounding box coordinates.");
                }

                var result = await Mediator.Send(request);

                if (result is null || result.Count == 0)
                {
                    var errorMsg = LogMessageResource.EntityNotFound(typeof(AnomalyPolygon).FullName, $"{Newtonsoft.Json.JsonConvert.SerializeObject(request)}");
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return NotFound(errorMsg);
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

        /// <summary>
        /// Obtaining polygons
        /// </summary>
        /// <param name="request">Custom get model</param>
        /// <returns>Anomaly polygons</returns>
        /// <response code="200">Found polygons</response>
        /// <response code="401">Invalid login or password</response>
        /// <response code="404">Polygons for this coordinates or date not found</response>
        /// <response code="500">Internal Server Error. Unhandled.</response>
        [HttpPost]
        [Route("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<List<CustomAnomalyPolygonsViewModel>>> CustomGetAnomalyPolygons([FromBody] BaseGetQuery<AnomalyPolygon> viewModel)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetAnomalyPolygonsByCoordinatesAndDate)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var request = new CustomGetAnomalyPolygonsCommand()
                {
                    GetQuery = new BaseGetQuery<AnomalyPolygon>()
                    {
                        Offset = viewModel.Offset,
                        Sort = viewModel.Sort,
                        Take = viewModel.Take,
                        SortBy = viewModel.SortBy,
                        SelectFields = viewModel.SelectFields,
                        Filters = new List<FieldFilter>()
                    }
                };

                foreach (var item in viewModel.Filters)
                {
                    if (item.FilterField.ToLower() == "CropIds".ToLower())
                    {
                        request.CropIds = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Guid>>(item.FilterValue.ToString() ?? "[]");
                        continue;
                    }

                    if (item.FilterField.ToLower() == "RegionIds".ToLower())
                    {
                        request.RegionIds = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Guid>>(item.FilterValue.ToString() ?? "[]");
                        continue;
                    }
                    request.GetQuery.Filters.Add(item);
                }

                var result = await Mediator.Send(request);

                if (result is null || result.Count == 0)
                {
                    var errorMsg = LogMessageResource.EntityNotFound(typeof(AnomalyPolygon).FullName, $"{Newtonsoft.Json.JsonConvert.SerializeObject(request)}");
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return NotFound(errorMsg);
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
    }
}
