using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Masofa.Web.Monolith.Controllers.Common
{
    [Route("common/[controller]")]
    [ApiExplorerSettings(GroupName = "Common")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, SystemAdmin, ModuleAdmin, Operator")]
    public class AccessMapItemController : BaseCrudController<AccessMapItem, MasofaCommonDbContext>
    {
        public AccessMapItemController(IFileStorageProvider fileStorageProvider, MasofaCommonDbContext dbContext, ILogger logger, IConfiguration configuration, IMediator mediator, IBusinessLogicLogger businessLogicLogger, IHttpContextAccessor httpContextAccessor) : base(fileStorageProvider, dbContext, logger, configuration, mediator, businessLogicLogger, httpContextAccessor)
        {
        }
    }
}
