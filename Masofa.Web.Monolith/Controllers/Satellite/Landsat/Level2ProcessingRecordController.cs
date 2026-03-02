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
    public class Level2ProcessingRecordController : BaseCrudController<Level2ProcessingRecordEntity, MasofaLandsatDbContext>
    {
        public Level2ProcessingRecordController(
            IFileStorageProvider fileStorageProvider, 
            MasofaLandsatDbContext dbContext, 
            UserManager<User> userManager, 
            ILogger<Level2ProcessingRecordController> logger, 
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
