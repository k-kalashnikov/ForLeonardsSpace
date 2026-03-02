using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Satellite.Landsat;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Masofa.Web.Monolith.Controllers.Satellite
{
    [Route("satellite/[controller]")]
    [ApiExplorerSettings(GroupName = "SatelliteLandsat")]
    public class Level1ProjectionParametersController : BaseCrudController<Level1ProjectionParametersEntity, MasofaLandsatDbContext>
    {
        public Level1ProjectionParametersController(
            IFileStorageProvider fileStorageProvider,
            MasofaLandsatDbContext dbContext,
            ILogger<Level1ProjectionParametersController> logger,
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
