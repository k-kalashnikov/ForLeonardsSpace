using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Identity;
using Masofa.Common.Models.Satellite.Landsat;
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
    [ApiExplorerSettings(GroupName = "Satellite")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, SystemAdmin, ModuleAdmin, Operator")]
    public class StTemperatureAssetController : BaseCrudController<StTemperatureAssetEntity, MasofaLandsatDbContext>
    {
        public StTemperatureAssetController(
            IFileStorageProvider fileStorageProvider,
            MasofaLandsatDbContext dbContext,
            UserManager<User> userManager,
            ILogger<StTemperatureAssetController> logger,
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
