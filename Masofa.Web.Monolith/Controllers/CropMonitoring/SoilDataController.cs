using Masofa.BusinessLogic.CropMonitoring.Bids;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.CropMonitoring;
using Masofa.Common.Models.Satellite.Parse.Sentinel.Inspire;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Masofa.Web.Monolith.Controllers.CropMonitoring
{
    /// <summary>
    /// Контроллер для работы с данными почвы
    /// </summary>
    [Route("cropMonitoring/[controller]")]
    [ApiExplorerSettings(GroupName = "CropMonitoring")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, SystemAdmin, ModuleAdmin, Operator")]
    public class SoilDataController : BaseCrudController<SoilData, MasofaCropMonitoringDbContext>
    {
        private MasofaCropMonitoringDbContext MasofaCropMonitoringDbContext { get; set; }
        public SoilDataController(IFileStorageProvider fileStorageProvider, MasofaCropMonitoringDbContext dbContext, ILogger<SoilDataController> logger, IConfiguration configuration, IMediator mediator, IBusinessLogicLogger businessLogicLogger, IHttpContextAccessor httpContextAccessor) : base(fileStorageProvider, dbContext, logger, configuration, mediator, businessLogicLogger, httpContextAccessor)
        {
            MasofaCropMonitoringDbContext = dbContext;
        }

        /// <summary>
        /// Находит данные почы поле по географическим координатам точки
        /// </summary>
        /// <param name="x">Долгота (longitude)</param>
        /// <param name="y">Широта (latitude)</param>
        /// <param name="srid">Система координат (по умолчанию 4326)</param>
        /// <returns>данные почвы, содержащее указанную точку</returns>
        /// <response code="200">данные почвы найдено</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="404">данные почвы с указанными координатами не найдено</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet]
        [Route("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<SoilData>> GetByPoints([FromQuery] double x, [FromQuery] double y, [FromQuery] string parameter, [FromQuery] string depthRange, [FromQuery] int srid = 4326)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetByPoints)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                var pt = new Point(x, y) { SRID = srid };

                var exactMatch = await MasofaCropMonitoringDbContext.SoilDatas
                    .FirstOrDefaultAsync(s =>
                        s.Point.Equals(pt) &&
                        s.Parameter == parameter &&
                        s.DepthRange == depthRange);

                if (exactMatch != null)
                {
                    return Ok(new SoilDataResponse(exactMatch));
                }

                var nearestPoints = await MasofaCropMonitoringDbContext.SoilDatas
                    .Where(s => s.Parameter == parameter && s.DepthRange == depthRange)
                    .OrderBy(s => s.Point.Distance(pt))
                    .Take(4)
                    .ToListAsync();

                if (!nearestPoints.Any())
                {
                    return NotFound("No soil data found in the vicinity");
                }

                var values = nearestPoints.Where(n => n.Value.HasValue).Select(n => n.Value.Value).ToList();
                if (!values.Any())
                {
                    return NotFound("No valid values in nearby points");
                }

                double avg = values.Average();
                double maxDeviation = values.Max(v => Math.Abs(v - avg));

                bool areClose = maxDeviation < (avg * 0.1);

                SoilData result;
                if (areClose && values.Count > 1)
                {
                    result = new SoilData
                    {
                        Id = Guid.NewGuid(),
                        Point = pt,
                        Parameter = parameter,
                        DepthRange = depthRange,
                        Value = avg,
                        Unit = nearestPoints.First().Unit,
                        Source = "Interpolated",
                        CreateAt = DateTime.UtcNow,
                        LastUpdateAt = DateTime.UtcNow,
                        Status = Masofa.Common.Models.StatusType.Active,
                        CreateUser = Guid.Empty,
                        LastUpdateUser = Guid.Empty,
                        TileKey = nearestPoints.First().TileKey
                    };
                }
                else
                {
                    result = nearestPoints.First();
                }

                if (result is null)
                {
                    var errorMsg = LogMessageResource.EntityNotFound(typeof(SoilData).FullName, $"lon {x}; lat {y}; srid {srid}");
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return NotFound(errorMsg);
                }

                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, Newtonsoft.Json.JsonConvert.SerializeObject(result)), requestPath);
                return Ok(new SoilDataResponse(result));
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        public class SoilDataResponse
        {
            public Guid Id { get; set; }
            public double X { get; set; }
            public double Y { get; set; }
            public string Parameter { get; set; }
            public string DepthRange { get; set; }
            public double? Value { get; set; }
            public string Unit { get; set; }
            public string Source { get; set; }
            public bool IsInterpolated { get; set; }

            public SoilDataResponse(SoilData data)
            {
                Id = data.Id;
                X = data.Point.X;
                Y = data.Point.Y;
                Parameter = data.Parameter;
                DepthRange = data.DepthRange;
                Value = data.Value;
                Unit = data.Unit;
                Source = data.Source;
                IsInterpolated = data.Source == "Interpolated";
            }
        }
    }
}
