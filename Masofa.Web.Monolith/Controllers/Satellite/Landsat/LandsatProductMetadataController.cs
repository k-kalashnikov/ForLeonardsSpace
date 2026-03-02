using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Satellite;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Masofa.Web.Monolith.Controllers.Satellite
{
    [Route("satellite/[controller]")]
    [ApiExplorerSettings(GroupName = "SatelliteLandsat")]
    public class LandsatProductMetadataController : BaseCrudController<LandsatProductMetadata, MasofaLandsatDbContext>
    {
        public LandsatProductMetadataController(
            IFileStorageProvider fileStorageProvider,
            MasofaLandsatDbContext dbContext,
            ILogger<LandsatProductMetadataController> logger,
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
