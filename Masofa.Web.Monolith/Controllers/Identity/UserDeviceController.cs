using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Identity;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Masofa.Web.Monolith.Controllers.Identity
{
    /// <summary>
    /// Контроллер для работы с сущностью пользовательских устройств
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("identity/[controller]")]
    [ApiExplorerSettings(GroupName = "Identity")]
    public class UserDeviceController : BaseCrudController<UserDevice, MasofaIdentityDbContext>
    {
        public UserDeviceController(
            IFileStorageProvider fileStorageProvider,
            MasofaIdentityDbContext dbContext,
            ILogger<UserDeviceController> logger,
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
