using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Era;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Masofa.Web.Monolith.Controllers.WeatherReport
{
    [Route("weatherReports/[controller]")]
    [ApiExplorerSettings(GroupName = "WeatherReports")]
    public class Era5WeekWeatherReportController : BaseEra5WeatherReportController<Era5WeekWeatherReport, MasofaEraDbContext>
    {
        public Era5WeekWeatherReportController(IFileStorageProvider fileStorageProvider, 
            MasofaEraDbContext dbContext, 
            ILogger<Era5WeekWeatherReportController> logger, 
            IConfiguration configuration, 
            IMediator mediator, 
            IBusinessLogicLogger businessLogicLogger, 
            IHttpContextAccessor httpContextAccessor) 
            : base(fileStorageProvider, dbContext, logger, configuration, mediator, businessLogicLogger, httpContextAccessor)
        {
        }
    }
}
