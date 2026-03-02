using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Identity;
using Masofa.Common.Models.Satellite.Sentinel;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Masofa.Web.Monolith.Controllers.Satellite
{
    [Route("satellite/[controller]")]
    [ApiExplorerSettings(GroupName = "SatelliteSentinel")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, SystemAdmin, ModuleAdmin, Operator")]
    public class SentinelProductQualityMetadataController : BaseCrudController<SentinelProductQualityMetadata, MasofaSentinelDbContext>
    {
        public SentinelProductQualityMetadataController(
            IFileStorageProvider fileStorageProvider,
            MasofaSentinelDbContext dbContext,
            UserManager<User> userManager,
            ILogger<SentinelProductQualityMetadataController> logger,
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