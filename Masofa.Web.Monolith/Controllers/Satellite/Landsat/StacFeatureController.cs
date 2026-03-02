using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Satellite.Landsat;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Masofa.Web.Monolith.Controllers.Satellite
{
    [Route("satellite/[controller]")]
    [ApiExplorerSettings(GroupName = "Satellite")]
    public class StacFeatureController : BaseCrudController<StacFeatureEntity, MasofaLandsatDbContext>
    {
        public StacFeatureController(
            IFileStorageProvider fileStorageProvider,
            MasofaLandsatDbContext dbContext,
            ILogger<StacFeatureController> logger,
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
