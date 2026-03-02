using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Identity;
using Masofa.Common.Models.Satellite.Landsat;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Masofa.Web.Monolith.Controllers.Satellite
{
    [Route("satellite/[controller]")]
    [ApiExplorerSettings(GroupName = "SatelliteLandsat")]
    public class Level1RadiometricRescalingController : BaseCrudController<Level1RadiometricRescalingEntity, MasofaLandsatDbContext>
    {
        public Level1RadiometricRescalingController(
            IFileStorageProvider fileStorageProvider,
            MasofaLandsatDbContext dbContext,
            UserManager<User> userManager,
            ILogger<Level1RadiometricRescalingController> logger,
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
