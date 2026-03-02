using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Era;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using Masofa.Web.Monolith.ViewModels.WeatherReport;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Masofa.Web.Monolith.Controllers.WeatherReport
{
    [Route("weatherReports/[controller]")]
    [ApiExplorerSettings(GroupName = "WeatherReports")]
    public class Era5MonthWeatherReportController : BaseEra5WeatherReportController<Era5MonthWeatherReport, MasofaEraDbContext>
    {
        public Era5MonthWeatherReportController(IFileStorageProvider fileStorageProvider, 
            MasofaEraDbContext dbContext, 
            ILogger<Era5MonthWeatherReportController> logger, 
            IConfiguration configuration, 
            IMediator mediator, 
            IBusinessLogicLogger businessLogicLogger, 
            IHttpContextAccessor httpContextAccessor) 
            : base(fileStorageProvider, dbContext, logger, configuration, mediator, businessLogicLogger, httpContextAccessor)
        {
        }

        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        public virtual async Task<ActionResult<Era5MonthWeatherReport>> GetByCoordinatesAndDate([FromBody] CoordinatesAndDateViewModel query)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetByCoordinatesAndDate)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                if (!ModelState.IsValid)
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelValidationFailed(requestPath, Newtonsoft.Json.JsonConvert.SerializeObject(query)), requestPath);
                    return BadRequest(ModelState);
                }

                var targetPoint = new Point(query.Longitude, query.Latitude)
                {
                    SRID = 4326
                };

                var closestStation = await DbContext.EraWeatherStations
                    .OrderBy(s => s.Point.Distance(targetPoint))
                    .FirstOrDefaultAsync();

                if (closestStation is null)
                {
                    return NotFound();
                }

                var inputDate = DateOnly.FromDateTime(query.InputDate);

                var closestFutureReport = await DbContext.Era5MonthWeatherReports
                    .Where(x => x.WeatherStation == closestStation.Id)
                    .Where(m => m.Year == query.InputDate.Year)
                    .Where(m => m.Month == query.InputDate.Month)
                    .OrderBy(x => x.Year)
                    .ThenBy(m => m.Month)
                    .FirstOrDefaultAsync();

                if (closestFutureReport is null)
                {
                    return NotFound();
                }

                return closestFutureReport;
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
