using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.IBMWeather;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Masofa.Web.Monolith.Controllers.IBMWeather
{
    /// <summary>
    /// Контроллер для работы с IBMMeteoStation
    /// </summary>
    [Route("ibm-weather/[controller]")]
    [ApiExplorerSettings(GroupName = "IBMWeather")]
    public class IBMWeatherDataController : BaseCrudController<IBMWeatherData, MasofaIBMWeatherDbContext>
    {
        public IBMWeatherDataController(
            IFileStorageProvider fileStorageProvider,
            MasofaIBMWeatherDbContext dbContext,
            ILogger<IBMWeatherDataController> logger,
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
    }
}
